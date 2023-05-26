﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Tests.Common.BaseTest;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.Controllers;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using System;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests
{
    public abstract class TrainingProviderControllerTestBase : BaseTest<TrainingProviderController>
    {
        // Dependencies
        protected ITrainingProviderLoader TrainingProviderLoader;
        protected ICacheService CacheService;
        protected ResultsAndCertificationConfiguration ResultsAndCertificationConfiguration;
        protected ILogger<TrainingProviderController> Logger;
        protected TrainingProviderController Controller;

        // HttpContext
        protected int ProviderUkprn;
        protected Guid UserId;
        protected IHttpContextAccessor HttpContextAccessor;
        protected string CacheKey;
        protected string InformationCacheKey;

        public override void Setup()
        {
            TrainingProviderLoader = Substitute.For<ITrainingProviderLoader>();
            CacheService = Substitute.For<ICacheService>();
            ResultsAndCertificationConfiguration = new ResultsAndCertificationConfiguration { DocumentRerequestInDays = 21 };
            Logger = Substitute.For<ILogger<TrainingProviderController>>();
            Controller = new TrainingProviderController(TrainingProviderLoader, CacheService, ResultsAndCertificationConfiguration, Logger);

            ProviderUkprn = 1234567890;
            var httpContext = new ClaimsIdentityBuilder<TrainingProviderController>(Controller)
               .Add(CustomClaimTypes.Ukprn, ProviderUkprn.ToString())
               .Add(CustomClaimTypes.UserId, Guid.NewGuid().ToString())
               .Build()
               .HttpContext;

            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            HttpContextAccessor.HttpContext.Returns(httpContext);

            CacheKey = CacheKeyHelper.GetCacheKey(httpContext.User.GetUserId(), CacheConstants.TrainingProviderCacheKey);
            InformationCacheKey = CacheKeyHelper.GetCacheKey(httpContext.User.GetUserId(), CacheConstants.TrainingProviderInformationCacheKey);
        }
    }
}
