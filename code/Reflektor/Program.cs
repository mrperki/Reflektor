using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reflektor.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Reflektor
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (options) =>
                {
                    await CreateHostBuilder(args, options).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1));
        }

        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineOptions options)
            => Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddEventLog(settings =>
                    {
                        settings.LogName = Assembly.GetExecutingAssembly().GetName().Name;
                        settings.SourceName = Assembly.GetExecutingAssembly().GetName().Name;
                    });
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(options);
                    services.AddSingleton<ConfigProvider>();
                    services.AddSingleton<IJobProvider, JobProvider>();
                    services.AddSingleton<JobWorkerFactory>();
                    services.AddTransient<IJobWorker, JobWorker>();
                    services.AddHostedService<ReflektorService>();
                }).UseWindowsService();
    }
}
