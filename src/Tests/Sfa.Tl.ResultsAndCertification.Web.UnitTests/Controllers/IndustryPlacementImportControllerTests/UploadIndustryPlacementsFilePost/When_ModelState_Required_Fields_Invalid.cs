﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement;
using Xunit;
using UploadContent = Sfa.Tl.ResultsAndCertification.Web.Content.IndustryPlacement.Upload;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.IndustryPlacementImportControllerTests.UploadIndustryPlacementsFilePost
{
    public class When_ModelState_Required_Fields_Invalid : TestSetup
    {
        public override void Given()
        {
            Controller.ModelState.AddModelError("File", UploadContent.Select_File_To_Upload_Required_Validation_Message);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            Result.Should().BeOfType(typeof(ViewResult));

            var viewResult = Result as ViewResult;
            viewResult.Model.Should().BeOfType(typeof(UploadIndustryPlacementsRequestViewModel));

            Controller.ViewData.ModelState.Should().ContainSingle();
            Controller.ViewData.ModelState.ContainsKey(nameof(UploadIndustryPlacementsRequestViewModel.File)).Should().BeTrue();

            var modelState = Controller.ViewData.ModelState[nameof(UploadIndustryPlacementsRequestViewModel.File)];
            modelState.Errors[0].ErrorMessage.Should().Be(UploadContent.Select_File_To_Upload_Required_Validation_Message);
        }
    }
}
