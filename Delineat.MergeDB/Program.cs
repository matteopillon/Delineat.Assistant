﻿using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delineat.MergeDB
{
    class Program
    {
        private static ILogger logger;
        private static DAAssistantDBContext dbContext = null;
        private static System.Data.OleDb.OleDbConnection accessConnection = null;
        private const string kNotFoundDescription = "DESCRIZIONE_NON_IMPOSTATA";
        static void Main(string[] args)
        {
            try
            {
                logger = new LoggerConfiguration()
                        .WriteTo.File("import.log")
                        .CreateLogger();

                var confBuilder = new ConfigurationBuilder();

                confBuilder.AddJsonFile("appsettings.json");
                var config = confBuilder.Build();



                LogDebug("Avvio importazione");

                using (accessConnection = new System.Data.OleDb.OleDbConnection(config.GetConnectionString("access")))
                {
                    LogDebug("Apertura DB Access");
                    accessConnection.Open();
                    LogDebug("Apertura DB Access OK");
                    LogDebug("Apertura DB SQL Server");
                    dbContext = new DAAssistantDBContext(config.GetConnectionString("defaultConnection"));
                    LogDebug("Apertura DB SQL Server OK");
                    MergeDB();
                    accessConnection.Close();
                }






            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
            }
            LogDebug("Operazione completata");
            Console.ReadLine();
        }

        private static void MergeDB()
        {

            //ReadTypes();
            //ReadUsers();
            ReadCustomers();
            //ReadJobsCustomers();
            //ReadSubJobs();

            //ReadJobExtraInfo();
            //ReadWorkLogs();
        }


        private static void ReadTypes()
        {
            LogDebug("Caricamento tipologie");
            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readTypesCommand = accessConnection.CreateCommand())
            {
                readTypesCommand.CommandText = "SELECT Tipologia, derscrizione FROM tipologie";
                using (var typesReader = readTypesCommand.ExecuteReader())
                {
                    while (typesReader.Read())
                    {
                        if (!typesReader.IsDBNull(0))
                        {
                            var type = typesReader.GetString(0);

                            if (!string.IsNullOrWhiteSpace(type))
                            {
                                var description = !typesReader.IsDBNull(1) ? typesReader.GetString(1) : string.Empty;
                                var dbType = dbContext.DayWorkTypes.FirstOrDefault(c => c.Code == type);
                                if (dbType == null)
                                {
                                    dbType = new DayWorkType();
                                    dbType.Code = type;
                                    dbType.Description = description;

                                    dbContext.DayWorkTypes.Add(dbType);
                                    dbContext.SaveChanges();
                                }
                            }

                        }
                    }
                }

            }
            LogDebug("Caricamento tipologie completato");
        }

        private static void ReadUsers()
        {


            LogDebug("Caricamento utenti");
            const string kAdminRoleCode = "ADMIN";
            const string kDefaultRoleCode = "DEFAULT";
            const string kUserRoleCode = "USER";

            var adminPermission = dbContext.Permissions.FirstOrDefault(p => p.Id == kAdminRoleCode);
            if (adminPermission == null)
            {
                adminPermission = new Permission();
                adminPermission.Enabled = true;
                adminPermission.Id = kAdminRoleCode;
                adminPermission.Name = "Amministrazione";
                dbContext.Permissions.Add(adminPermission);
                dbContext.SaveChanges();
            }

            var defaultPermission = dbContext.Permissions.FirstOrDefault(p => p.Id == kDefaultRoleCode);
            if (defaultPermission == null)
            {
                defaultPermission = new Permission();
                defaultPermission.Enabled = true;
                defaultPermission.Id = kDefaultRoleCode;
                defaultPermission.Name = "Default";
                dbContext.Permissions.Add(defaultPermission);
                dbContext.SaveChanges();
            }


            var adminRole = dbContext.Roles.FirstOrDefault(r => r.Name == kAdminRoleCode);
            if (adminRole == null)
            {
                adminRole = new Role();
                adminRole.Name = kAdminRoleCode;
                adminRole.Description = "Amministratore";
                adminRole.Permissions = new List<Permission>();
                adminRole.Permissions.Add(defaultPermission);
                adminRole.Permissions.Add(adminPermission);
                dbContext.Roles.Add(adminRole);
                dbContext.SaveChanges();
            }

            var userRole = dbContext.Roles.FirstOrDefault(r => r.Name == kUserRoleCode);
            if (userRole == null)
            {
                userRole = new Role();
                userRole.Name = kUserRoleCode;
                userRole.Description = "Utente";

                userRole.Permissions = new List<Permission>();
                userRole.Permissions.Add(defaultPermission);
                dbContext.Roles.Add(userRole);
                dbContext.SaveChanges();
            }

            // recupero tutti gli utenti
            // Apro la tabella delle commesse in access
            using (var readUsersCommand = accessConnection.CreateCommand())
            {
                readUsersCommand.CommandText = "SELECT Operatore, descrizione FROM operatori";
                using (var usersReader = readUsersCommand.ExecuteReader())
                {
                    while (usersReader.Read())
                    {
                        if (!usersReader.IsDBNull(0))
                        {
                            var op = usersReader.GetString(0);

                            if (!string.IsNullOrWhiteSpace(op))
                            {
                                var name = !usersReader.IsDBNull(1) ? usersReader.GetString(1) : string.Empty;
                                var user = dbContext.Users.FirstOrDefault(c => c.Nickname == op);
                                if (user == null)
                                {
                                    user = new User();
                                    user.Nickname = op;
                                    user.FirstName = name;
                                    user.LastName = string.Empty;
                                    user.Roles = new List<Role>();
                                    if (name.ToUpper().Contains("GIORDANO"))
                                    {
                                        user.Roles.Add(adminRole);
                                    }
                                    else
                                    {
                                        user.Roles.Add(userRole);
                                    }

                                    dbContext.Users.Add(user);
                                    dbContext.SaveChanges();
                                }
                            }

                        }
                    }
                }

                readUsersCommand.CommandText = "SELECT distinct Operatore FROM Registro_Ore";
                using (var usersReader = readUsersCommand.ExecuteReader())
                {
                    while (usersReader.Read())
                    {
                        if (!usersReader.IsDBNull(0))
                        {
                            var op = usersReader.GetString(0);

                            if (!string.IsNullOrWhiteSpace(op))
                            {
                                var user = dbContext.Users.FirstOrDefault(c => c.Nickname == op);
                                if (user == null)
                                {
                                    user = dbContext.Users.FirstOrDefault(c => c.Nickname == op.Replace(" ", "_"));
                                }

                                if (user == null)
                                {
                                    user = new User();
                                    user.Nickname = op.Replace(" ", "_");
                                    user.FirstName = op;
                                    user.LastName = string.Empty;
                                    // Aggiungo senza ruoli perchè non attivii

                                    dbContext.Users.Add(user);
                                    dbContext.SaveChanges();
                                }
                            }

                        }
                    }
                }

            }
            LogDebug("Caricamento utenti completato");
        }

        private static void ReadCustomers()
        {
            LogDebug("Caricamento clienti");
            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readCustomersCommand = accessConnection.CreateCommand())
            {
                readCustomersCommand.CommandText = "SELECT distinct [cod-cliente], cliente FROM codici_commesse";
                using (var jobsReader = readCustomersCommand.ExecuteReader())
                {
                    while (jobsReader.Read())
                    {
                        if (!jobsReader.IsDBNull(0))
                        {
                            var code = jobsReader.GetString(0).ToUpper();

                            if (!string.IsNullOrWhiteSpace(code))
                            {
                                var description = !jobsReader.IsDBNull(1) && !string.IsNullOrWhiteSpace(jobsReader.GetString(1)) ? jobsReader.GetString(1) : kNotFoundDescription;
                                var customer = dbContext.Customers.FirstOrDefault(c => c.Code == code);
                                if (customer == null)
                                {
                                    customer = new Customer();
                                    customer.Code = code;
                                    customer.Description = description;
                                    customer.InsertDate = DateTime.Now;
                                    dbContext.Customers.Add(customer);
                                    dbContext.SaveChanges();
                                    LogDebug($"Aggiunto {customer.Code}");
                                }
                                else
                                {
                                    LogDebug($"Cliente {customer.Code} esistente");
                                    if (!kNotFoundDescription.Equals(description) &&
                                         kNotFoundDescription.Equals(customer.Description))
                                    {
                                        LogDebug($"Aggiornata descrizione {customer.Code}");
                                        customer.Description = description;
                                        dbContext.Customers.Update(customer);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }

                        }
                    }
                }

            }
            LogDebug("Caricamento clienti completato");
        }

        private static void ReadJobsCustomers()
        {
            LogDebug("Associazione Commesse-Clienti");


            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readJobsCommand = accessConnection.CreateCommand())
            {
                readJobsCommand.CommandText = "SELECT distinct commessa, [cod-cliente],ID, CommCliente, descrizione FROM codici_commesse where [sub-cod] is null";
                using (var jobsReader = readJobsCommand.ExecuteReader())
                {
                    while (jobsReader.Read())
                    {
                        if (!jobsReader.IsDBNull(0))
                        {
                            var jobCode = jobsReader.GetString(0).ToUpper();

                            var commCliente = !jobsReader.IsDBNull(3) ? jobsReader.GetString(3) : string.Empty;

                            if (!string.IsNullOrWhiteSpace(jobCode))
                            {
                                var job = dbContext.Jobs.Include(c => c.Customer).FirstOrDefault(c => c.Code.ToLower() == jobCode.ToLower());
                                if (job == null)
                                {
                                    job = new Job();
                                    job.InsertDate = DateTime.MinValue;
                                    job.Code = jobCode.ToLower();
                                    job.Description = !jobsReader.IsDBNull(4) ? jobsReader.GetString(4) : string.Empty;
                                    job.UpdateDate = DateTime.Now;
                                    dbContext.Jobs.Add(job);
                                    dbContext.SaveChanges();
                                }


                                if (job != null && !jobsReader.IsDBNull(1))
                                {
                                    var customerCode = jobsReader.GetString(1).ToUpper();
                                    if (job.Customer == null)
                                    {
                                        job.Customer = dbContext.Customers.FirstOrDefault(c => c.Code == customerCode.ToUpper());
                                        if (job.Customer == null)
                                        {
                                            LogWarning($"Commessa con id {jobsReader.GetInt32(2)} cliente con codice {customerCode} non trovato su sql server");
                                        }
                                        else
                                        {
                                            dbContext.Update(job);
                                            dbContext.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        LogDebug($"Commessa {job.Code} con cliente già assengato");
                                    }
                                }
                                else
                                {
                                    LogWarning($"Commessa {jobCode} non trovata");
                                }
                            }

                        }
                        else
                        {
                            LogWarning($"Commessa con id {jobsReader.GetInt32(2)} senza cliente o commessa");
                        }
                    }
                }

            }
            LogDebug("Associazione Commesse-Clienti completata");
        }

        private static void ReadSubJobs()
        {
            LogDebug("Lettura sotto commesse");
            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readJobsCommand = accessConnection.CreateCommand())
            {
                readJobsCommand.CommandText = "SELECT distinct commessa, [sub-cod], descrizione, [cod-cliente], ID FROM codici_commesse where [sub-cod] is not null order by commessa, [sub-cod]";
                using (var jobsReader = readJobsCommand.ExecuteReader())
                {
                    while (jobsReader.Read())
                    {
                        if (!jobsReader.IsDBNull(0) && !jobsReader.IsDBNull(1))
                        {
                            var jobCode = jobsReader.GetString(0).ToLower();
                            var subJobCode = jobsReader.GetInt32(1).ToString().ToLower();
                            var description = !jobsReader.IsDBNull(2) ? jobsReader.GetString(2) : kNotFoundDescription;
                            if (!string.IsNullOrWhiteSpace(jobCode))
                            {
                                var job = dbContext.Jobs.Include(j => j.SubJobs).Include(c => c.Customer).FirstOrDefault(c => c.Code.ToLower() == jobCode);

                                if (job == null)
                                {
                                    job = new Job();
                                    job.InsertDate = DateTime.MinValue;
                                    job.Code = jobCode;
                                    job.Description = description;
                                    job.UpdateDate = DateTime.Now;
                                    job.SubJobs = new List<Job>();
                                    if (!jobsReader.IsDBNull(3))
                                    {
                                        if (job.Customer == null)
                                        {
                                            job.Customer = dbContext.Customers.FirstOrDefault(c => c.Code == jobsReader.GetString(3).ToUpper());
                                        }
                                    }

                                    dbContext.Jobs.Add(job);
                                    dbContext.SaveChanges();
                                    LogDebug($"Aggiunta commessa {jobCode} da subcommessa {subJobCode}");
                                }

                                if (job != null)
                                {

                                    var subJob = job.SubJobs.FirstOrDefault(sj => sj.Code == subJobCode);
                                    if (subJob == null)
                                    {
                                        subJob = new Job();
                                        subJob.Code = subJobCode;
                                        subJob.Description = description;
                                        subJob.InsertDate = DateTime.Now;
                                        job.SubJobs.Add(subJob);
                                        dbContext.Jobs.Update(job);
                                        dbContext.SaveChanges();
                                        LogDebug($"Aggiunta sottocommessa {subJobCode} per commessa {jobCode}");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(subJob.Description) || (subJob.Description == kNotFoundDescription && description != kNotFoundDescription))
                                        {
                                            subJob.Description = description;
                                            dbContext.Jobs.Update(subJob);
                                            dbContext.SaveChanges();
                                            LogDebug($"Aggiornata sotto commessa {subJobCode} per commessa {jobCode}");
                                        }

                                        if (!jobsReader.IsDBNull(3))
                                        {
                                            if (subJob.Customer == null)
                                            {
                                                subJob.Customer = dbContext.Customers.FirstOrDefault(c => c.Code == jobsReader.GetString(3).ToUpper());
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    LogWarning($"Commessa {jobCode} non trovata");
                                }
                            }
                        }
                        else
                        {
                            LogWarning($"Commessa con id {jobsReader.GetInt32(4)} senza commessa o sotto commessa");
                        }
                    }
                }

            }
            LogDebug("Lettura sotto commesse completata");
        }


        private static void ReadWorkLogs()
        {
            LogDebug("Lettura registrazioni ore");
            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readWorksCommand = accessConnection.CreateCommand())
            {
                dbContext.DayWorkLogs.RemoveRange(dbContext.DayWorkLogs.Where(d => d.InsertDate == DateTime.MinValue));
                readWorksCommand.CommandText = "SELECT distinct ID, commessa, [sub_comm], operatore, data, Ore, tipologia FROM registro_ore order by ID";
                using (var worksReader = readWorksCommand.ExecuteReader())
                {
                    var count = 1;
                    while (worksReader.Read())
                    {
                        
                        if (count % 100 == 0)
                        {
                            LogDebug($"'Registrazione {count}");
                        }
                        count++;

                        var id = worksReader.GetInt32(0);
                        if (!worksReader.IsDBNull(1) &&
                            !worksReader.IsDBNull(3) &&
                            !worksReader.IsDBNull(4) &&
                            !worksReader.IsDBNull(5) &&
                            !worksReader.IsDBNull(6))
                        {
                            // Cerco la commessa
                            var job = dbContext.Jobs.Include(j => j.SubJobs).FirstOrDefault(j => j.Code == worksReader.GetString(1).ToLower());
                            if (job == null)
                            {
                                LogWarning($"Commessa registrazione con id {id} commessa {worksReader.GetString(1)} non trovata");
                            }
                            var op = dbContext.Users.FirstOrDefault(o => o.Nickname == worksReader.GetString(3));
                            if (op == null)
                            {
                                op = dbContext.Users.FirstOrDefault(o => o.Nickname == worksReader.GetString(3).Replace(" ", "_"));
                                if (op == null)
                                {
                                    LogWarning($"Operatore registrazione con id {id} utente {worksReader.GetString(3)} non trovato");
                                }
                            }
                            var dbType = dbContext.DayWorkTypes.FirstOrDefault(t => t.Code == worksReader.GetString(6));
                            if (dbType == null)
                            {
                                //Se non trovato lo aggiungo
                                dbType = new DayWorkType();
                                dbType.Code = worksReader.GetString(6);
                                dbType.Description = dbType.Code;

                                dbContext.DayWorkTypes.Add(dbType);
                                dbContext.SaveChanges();
                                LogWarning($"Aggiunta tipologia {dbType.Code}");
                            }


                            Job subJob = null;
                            if (job != null && !worksReader.IsDBNull(2))
                            {
                                var subJobCode = worksReader.GetString(2);
                                if (!string.IsNullOrWhiteSpace(subJobCode))
                                {
                                    subJob = job.SubJobs.FirstOrDefault(t => t.Code == subJobCode);
                                    if (subJob == null)
                                    {
                                        var numericSubJobCode = 0;
                                        if (int.TryParse(subJobCode, out numericSubJobCode))
                                        {
                                            subJob = job.SubJobs.FirstOrDefault(t => t.Code == numericSubJobCode.ToString());
                                            if (subJob == null && numericSubJobCode > 0)
                                            {
                                                LogWarning($"Operatore registrazione con id {worksReader.GetInt32(0)} subjob {subJobCode} non trovato");
                                            }
                                        }

                                    }

                                }
                            }


                            if (job != null && dbType != null && op != null)
                            {
                                var dayWorkLog = new DayWorkLog()
                                {
                                    Date = worksReader.GetDateTime(4).Date,
                                    InsertDate = DateTime.MinValue,
                                    Job = subJob ?? job,
                                    Minutes = GetMinutes(worksReader.GetFloat(5)),
                                    Note = string.Empty,
                                    User = op,
                                    WorkType = dbType,
                                    InsertUser = op,
                                    UpdateUser = op,
                                    UpdateDate = DateTime.Now,
                                };
                                if (dayWorkLog.Minutes > 0)
                                {
                                    dbContext.DayWorkLogs.Add(dayWorkLog);
                                    dbContext.SaveChanges();
                                    LogDebug($"Registrazione con id {id} importata");
                                }

                            }
                            else
                            {
                                LogError($"Registrazione con id {id} scartata");
                            }
                        }
                        else
                        {
                            LogWarning($"Registrazione con id {id} senza dati obbligatori");
                        }
                    }
                }

            }


            LogDebug("Lettura registrazioni completata");
        }


        private static void ReadJobExtraInfo()
        {
            LogDebug("Lettura informazioni extra commessa");
            // recupero tutti i clienti
            // Apro la tabella delle commesse in access
            using (var readJobsCommand = accessConnection.CreateCommand())
            {
                readJobsCommand.CommandText = "SELECT * FROM codici_commesse";
                using (var jobsReader = readJobsCommand.ExecuteReader())
                {
                    var count = 1;
                    while (jobsReader.Read())
                    {

                        if (count % 100 == 0)
                        {
                            LogDebug($"Commessa {count}");
                        }
                        count++;
                        if (!jobsReader.IsDBNull(0) && !jobsReader.IsDBNull(jobsReader.GetOrdinal("commessa")))
                        {
                            var jobCode = jobsReader.GetString(jobsReader.GetOrdinal("commessa")).ToLower();
                            var subJobCode = string.Empty;

                            if (!jobsReader.IsDBNull(jobsReader.GetOrdinal("sub-cod")))
                            {
                                subJobCode = jobsReader.GetInt32(jobsReader.GetOrdinal("sub-cod")).ToString().ToLower();
                            }
                            Job job = null;
                            if (!string.IsNullOrWhiteSpace(jobCode))
                            {
                                job = dbContext.Jobs.Include(j => j.SubJobs).ThenInclude(sj => sj.Fields).Include(j => j.Fields).FirstOrDefault(c => c.Code.ToLower() == jobCode.ToLower());
                            }
                            if (job != null && !string.IsNullOrEmpty(subJobCode))
                            {
                                job = job.SubJobs?.FirstOrDefault(j => j.Code.ToLower() == subJobCode.ToLower());
                            }

                            if (job != null)
                            {
                                if (job.CustomerInfo == null) job.CustomerInfo = new JobCustomerInfo();
                                for (var i = 0; i < jobsReader.FieldCount; i++)
                                {
                                    var fieldName = jobsReader.GetName(i).ToLower();
                                    switch (fieldName)
                                    {
                                        case "id":

                                        case "data inizio":
                                            if (!jobsReader.IsDBNull(i) && job.BeginDate != null)
                                            {
                                                job.BeginDate = jobsReader.GetDateTime(i);
                                            }
                                            break;
                                        case "codice":
                                        case "commessa":
                                        case "cod-cliente":
                                        case "sub-cod":

                                        case "descrizione":
                                        case "tipologia":
                                        case "cliente":
                                            break;
                                        case "commcliente":
                                            job.CustomerInfo.Info = string.IsNullOrEmpty(job.CustomerInfo.Info) && !jobsReader.IsDBNull(i) ? jobsReader.GetString(i) : string.Empty;
                                            break;
                                        case "preventivo n°":
                                            job.CustomerInfo.QuotationRef = string.IsNullOrEmpty(job.CustomerInfo.QuotationRef) && !jobsReader.IsDBNull(i) ? jobsReader.GetString(i) : string.Empty;
                                            break;
                                        case "ordine n°":
                                            job.CustomerInfo.OrderRef = string.IsNullOrEmpty(job.CustomerInfo.OrderRef) && !jobsReader.IsDBNull(i) ? jobsReader.GetString(i) : string.Empty;
                                            break;
                                        default:
                                            var extraField = dbContext.ExtraFields.FirstOrDefault(ef => ef.Label.ToLower() == fieldName.ToLower());
                                            if (extraField == null)
                                            {
                                                extraField = new ExtraField()
                                                {
                                                    Description = fieldName,
                                                    Label = Capitalize(fieldName),
                                                    Type = GetExtraFieldType(jobsReader.GetFieldType(i)),
                                                    ValidationExpression = string.Empty
                                                };
                                                dbContext.ExtraFields.Add(extraField);
                                                dbContext.SaveChanges();
                                            }

                                            if (!jobsReader.IsDBNull(i))
                                            {
                                                var extraFieldValue = job.Fields.FirstOrDefault(f => f.ExtraField.Description == fieldName);

                                                if (extraFieldValue == null)
                                                {
                                                    extraFieldValue = new JobExtraFieldValue()
                                                    {
                                                        Job = job,
                                                        ExtraField = extraField,
                                                        InsertDate = DateTime.Now
                                                    };

                                                    switch (extraField.Type)
                                                    {
                                                        case ExtraFieldType.Bool:
                                                            var boolData = jobsReader.GetBoolean(i);
                                                            extraFieldValue.NumberValue = boolData ? 1 : 0;
                                                            extraFieldValue.TextValue = boolData ? "SI" : "NO";
                                                            break;
                                                        case ExtraFieldType.Date:
                                                            extraFieldValue.DateTimeValue = jobsReader.GetDateTime(i);
                                                            extraFieldValue.TextValue = extraFieldValue.DateTimeValue.HasValue ? extraFieldValue.DateTimeValue.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                                                            extraFieldValue.NumberValue = extraFieldValue.DateTimeValue.HasValue ? extraFieldValue.DateTimeValue.Value.Ticks : 0;
                                                            break;
                                                        case ExtraFieldType.Numeric:
                                                            try
                                                            {
                                                                extraFieldValue.NumberValue = jobsReader.GetInt32(i);
                                                            }
                                                            catch
                                                            {
                                                                extraFieldValue.NumberValue = jobsReader.GetDouble(i);
                                                            }
                                                            extraFieldValue.TextValue = extraFieldValue.NumberValue.ToString();
                                                            break;
                                                        default:
                                                            extraFieldValue.TextValue = jobsReader.GetString(i);
                                                            break;

                                                    }
                                                    dbContext.JobExtraFields.Add(extraFieldValue);
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                            break;
                                    }
                                }
                                dbContext.SaveChanges();
                            }
                        }
                    }

                }
                LogDebug("Lettura sotto commesse completata");
            }
        }

        public static string Capitalize(string text)
        {

            switch ((text ?? string.Empty).Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return text.ToUpper();
                default:
                    return $"{text[0].ToString().ToUpper()}{text.Substring(1)}";
            }

        }
        private static ExtraFieldType GetExtraFieldType(Type type)
        {
            if (type == typeof(DateTime))
            {
                return ExtraFieldType.Date;
            }
            else if (type == typeof(bool))
            {
                return ExtraFieldType.Bool;
            }
            else if (type == typeof(bool))
            {
                return ExtraFieldType.Bool;
            }
            else if (type == typeof(int) || type == typeof(double) || type == typeof(decimal))
            {
                return ExtraFieldType.Numeric;
            }

            return ExtraFieldType.Text;
        }

        static int GetMinutes(double value)
        {
            var hours = Math.Truncate(value);
            var minutes = 60 * (value - hours);
            return Convert.ToInt32(hours * 60 + minutes);
        }


        static void LogError(string message, Exception ex = null)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(message);
            logger?.Error(ex, message);
        }

        static void LogDebug(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(message);
            logger?.Debug(message);
        }

        static void LogWarning(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(message);
            logger?.Warning(message);
        }
    }
}
