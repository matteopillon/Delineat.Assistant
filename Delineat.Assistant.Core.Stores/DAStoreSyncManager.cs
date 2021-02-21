using Delineat.Assistant.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Delineat.Assistant.Core.Stores
{
    public class DAStoreSyncManager
    {
        private ILogger logger;
        private IDAStore store;

        public DAStoreSyncManager(IDAStore store, ILogger logger)
        {
            this.store = store;
            this.logger = logger;
        }

        public bool SyncFromSource()
        {
            try
            {
                logger.LogTrace($"Inizio sync");

                foreach (var syncStore in store.SyncStores)
                {
                    logger.LogInformation($"Test sync store {syncStore.Name}");
                    //Se non inizializzato
                    if (!store.AlreadySync(syncStore))
                    {
                        logger.LogInformation($"Inizio syncronizzazione {syncStore.Name}");
                        try
                        {
                            store.BeginSync(syncStore, logger);
                            //Recupero tutte le commesse
                            var jobs = syncStore.GetJobs();
                            logger.LogInformation($"Commesse recuperate {jobs.Count}");
                            foreach (var job in jobs)
                            {
                                store.SyncJob(job);
                                logger.LogInformation($"Commessa {job} sincronizzata");
                                foreach (var item in syncStore.GetJobItems(job))
                                {
                                    store.SyncItem(job, item);
                                }
                            }
                            store.CompleteSync(syncStore);
                        }
                        catch (Exception ex)
                        {
                            logger.LogCritical(ex, "Impossibile sincronizzare");
                            store.TryRollbackSync(syncStore);
                            return false;
                        }
                    }
                    else
                    {
                        logger.LogInformation($"store già aggiornato {syncStore.Name}");
                    }
                }

                //Imposto come default il primo della lista
                store.SetDefaultSyncStore(store.SyncStores.FirstOrDefault());

                store.UpdateSyncSettings(store.SyncStores);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Impossibile sincronizzare");
                return false;
            }

        }
    }
}
