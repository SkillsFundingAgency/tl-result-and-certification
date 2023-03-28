﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.ProviderAddress;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.StatementOfAchievement;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using CheckAndSubmitContent = Sfa.Tl.ResultsAndCertification.Web.Content.StatementOfAchievement.RequestSoaCheckAndSubmit;
using BreadcrumbContent = Sfa.Tl.ResultsAndCertification.Web.Content.ViewComponents.Breadcrumb;
using IndustryPlacementStatusContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.IndustryPlacementStatus;
using SubjectStatusContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.SubjectStatus;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.StatementOfAchievementControllerTests.RequestSoaCheckAndSubmitGet
{
    public class When_ViewModel_IsValid : TestSetup
    {
        private SoaLearnerRecordDetailsViewModel _mockLearnerDetails;
        private AddressViewModel _address;

        public override void Given()
        {
            ProfileId = 11;
            ResultsAndCertificationConfiguration.SoaAvailableDate = DateTime.UtcNow.AddDays(-30);

            _address = new AddressViewModel { AddressId = 1, DepartmentName = "Operations", OrganisationName = "College Ltd", AddressLine1 = "10, House", AddressLine2 = "Street", Town = "Birmingham", Postcode = "B1 1AA" };
            _mockLearnerDetails = new SoaLearnerRecordDetailsViewModel
            {
                ProfileId = ProfileId,
                Uln = 1234567890,
                LearnerName = "John Smith",
                DateofBirth = DateTime.Now.AddYears(-20),
                ProviderDisplayName = "Barsley College (569874567)",
                ProviderName = "Barsley College",
                ProviderUkprn = 569874567,

                TlevelTitle = "Design, Surveying and Planning for Construction",
                RegistrationPathwayId = 1,
                PathwayDisplayName = "Design, Surveying and Planning for Construction (60358300)",
                PathwayName = "Design, Surveying and Planning for Construction",
                PathwayCode = "60358300",
                PathwayGrade = "A*",
                SpecialismDisplayName = "Building Services Design (ZTLOS003)",
                SpecialismName = "Building Services Design",
                SpecialismCode = "ZTLOS003",
                SpecialismGrade = "None",

                EnglishStatus = SubjectStatus.Achieved,
                MathsStatus = SubjectStatus.Achieved,
                IndustryPlacementStatus = IndustryPlacementStatus.Completed,

                HasPathwayResult = true,
                IsNotWithdrawn = false,
                IsLearnerRegistered = true,
                IsIndustryPlacementAdded = true,
                IsIndustryPlacementCompleted = true,

                ProviderAddress = _address,
            };

            StatementOfAchievementLoader.GetSoaLearnerRecordDetailsAsync(ProviderUkprn, ProfileId).Returns(_mockLearnerDetails);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            StatementOfAchievementLoader.Received(1).GetSoaLearnerRecordDetailsAsync(ProviderUkprn, ProfileId);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            var viewResult = Result as ViewResult;
            var model = viewResult.Model as SoaLearnerRecordDetailsViewModel;

            model.Should().NotBeNull();
            model.ProfileId.Should().Be(_mockLearnerDetails.ProfileId);
            model.Uln.Should().Be(_mockLearnerDetails.Uln);
            model.LearnerName.Should().Be(_mockLearnerDetails.LearnerName);
            model.DateofBirth.Should().Be(_mockLearnerDetails.DateofBirth);
            model.ProviderDisplayName.Should().Be(_mockLearnerDetails.ProviderDisplayName);
            model.ProviderName.Should().Be(_mockLearnerDetails.ProviderName);
            model.ProviderUkprn.Should().Be(_mockLearnerDetails.ProviderUkprn);

            model.TlevelTitle.Should().Be(_mockLearnerDetails.TlevelTitle);
            model.RegistrationPathwayId.Should().Be(_mockLearnerDetails.RegistrationPathwayId);
            model.PathwayDisplayName.Should().Be(_mockLearnerDetails.PathwayDisplayName);
            model.PathwayName.Should().Be(_mockLearnerDetails.PathwayName);
            model.PathwayCode.Should().Be(_mockLearnerDetails.PathwayCode);
            model.PathwayGrade.Should().Be(_mockLearnerDetails.PathwayGrade);
            model.SpecialismDisplayName.Should().Be(_mockLearnerDetails.SpecialismDisplayName);
            model.SpecialismName.Should().Be(_mockLearnerDetails.SpecialismName);
            model.SpecialismCode.Should().Be(_mockLearnerDetails.SpecialismCode);
            model.SpecialismGrade.Should().Be(_mockLearnerDetails.SpecialismGrade);

            model.EnglishStatus.Should().Be(_mockLearnerDetails.EnglishStatus);
            model.MathsStatus.Should().Be(_mockLearnerDetails.MathsStatus);
            model.IndustryPlacementStatus.Should().Be(_mockLearnerDetails.IndustryPlacementStatus);

            // Uln
            model.SummaryUln.Title.Should().Be(CheckAndSubmitContent.Title_Uln_Text);
            model.SummaryUln.Value.Should().Be(_mockLearnerDetails.Uln.ToString());

            // LearnerName
            model.SummaryLearnerName.Title.Should().Be(CheckAndSubmitContent.Title_Name_Text);
            model.SummaryLearnerName.Value.Should().Be(_mockLearnerDetails.LearnerName);

            // DateofBirth
            model.SummaryDateofBirth.Title.Should().Be(CheckAndSubmitContent.Title_DateofBirth_Text);
            model.SummaryDateofBirth.Value.Should().Be(_mockLearnerDetails.DateofBirth.ToDobFormat());

            // ProviderName
            model.SummaryProvider.Title.Should().Be(CheckAndSubmitContent.Title_Provider_Text);
            model.SummaryProvider.Value.Should().Be(_mockLearnerDetails.ProviderDisplayName);

            // TLevelTitle
            model.SummaryTlevelTitle.Title.Should().Be(CheckAndSubmitContent.Title_Tlevel_Title_Text);
            model.SummaryTlevelTitle.Value.Should().Be(_mockLearnerDetails.TlevelTitle);

            // CoreCode
            model.SummaryCoreCode.Title.Should().Be(CheckAndSubmitContent.Title_Core_Code_Text);
            model.SummaryCoreCode.Value.Should().Be(string.Format(CheckAndSubmitContent.Core_Code_Value, _mockLearnerDetails.PathwayDisplayName, _mockLearnerDetails.PathwayGrade));

            // SpecialismCode
            model.SummarySpecialismCode.Title.Should().Be(CheckAndSubmitContent.Title_Occupational_Specialism_Text);
            model.SummarySpecialismCode.Value.Should().Be(string.Format(CheckAndSubmitContent.Occupational_Specialism_Value, _mockLearnerDetails.SpecialismDisplayName, _mockLearnerDetails.SpecialismGrade));

            // EnglishStatus
            model.SummaryEnglishStatus.Title.Should().Be(CheckAndSubmitContent.Title_English_Text);
            model.SummaryEnglishStatus.Value.Should().Be(SubjectStatusContent.Achieved_Display_Text);

            // MathsStatus
            model.SummaryMathsStatus.Title.Should().Be(CheckAndSubmitContent.Title_Maths_Text);
            model.SummaryMathsStatus.Value.Should().Be(SubjectStatusContent.Achieved_Display_Text);

            // Industry Placement
            model.SummaryIndustryPlacement.Title.Should().Be(CheckAndSubmitContent.Title_Industry_Placement_Text);
            model.SummaryIndustryPlacement.Value.Should().Be(IndustryPlacementStatusContent.Completed_Display_Text);

            // Department
            model.SummaryDepartment.Title.Should().Be(CheckAndSubmitContent.Title_Department_Text);
            model.SummaryDepartment.Value.Should().Be(_address.DepartmentName);

            // Address
            model.SummaryAddress.Title.Should().Be(CheckAndSubmitContent.Title_Organisation_Address_Text);
            model.SummaryAddress.Value.Should().Be(string.Format(CheckAndSubmitContent.Organisation_Address_Value, FormatedAddress));

            // Breadcrum 
            model.Breadcrumb.Should().NotBeNull();
            model.Breadcrumb.BreadcrumbItems.Count.Should().Be(4);

            model.Breadcrumb.BreadcrumbItems[0].DisplayName.Should().Be(BreadcrumbContent.Home);
            model.Breadcrumb.BreadcrumbItems[0].RouteName.Should().Be(RouteConstants.Home);
            model.Breadcrumb.BreadcrumbItems[1].DisplayName.Should().Be(BreadcrumbContent.Request_Statement_Of_Achievement);
            model.Breadcrumb.BreadcrumbItems[1].RouteName.Should().Be(RouteConstants.RequestStatementOfAchievement);
            model.Breadcrumb.BreadcrumbItems[2].DisplayName.Should().Be(BreadcrumbContent.Search_For_Learner);
            model.Breadcrumb.BreadcrumbItems[2].RouteName.Should().Be(RouteConstants.RequestSoaUniqueLearnerNumber);
            model.Breadcrumb.BreadcrumbItems[3].DisplayName.Should().Be(BreadcrumbContent.Check_Learner_Details);
            model.Breadcrumb.BreadcrumbItems[3].RouteName.Should().BeNull();
        }

        private string FormatedAddress
        {
            get
            {
                var addressLines = new List<string> { _address.OrganisationName, _address.AddressLine1, _address.AddressLine2, _address.Town, _address.Postcode };
                return string.Join(CheckAndSubmitContent.Html_Line_Break, addressLines.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }
    }
}
