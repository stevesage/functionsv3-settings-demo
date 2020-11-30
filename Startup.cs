using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

[assembly : FunctionsStartup(typeof(Settings.Demo.Startup))]
namespace Settings.Demo
{
    internal class Startup : FunctionsStartup
    {
        private string _environmentName = "Development";
        private IConfiguration _configuration;

         public override void Configure(IFunctionsHostBuilder builder)
        {
            var executioncontextoptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;

            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IHostingEnvironment));
            if (descriptor?.ImplementationInstance is IHostingEnvironment hostingEnvironment)
            {
                _environmentName = hostingEnvironment.EnvironmentName;
            }

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder = configurationBuilder
                .SetBasePath(executioncontextoptions.AppDirectory)
                .AddJsonFile(Path.Combine("Configuration", "Data", "timerSchedules.json"))
                .AddEnvironmentVariables();

            descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfigurationRoot configuration)
            {
                configurationBuilder.AddConfiguration(configuration);
            }

            _configuration = configurationBuilder.Build();

            builder.Services.AddSingleton(_configuration);


            var serviceProvider = builder.Services.BuildServiceProvider();


        }
    }
}