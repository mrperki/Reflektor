using Reflektor.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Reflektor
{
    public interface IJobProvider
    {
        List<Job> Jobs { get; }
        Job GetReadyJob();
    }

    public class JobProvider
    {
        private const string _defaultJobsFilePath = ".\\Reflektor.Jobs.json";
        private const int _semaphoreTimeoutMs = 10000;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly CommandLineOptions _options;
        private List<Job> _jobs = new List<Job>();
        private List<TriggerProgress> _progress = new List<TriggerProgress>();

        public JobProvider() : this(new CommandLineOptions { JobsFilePath = _defaultJobsFilePath })
        {
        }

        public JobProvider(CommandLineOptions options)
        {
            _options = options;
        }

        // for testing
        internal JobProvider(List<Job> jobs)
        {
            _jobs = jobs;
        }

        public List<Job> Jobs
        {
            get
            {
                Init();
                return _jobs;
            }            
        }

        public TriggerProgress GetReadyJob()
        {
            Init();

            try
            {
                _semaphore.Wait(_semaphoreTimeoutMs);
                
                var readyJob = _progress.Where(p => p.CheckReadiness()).OrderBy(p => p.NextRun).FirstOrDefault();
                if (readyJob == null) return null;

                readyJob.Status = TriggerStatus.Running;
                return readyJob;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void Init()
        {
            if (_jobs?.Any() ?? false) return;
            //todo: get config file and populate _jobs
        }
    }
}
