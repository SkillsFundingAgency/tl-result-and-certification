﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TlevelControllerTests.VerifyAsync
{
    public class When_ViewModel_IsValid : TestSetup
    {
        private ConfirmTlevelViewModel expectedModel;

        public override void Given()
        {
            pathwayId = 10;
            expectedModel = new ConfirmTlevelViewModel 
            {
                TqAwardingOrganisationId = 1,
                RouteId = 2,
                PathwayId = pathwayId,
                PathwayStatusId = (int)TlevelReviewStatus.AwaitingConfirmation,
                IsEverythingCorrect = true,
                TlevelTitle = "Tlevel title",
                PathwayDisplayName = "Pathway1<br/>(45789465489)",
                Specialisms = new List<string> { "sp1<br/>(567565)", "sp2<br/>(564547)" }
            };

            TlevelLoader.GetVerifyTlevelDetailsByPathwayIdAsync(AoUkprn, pathwayId)
                .Returns(expectedModel);
        }

        [Fact]
        public void Then_Returns_Expected_ViewModel()
        {
            var viewResult = Result as ViewResult;
            var model = viewResult.Model as ConfirmTlevelViewModel;

            model.Should().NotBeNull();
            model.TqAwardingOrganisationId.Should().Be(expectedModel.TqAwardingOrganisationId);
            model.RouteId.Should().Be(expectedModel.RouteId);
            model.PathwayId.Should().Be(expectedModel.PathwayId);
            model.PathwayStatusId.Should().Be(expectedModel.PathwayStatusId);
            model.TlevelTitle.Should().Be(expectedModel.TlevelTitle);
            model.PathwayDisplayName.Should().Be(expectedModel.PathwayDisplayName);
            model.Specialisms.Should().BeEquivalentTo(expectedModel.Specialisms);
            model.IsEverythingCorrect.Should().Be(expectedModel.IsEverythingCorrect);
            model.Specialisms.Should().NotBeNull();
            model.Specialisms.Count().Should().Be(2);
        }
    }
}
