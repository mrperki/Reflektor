using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reflektor.Models;
using System;

namespace Reflektor
{
    public class JobWorkerFactory
    {
        private readonly Func<IJobWorker> _factoryFunc;

        public JobWorkerFactory(IServiceProvider services)
            : this(() => services.GetService<IJobWorker>())
        {
        }

        internal JobWorkerFactory(Func<IJobWorker> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public IJobWorker CreateWorker() => _factoryFunc.Invoke();
    }
}
