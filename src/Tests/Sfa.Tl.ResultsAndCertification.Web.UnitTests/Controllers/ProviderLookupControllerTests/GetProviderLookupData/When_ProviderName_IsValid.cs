﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Provider;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.ProviderLookupControllerTests.GetProviderLookupData
{
    public class When_ProviderName_IsValid : When_FindProviderAsync_Post_Action_Is_Called
    {
        private List<ProviderLookupData> expectedResults;

        public override void Given()
        {
            expectedResults = new List<ProviderLookupData>
            {
                new ProviderLookupData { Id = 1, DisplayName = "Provider 1" },
                new ProviderLookupData { Id = 2, DisplayName = "Provider 2" },
            };

            ProviderLoader.GetProviderLookupDataAsync(ProviderName, false)
                .Returns(expectedResults);
        }

        [Fact]
        public void Then_Called_Expected_Method()
        {
            ProviderLoader.Received(1).GetProviderLookupDataAsync(ProviderName, false);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            Result.Should().BeOfType(typeof(JsonResult));
            var actualResults = Result.Value as IEnumerable<ProviderLookupData>;

            actualResults.Count().Should().Be(2);
            actualResults.First().Id.Should().Be(expectedResults.First().Id);
            actualResults.First().DisplayName.Should().Be(expectedResults.First().DisplayName);
        }
    }
}
