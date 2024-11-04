﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Configuration;
using Sfa.Tl.ResultsAndCertification.Data;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using System;

namespace Sfa.Tl.ResultsAndCertification.Tests.Common.Configuration
{
    public class TestDatabaseConfiguration
    {
        public static ResultsAndCertificationConfiguration ResultsAndCertificationConfiguration { get; }

        static TestDatabaseConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

            if (configuration[Constants.EnvironmentNameConfigKey].Equals("__EnvironmentName__", StringComparison.InvariantCultureIgnoreCase))
            {
                configuration = new ConfigurationBuilder().AddJsonFile("appsettings.local.json").Build();
            }

            ResultsAndCertificationConfiguration = LoadConfiguration(configuration);
        }

        public static ResultsAndCertificationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ResultsAndCertificationDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .EnableSensitiveDataLogging()
           .Options;

            return new ResultsAndCertificationDbContext(options);
        }

        public static ResultsAndCertificationDbContext CreateRelationalDbContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ResultsAndCertificationDbContext>()
                .UseSqlServer(ResultsAndCertificationConfiguration.IntTestSqlConnectionString, builder =>
                    builder.EnableRetryOnFailure().UseNetTopologySuite()).Options;

            return new ResultsAndCertificationDbContext(dbContextOptions);
        }

        public static string GetConnectionString()
        {
            return ResultsAndCertificationConfiguration.IntTestSqlConnectionString;
        }

        private static ResultsAndCertificationConfiguration LoadConfiguration(IConfiguration configuration)
        {
            string tableServiceUri = configuration[Constants.TableServiceUriConfigKey];
            IConfigurationLoader configurationLoader = ConfigurationLoaderFactory.GetConfigurationLoader(tableServiceUri, isDevelopment: true);

            return configurationLoader.Load(
                 configuration[Constants.EnvironmentNameConfigKey],
                 configuration[Constants.VersionConfigKey],
                 configuration[Constants.ServiceNameConfigKey]);
        }
    }
}
