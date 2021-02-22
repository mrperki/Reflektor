using Microsoft.Extensions.Logging;
using Reflektor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflektor
{
    public interface IJobWorker
    {
        Task<bool> Work(IJobInstance job);
    }

    public class JobWorker : IJobWorker
    {
        private readonly ILogger _logger;

        public JobWorker(ILogger<JobWorker> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Work(IJobInstance job)
        {
            return true;
        }
    }
}
