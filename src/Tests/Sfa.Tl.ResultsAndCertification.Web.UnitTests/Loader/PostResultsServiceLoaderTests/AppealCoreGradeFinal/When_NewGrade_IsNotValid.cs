﻿using FluentAssertions;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.PostResultsService;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System.Collections.Generic;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.PostResultsServiceLoaderTests.AppealCoreGradeFinal
{
    public class When_NewGrade_IsNotValid : TestSetup
    {
        public override void Given()
        {
            ViewModel = new PrsPathwayGradeCheckAndSubmitViewModel
            {
                ProfileId = 1,
                AssessmentId = 2,
                ResultId = 3,
                NewGrade = "K"
            };

            var lookupGrades = new List<LookupData> { new LookupData { Id = 11, Code = "A", Value = "A" } };
            InternalApiClient.GetLookupDataAsync(LookupCategory.PathwayComponentGrade).
                Returns(lookupGrades);
        }

        [Fact]
        public void Then_False_Returned()
        {
            ActualResult.Should().BeFalse();
        }

        [Fact]
        public void Then_Expected_Methods_Are_Called()
        {
            InternalApiClient.Received(1).GetLookupDataAsync(LookupCategory.PathwayComponentGrade);
            InternalApiClient.DidNotReceive().PrsActivityAsync(Arg.Any<PrsActivityRequest>());
        }
    }
}