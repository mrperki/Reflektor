using Reflektor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Reflektor
{
    public interface IJobProvider
    {
        void Initialize();
        IJobInstance GetNextReadyJob();
        void MarkCompleted(IJobInstance jobInstance);
        void MarkFailed(IJobInstance jobInstance);
    }

    public class JobProvider : IJobProvider
    {
        private const int _semaphoreTimeoutMs = 10000;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly ConfigProvider _configProvider;
        private List<TriggerProgress> _progress = new List<TriggerProgress>();

        public JobProvider(ConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public void Initialize()
        {
            _progress = _configProvider.Jobs.GetAllInitialProgress();
        }

        public bool HasJobs => _configProvider.Jobs.Any();

        public IJobInstance GetNextReadyJob()
        {
            try
            {
                _semaphore.Wait(_semaphoreTimeoutMs);
                
                var readyJob = _progress.Where(p => p.CheckReadiness()).OrderBy(p => p.NextRun).FirstOrDefault();
                if (readyJob == null) return null;

                readyJob.Status = TriggerStatus.Running;
                readyJob.InstanceId = Guid.NewGuid();
                return readyJob;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void MarkCompleted(IJobInstance jobInstance)
        {
            try
            {
                _semaphore.Wait(_semaphoreTimeoutMs);

                var completedJob = _progress.GetForInstance(jobInstance);
                completedJob.CompleteAndQueueNext();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void MarkFailed(IJobInstance jobInstance)
        {
            try
            {
                _semaphore.Wait(_semaphoreTimeoutMs);

                var failedJob = _progress.GetForInstance(jobInstance);
                failedJob.Fail();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
