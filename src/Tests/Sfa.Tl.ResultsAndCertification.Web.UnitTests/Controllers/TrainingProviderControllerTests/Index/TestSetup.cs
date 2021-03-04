﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Tests.Common.BaseTest;
using Sfa.Tl.ResultsAndCertification.Web.Controllers;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.Index
{
    public abstract class TestSetup : BaseTest<TrainingProviderController>
    {
        protected ITrainingProviderLoader TrainingProviderLoader;
        protected ILogger<TrainingProviderController> Logger;
        protected TrainingProviderController Controller;
        public IActionResult Result { get; private set; }

        public override void Setup()
        {
            TrainingProviderLoader = Substitute.For<ITrainingProviderLoader>();
            Logger = Substitute.For<ILogger<TrainingProviderController>>();
            Controller = new TrainingProviderController(TrainingProviderLoader);
        }

        public override Task When()
        {
            Result = Controller.Index();
            return Task.CompletedTask;
        }
    }
}
