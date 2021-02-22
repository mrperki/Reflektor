using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Reflektor.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reflektor
{
    public class ConfigProvider
    {
        private readonly string _configFilePath;
        private readonly Func<IConfigurationRoot> _configRootFactory;
        private readonly ILogger _logger;
        private IConfigurationRoot _configRoot;
        private DateTime? _lastAccessed;
        private DateTime _lastUpdated;

        internal static IConfigurationRoot GetConfigurationRootFromFile(string configFilePath)
            => new ConfigurationBuilder()
                .AddJsonFile(configFilePath, false, true)
                .Build();

        public ConfigProvider(CommandLineOptions options, ILogger<ConfigProvider> logger) : this(options.JobsFilePath, logger)
        {
        }

        public ConfigProvider(string configFilePath, ILogger<ConfigProvider> logger)
            : this(() => GetConfigurationRootFromFile(configFilePath), logger)
        {
            _configFilePath = configFilePath;
        }

        internal ConfigProvider(Func<IConfigurationRoot> configRootFactory, ILogger logger)
        {
            _configRootFactory = configRootFactory;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogDebug("Loading config ...");

            try
            {
                _configRoot = _configRootFactory.Invoke();
            }
            catch (FileNotFoundException e)
            {
                _logger.LogError($"Config file {_configFilePath} was not found. Details: {e.Message}");
            }

            ChangeToken.OnChange(() => _configRoot.GetReloadToken(), () =>
            {
                _logger.LogInformation("Jobs config has been updated");
                _lastUpdated = DateTime.Now;
            });
            _logger.LogInformation("Configuration loaded");
        }

        public List<Job> Jobs
        {
            get
            {
                _lastAccessed = DateTime.Now;
                return _configRoot.GetSection("Jobs").Get<List<Job>>();
            }
        }

        public bool HasUpdated => _lastUpdated > _lastAccessed;
    }
}
