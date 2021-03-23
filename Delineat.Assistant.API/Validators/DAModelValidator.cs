using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Validators
{
    public class DAValidationResult
    {
        private readonly List<string> errors = new List<string>();

        public bool IsValid { get; set; }

        public string[] Errors => errors.ToArray();

        public void AddError(string errorMessage)
        {
            IsValid = false;
            errors.Add(errorMessage);
        }
    }
    public class DAModelValidator
    {
        private IDAStore store;

        public DAModelValidator(IDAStore store)
        {
            this.store = store;
        }

        private DAValidationResult CreateResult()
        {
            return new DAValidationResult() { IsValid = true };
        }
        public DAValidationResult Validate(DWJob job)
        {
            var result = CreateResult();
            if (job == null)
                result.AddError("Commessa non valorizzata");
            else
            {
                if (string.IsNullOrWhiteSpace(job.Code))
                {
                    result.AddError("Codice della commessa non valorizzato");
                }
                else
                {
                    if (job.Parent == null)
                    {
                        job.Code = job.Code.Trim().ToUpper();
                        //Verifico se il codice della commessa segue la regola XXXAA
                        if (job.Code.Length == 5)
                        {
                            var progressivo = 0;
                            if (!int.TryParse(job.Code.Substring(0, 3), out progressivo))
                            {
                                result.AddError("I primi tre caratteri del codice commessa devono essere numerici");
                            }
                            else
                            {
                                if (job.JobId == 0)
                                {
                                    //Verifico che non esistà già una commessa con il codice
                                    var existingJob = store.GetJobs().Where(j => j.Code.ToUpper() == job.Code).FirstOrDefault();
                                    if (existingJob != null)
                                    {
                                        result.AddError($"Esiste già la commessa {existingJob.Description} con codice {existingJob.Code}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.AddError("Il codice della commessa deve essere lungo 5 caratteri");
                        }
                    }
                    else
                    {
                        var existingJob = store.GetJobs().Where(j => j.Code.ToUpper() == job.Code && job.Parent?.JobId == job.Parent.JobId).FirstOrDefault();
                        if (existingJob != null)
                        {
                            result.AddError($"Esiste già la sotto commessa {existingJob.Description} con codice {existingJob.Code}");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(job.Description))
                {
                    result.AddError("Descrizione della commessa non valorizzata");
                }
            }



            return result;

        }

        public DAValidationResult Validate(DWTopic topic)
        {
            var result = CreateResult();

            if (topic == null)
                result.AddError("Topic non valorizzato");
            else
            {
                if (topic.JobId <= 0)
                {
                    result.AddError("La commessa deve essere valorizzata");
                }
                if (string.IsNullOrWhiteSpace(topic.Description))
                {
                    result.AddError("Descrizione non valorizzata");
                }
                else
                {
                    //Controllo che non esista già un topic con la descrizione:
                    var job = store.GetJob(topic.JobId);
                    if (job.Topics.Exists(t => topic.Description.ToUpper().Equals((t.Description ?? string.Empty).ToUpper())))
                    {
                        result.AddError($"Esiste già una cartella con descrizione {topic.Description}");
                    }
                }
            }

            return result;
        }

        public DAValidationResult Validate(DWTag tag)
        {
            var result = CreateResult();
            if (string.IsNullOrWhiteSpace(tag.Description))
            {
                result.AddError("Descrizione non valorizzata");
            }
            else
            {
                tag.Description = tag.Description.ToUpper().Trim();
            }

            return result;
        }

        public DAValidationResult Validate(DWWorkLog log)
        {
            var result = CreateResult();

            return result;
        }
    }
}
