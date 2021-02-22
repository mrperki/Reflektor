using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reflektor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reflektor
{
    public class ReflektorService : BackgroundService
    {
        private readonly ConfigProvider _configProvider;
        private readonly IJobProvider _jobProvider;
        private readonly JobWorkerFactory _factory;
        private readonly ILogger _logger;

        public ReflektorService(ConfigProvider configProvider,
            IJobProvider jobProvider,
            JobWorkerFactory factory,
            ILogger<ReflektorService> logger)
        {
            _configProvider = configProvider;
            _jobProvider = jobProvider;
            _factory = factory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Received request to stop service");
                return;
            }

            _logger.LogInformation("Service started");

            try
            {
                _configProvider.Initialize();
                _jobProvider.Initialize();

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Checking for jobs to run");
                    var tasks = new List<Task>();
                    IJobInstance readyJob = null;
                    do
                    {
                        readyJob = _jobProvider.GetNextReadyJob();
                        if (readyJob != null)
                            tasks.Add(RunJob(readyJob));
                    }
                    while (readyJob != null);
                    _logger.LogDebug($"Found {tasks.Count} jobs ready to run");

                    while (tasks.Any())
                    {
                        var task = await Task.WhenAny(tasks);
                        tasks.Remove(task);
                        await task;
                    }

                    _logger.LogDebug("All jobs have completed. Service will now rest for 5000 ms.");
                    Thread.Sleep(5000);
                }

                _logger.LogInformation("Received request to stop service");
            }
            catch (ServiceFailException e)
            {
                _logger.LogError(e, "The service has failed and will be shut down.");
                return;
            }
        }

        private async Task RunJob(IJobInstance job)
        {
            _logger.LogDebug($"About to run job {job.Job.Name}");
            var worker = _factory.CreateWorker();
            if (await worker.Work(job))
            {
                _jobProvider.MarkCompleted(job);
                _logger.LogInformation($"Job {job.Job.Name} completed successfully");
            }
            else
            {
                _jobProvider.MarkFailed(job);
                _logger.LogWarning($"Job {job.Job.Name} failed to run");
            }
        }
    }
}
