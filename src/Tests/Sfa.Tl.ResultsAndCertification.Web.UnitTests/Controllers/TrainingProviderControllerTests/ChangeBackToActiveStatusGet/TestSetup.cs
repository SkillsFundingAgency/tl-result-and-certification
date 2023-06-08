﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.ChangeBackToActiveStatusGet
{
    public abstract class TestSetup: TrainingProviderControllerTestBase
    {
        public int ProfileId { get; set; }
        public IActionResult Result { get; private set; }

        public async override Task When()
        {
            Result = await Controller.ChangeBackToActiveStatusAsync(ProfileId);
        }
    }
}