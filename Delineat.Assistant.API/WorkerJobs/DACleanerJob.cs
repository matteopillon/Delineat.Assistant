using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Delineat.Assistant.API.WorkerJobs
{
    public class DACleanerJob : IDAWorkerJob
    {
        private readonly int days;
        private readonly string folder;
        private readonly ILogger logger;

        public DACleanerJob(string folder, int days, ILoggerFactory loggerFactory)
        {
            this.folder = folder;
            this.days = days;
            if (loggerFactory != null)
            {
                this.logger = loggerFactory.CreateLogger<DACleanerJob>();
            }
        }

        public bool Execute()
        {
            if (Directory.Exists(folder))
            {
                foreach (var subDir in Directory.GetDirectories(folder))
                {
                    var subDirInfo = new DirectoryInfo(subDir);
                    if (DateTime.Now - subDirInfo.CreationTime > new TimeSpan(days, 0, 0, 0))
                    {
                        try
                        {
                            subDirInfo.Delete(true);
                        }
                        catch (Exception ex)
                        {
                            if (logger != null)
                                logger.LogCritical(DAConsts.Logs.LoggerExceptionEventId, ex, $"Impossibile elinare la cartella {subDir}");
                        }
                    }
                }
            }
            return true;
        }
    }
}
