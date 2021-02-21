using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Delineat.Assistant.API.WorkerJobs
{

    public class DAWorkersService : IDAWorkersService
    {
        public DAWorkersService(ILoggerFactory loggerFactory, double interval)
        {
            if (loggerFactory != null)
                this.logger = loggerFactory.CreateLogger<DAWorkersService>();
            this.interval = interval;
        }

        private List<IDAWorkerJob> workers = new List<IDAWorkerJob>();
        private readonly ILogger logger;
        private readonly double interval;
        private Timer timer = null;
        public void AddWorker(IDAWorkerJob worker)
        {
            workers.Add(worker);
        }

        public void Start()
        {
            if (workers.Count > 0 && interval > 0)
            {
                timer = new Timer(interval);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var worker in workers)
            {
                ExecuteWorker(worker);
            }
        }

        private  void ExecuteWorker(IDAWorkerJob worker)
        {
            try
            {
                worker.Execute();
            }
            catch (Exception ex)
            {
                if (logger != null) logger.LogCritical(DAConsts.Logs.LoggerExceptionEventId, ex, "Impossibile eseguire worker {0}", worker.GetType().FullName);
            }
        }
    }
}
