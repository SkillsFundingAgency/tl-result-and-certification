﻿using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.LearnerRecordDetailsGet
{
    public abstract class TestSetup : TrainingProviderControllerTestBase
    {
        public int ProfileId { get; set; }

        public IActionResult Result { get; private set; }

        public LearnerRecordDetailsViewModel LearnerRecordDetailsViewModel;

        protected LearnerRecordDetailsViewModel Mockresult = null;

        public async override Task When()
        {
            Result = await Controller.LearnerRecordDetailsAsync(ProfileId);
        }
    }
}
