﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.UpdateLearnerRecord
{
    public abstract class TestSetup : TrainingProviderControllerTestBase
    {
        public IActionResult Result { get; private set; }

        public async override Task When()
        {
            Result = await Controller.UpdateLearnerRecordAsync();
        }
    }
}