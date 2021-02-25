using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Managers;
using Delineat.Assistant.API.WorkerJobs;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Tips.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API
{
    public class Program
    {
        private static DAWorkersService jobService;
        public static string[] programArgs;
        public static void Main(string[] args)
        {
            programArgs = args ?? new string[0];

            var host = BuildWebHost(args);


            //Init stores
            InitStores(host.Services);

            InitEmails(host.Services);

            StartJobManager(host.Services);


            host.Run();

            //if (IsRunAsService(args))
            //{
            //    host.RunAsDWService();
            //}
            //else
            //{

            //}
        }

        public static IConfiguration CreateConfiguration(string[] args) => new ConfigurationBuilder()
            .SetBasePath(GetBaseDirectory())
            .AddJsonFile("hosting.json", optional: true)
            .AddCommandLine(args)
            .Build();

        public static IHostBuilder CreateHostBuilder(string[] args) =>


            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(ConfigConfiguration)
                              .UseContentRoot(Directory.GetCurrentDirectory())
                              .ConfigureLogging(ConfigureLogging)
                              .UseStartup<Startup>()

                                .ConfigureKestrel(options =>
                                {
                                    options.Limits.MaxRequestBodySize = null;
                                    options.Limits.MaxRequestBufferSize = null;
                                });
                });

        public static IHost BuildWebHost(string[] args)
        {



            return CreateHostBuilder(args).Build();
            /*
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })*/

        }





        private static string GetBaseDirectory()
        {

            var directoryPath = Directory.GetCurrentDirectory();
            /*
            if (IsRunAsService(programArgs))
            {
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                directoryPath = Path.GetDirectoryName(exePath);
            }*/
            return directoryPath;
        }

        private static void ConfigureLogging(WebHostBuilderContext ctx, ILoggingBuilder obj)
        {
            obj.AddConsole();
            Trace.AutoFlush = true;
            var logConfig = ctx.Configuration.Get<DALogConfiguration>();
            if (logConfig != null)
            {
                if (logConfig != null && !string.IsNullOrWhiteSpace(logConfig.TracePath))
                {
                    var loggerSwitch = new SourceSwitch("sourceSwitch", "Logging");
                    loggerSwitch.Level = SourceLevels.Verbose;
                    obj.AddTraceSource(loggerSwitch,
                        new System.Diagnostics.TextWriterTraceListener(logConfig.TracePath));
                }
            }

        }

        static void ConfigConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config)
        {

            var env = ctx.HostingEnvironment;
#if DEBUG
            System.Console.WriteLine(env.ContentRootPath);
#endif
            config.SetBasePath(GetBaseDirectory())
                .AddJsonFile("hosting.json", optional: false)
                  .AddJsonFile($"hosting.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("stores.json", optional: false, reloadOnChange: false)
                  .AddJsonFile($"stores.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("logs.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"logs.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("client.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"client.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("jobs.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"jobs.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("email.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"email.{env.EnvironmentName}.json", optional: true)
                  .AddJsonFile("server.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"server.{env.EnvironmentName}.json", optional: true);




        }

        private static void InitEmails(IServiceProvider services)
        {

            var emailOptions = services.GetService(typeof(IOptions<DAEmailConfiguration>)) as IOptions<DAEmailConfiguration>;


            var loggerFactory = services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory.CreateLogger<Program>();

            if (emailOptions.Value.InternalEmails != null)
            {
                DWEmailHelper.InternalEmails = new List<string>(emailOptions.Value.InternalEmails);
                logger.LogInformation(emailOptions.Value.InternalEmails.Length > 0 ? "Email interne:" : "Nessun dominio di email interna");
                foreach (var email in DWEmailHelper.InternalEmails)
                {
                    logger.LogInformation(email);
                }
            }
            else
            {
                logger.LogInformation("Configurazione email interne non trovata");
            }



        }

        private static void InitStores(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
               
                var storesOptions = scope.ServiceProvider.GetService<IOptions<DAStoresConfiguration>>();
                var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                var store = scope.ServiceProvider.GetService<IDAStore>();
                if (storesOptions != null && loggerFactory != null)
                {

                    new DAStoresManager(loggerFactory, storesOptions, store).InitStores();
                }
            }

        }

        private static void StartJobManager(IServiceProvider services)
        {
            DAJobsConfiguration jobsConfiguration = null;
            using (var scope = services.CreateScope())
            {

                var jobsOptions = scope.ServiceProvider.GetService<IOptions<DAJobsConfiguration>>();
                if (jobsOptions != null)
                    jobsConfiguration = jobsOptions.Value;

                if (jobsConfiguration != null && !jobsConfiguration.Disabled)
                {
                    ILoggerFactory loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                    double interval = 10000;
                    if (jobsConfiguration != null)
                        interval = jobsConfiguration.Interval;



                    jobService = new DAWorkersService(loggerFactory, interval);

                    jobService.AddWorker(GetEmailNotifierWorker(services, loggerFactory));

                    jobService.AddWorker(GetCleanerWorker(services, jobsConfiguration, loggerFactory));

                    jobService.Start();
                }
            }
        }

        private static IDAWorkerJob GetCleanerWorker(IServiceProvider services, DAJobsConfiguration jobsConfiguration, ILoggerFactory loggerFactory)
        {
            using (var scope = services.CreateScope())
            {

                string sessionsPath = string.Empty;
                var serverConfiguration = scope.ServiceProvider.GetService<IOptions<DAServerConfiguration>>();

                if (serverConfiguration != null)
                    sessionsPath = serverConfiguration.Value.SessionsPath;

                return new DACleanerJob(sessionsPath, jobsConfiguration.CleanDays, loggerFactory);
            }
        }

        private static IDAWorkerJob GetEmailNotifierWorker(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            using (var scope = services.CreateScope())
            {

                var storesConfiguration = scope.ServiceProvider.GetService<IOptions<DAStoresConfiguration>>();
                var emailConfiguration = scope.ServiceProvider.GetService<IOptions<DAEmailConfiguration>>();
                var store = scope.ServiceProvider.GetService(typeof(IDAStore)) as IDAStore;
                return new DANoteNotifierJob(storesConfiguration, emailConfiguration, loggerFactory, store);
            }
        }

    }
}
