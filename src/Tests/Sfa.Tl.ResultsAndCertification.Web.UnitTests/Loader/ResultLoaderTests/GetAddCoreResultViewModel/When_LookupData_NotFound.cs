﻿using FluentAssertions;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using System.Collections.Generic;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.ResultLoaderTests.GetAddCoreResultViewModel
{
    public class When_LookupData_NotFound : TestSetup
    {
        public override void Given()
        {
            expectedApiResultDetails = new Models.Contracts.ResultDetails { PathwayAssessmentId = AssessmentId };
            InternalApiClient.GetResultDetailsAsync(AoUkprn, ProfileId).Returns(expectedApiResultDetails);

            expectedApiLookupData = new List<LookupData>();
            InternalApiClient.GetLookupDataAsync(LookupCategory.PathwayComponentGrade).Returns(expectedApiLookupData);
        }

        [Fact]
        public void Then_Returns_Null()
        {
            ActualResult.Should().BeNull();
        }

        [Fact]
        public void Then_Expected_Methods_Are_Called()
        {
            InternalApiClient.Received(1).GetResultDetailsAsync(AoUkprn, ProfileId);
            InternalApiClient.Received(1).GetLookupDataAsync(LookupCategory.PathwayComponentGrade);
        }
    }
}
