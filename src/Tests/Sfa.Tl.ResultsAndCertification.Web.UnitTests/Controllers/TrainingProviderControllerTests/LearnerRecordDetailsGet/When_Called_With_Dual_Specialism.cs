﻿using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.LearnerRecordDetailsGet
{
    public class When_Called_With_Dual_Specialism : TestSetup
    {
        private Dictionary<string, string> _routeAttributes;
        public override void Given()
        {
            ProfileId = 10;
            Mockresult = new LearnerRecordDetailsViewModel
            {
                ProfileId = 10,
                RegistrationPathwayId = 15,
                Uln = 1235469874,
                LearnerName = "Test user",
                DateofBirth = DateTime.UtcNow.AddYears(-20),
                ProviderName = "Barsley College",
                ProviderUkprn = 58794528,
                TlevelTitle = "Tlevel in Test Pathway Name",
                AcademicYear = 2020,
                AwardingOrganisationName = "Pearson",
                MathsStatus = SubjectStatus.NotSpecified,
                EnglishStatus = SubjectStatus.NotSpecified,
                IsLearnerRegistered = true,
                RegistrationPathwayStatus = RegistrationPathwayStatus.Withdrawn,
                IsPendingWithdrawal = false,
                IndustryPlacementId = 10,
                IndustryPlacementStatus = IndustryPlacementStatus.NotSpecified,
                OverallResultDetails = new Models.OverallResults.OverallResultDetail
                {
                    PathwayName = "Pathway 1",
                    PathwayResult = "Distinction",
                    SpecialismDetails = new List<Models.OverallResults.OverallSpecialismDetail>
                    {
                        new Models.OverallResults.OverallSpecialismDetail
                        {
                            SpecialismName = "Specialism 1",
                            SpecialismResult = "A"
                        },
                         new Models.OverallResults.OverallSpecialismDetail
                        {
                            SpecialismName = "Specialism 2",
                            SpecialismResult = "B"
                        }
                    },
                    OverallResult = "Distinction"
                },
                OverallResultPublishDate = DateTime.UtcNow,
                LastDocumentRequestedDate = "01/01/2022".ToDateTime(),
                IsReprint = false
            };

            _routeAttributes = new Dictionary<string, string> { { Constants.ProfileId, Mockresult.ProfileId.ToString() } };

            TrainingProviderLoader.GetLearnerRecordDetailsAsync<LearnerRecordDetailsViewModel>(ProviderUkprn, ProfileId).Returns(Mockresult);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            TrainingProviderLoader.Received(1).GetLearnerRecordDetailsAsync<LearnerRecordDetailsViewModel>(ProviderUkprn, ProfileId);
        }

        [Fact]
        public void Then_Expect_DualSpecialism()
        {
            Result.Should().NotBeNull();
            (Result as ViewResult).Model.Should().NotBeNull();

            var model = (Result as ViewResult).Model as LearnerRecordDetailsViewModel;
            model.OverallResultDetails.SpecialismDetails.Should().HaveCountGreaterThan(1);
            model.OverallResultDetails.SpecialismDetails.ForEach(x => x.SpecialismResult.Should().NotBeNull());
            model.OverallResultDetails.SpecialismDetails.ForEach(x => x.SpecialismName.Should().NotBeNull());
        }
    }
}
