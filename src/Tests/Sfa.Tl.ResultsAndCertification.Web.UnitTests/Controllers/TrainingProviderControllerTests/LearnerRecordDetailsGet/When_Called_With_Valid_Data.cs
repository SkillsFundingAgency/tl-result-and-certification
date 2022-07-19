﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using System;
using Xunit;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using SubjectStatusContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.SubjectStatus;
using LearnerRecordDetailsContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.LearnerRecordDetails;
using System.Collections.Generic;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.LearnerRecordDetailsGet
{
    public class When_Called_With_Valid_Data : TestSetup
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
                        }
                    },
                    OverallResult = "Distinction"
                },
                OverallResultPublishDate = DateTime.UtcNow
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
        public void Then_Returns_Expected_Results()
        {
            Result.Should().NotBeNull();
            (Result as ViewResult).Model.Should().NotBeNull();

            var model = (Result as ViewResult).Model as LearnerRecordDetailsViewModel;

            model.ProfileId.Should().Be(Mockresult.ProfileId);
            model.RegistrationPathwayId.Should().Be(Mockresult.RegistrationPathwayId);
            model.Uln.Should().Be(Mockresult.Uln);
            model.LearnerName.Should().Be(Mockresult.LearnerName);
            model.DateofBirth.Should().Be(Mockresult.DateofBirth);
            model.ProviderName.Should().Be(Mockresult.ProviderName);
            model.ProviderUkprn.Should().Be(Mockresult.ProviderUkprn);
            model.TlevelTitle.Should().Be(Mockresult.TlevelTitle);
            model.StartYear.Should().Be("2020 to 2021");
            model.AwardingOrganisationName.Should().Be(Mockresult.AwardingOrganisationName);
            model.MathsStatus.Should().Be(Mockresult.MathsStatus);
            model.EnglishStatus.Should().Be(Mockresult.EnglishStatus);
            model.IsLearnerRegistered.Should().Be(Mockresult.IsLearnerRegistered);

            model.IndustryPlacementId.Should().Be(Mockresult.IndustryPlacementId);
            model.IndustryPlacementStatus.Should().Be(Mockresult.IndustryPlacementStatus);

            model.IsMathsAdded.Should().BeFalse();
            model.IsEnglishAdded.Should().BeFalse();
            model.IsIndustryPlacementAdded.Should().BeFalse();
            model.CanAddIndustryPlacement.Should().BeTrue();
            model.IsStatusCompleted.Should().BeFalse();

            // DateofBirth
            model.SummaryDateofBirth.Title.Should().Be(LearnerRecordDetailsContent.Title_DateofBirth_Text);
            model.SummaryDateofBirth.Value.Should().Be(Mockresult.DateofBirth.ToDobFormat());

            // ProviderName
            model.SummaryProviderName.Title.Should().Be(LearnerRecordDetailsContent.Title_Provider_Name_Text);
            model.SummaryProviderName.Value.Should().Be(Mockresult.ProviderName);

            // ProviderUkprn
            model.SummaryProviderUkprn.Title.Should().Be(LearnerRecordDetailsContent.Title_Provider_Ukprn_Text);
            model.SummaryProviderUkprn.Value.Should().Be(Mockresult.ProviderUkprn.ToString());

            // TLevelTitle
            model.SummaryTlevelTitle.Title.Should().Be(LearnerRecordDetailsContent.Title_TLevel_Text);
            model.SummaryTlevelTitle.Value.Should().Be(Mockresult.TlevelTitle);
            
            // Start Year
            model.SummaryStartYear.Title.Should().Be(LearnerRecordDetailsContent.Title_StartYear_Text);
            model.SummaryStartYear.Value.Should().Be(Mockresult.StartYear);

            // AO Name
            model.SummaryAoName.Title.Should().Be(LearnerRecordDetailsContent.Title_AoName_Text);
            model.SummaryAoName.Value.Should().Be(Mockresult.AwardingOrganisationName);

            // Summary Industry Placement
            model.SummaryIndustryPlacementStatus.Should().NotBeNull();
            model.SummaryIndustryPlacementStatus.Title.Should().Be(LearnerRecordDetailsContent.Title_IP_Status_Text);
            model.SummaryIndustryPlacementStatus.Value.Should().Be(SubjectStatusContent.Not_Yet_Recevied_Display_Text);
            model.SummaryIndustryPlacementStatus.HiddenActionText.Should().Be(LearnerRecordDetailsContent.Hidden_Action_Text_Industry_Placement);
            model.SummaryIndustryPlacementStatus.ActionText.Should().Be(LearnerRecordDetailsContent.Action_Text_Link_Add);
            model.SummaryIndustryPlacementStatus.RouteName.Should().Be(RouteConstants.AddIndustryPlacement);
            model.SummaryIndustryPlacementStatus.RouteAttributes.Should().BeEquivalentTo(_routeAttributes);

            // Summary Maths StatusHidden_Action_Text_Maths
            model.SummaryMathsStatus.Should().NotBeNull();
            model.SummaryMathsStatus.Title.Should().Be(LearnerRecordDetailsContent.Title_Maths_Text);
            model.SummaryMathsStatus.Value.Should().Be(SubjectStatusContent.Not_Yet_Recevied_Display_Text);
            model.SummaryMathsStatus.NeedBorderBottomLine.Should().BeTrue();
            model.SummaryMathsStatus.HiddenActionText.Should().Be(LearnerRecordDetailsContent.Hidden_Action_Text_Maths);
            model.SummaryMathsStatus.ActionText.Should().Be(LearnerRecordDetailsContent.Action_Text_Link_Add);
            model.SummaryMathsStatus.RouteName.Should().Be(RouteConstants.AddMathsStatus);
            model.SummaryMathsStatus.RouteAttributes.Should().BeEquivalentTo(_routeAttributes);

            // Summary English Status
            model.SummaryEnglishStatus.Should().NotBeNull();
            model.SummaryEnglishStatus.Title.Should().Be(LearnerRecordDetailsContent.Title_English_Text);
            model.SummaryMathsStatus.Value.Should().Be(SubjectStatusContent.Not_Yet_Recevied_Display_Text);
            model.SummaryEnglishStatus.NeedBorderBottomLine.Should().BeTrue();
            model.SummaryEnglishStatus.HiddenActionText.Should().Be(LearnerRecordDetailsContent.Hidden_Action_Text_English);
            model.SummaryEnglishStatus.ActionText.Should().Be(LearnerRecordDetailsContent.Action_Text_Link_Add);
            model.SummaryEnglishStatus.RouteName.Should().Be(RouteConstants.AddEnglishStatus);
            model.SummaryEnglishStatus.RouteAttributes.Should().BeEquivalentTo(_routeAttributes);

            model.DisplayOverallResults.Should().BeTrue();

            // Overall core result details
            model.SummaryCoreResult.Title.Should().Be(Mockresult.OverallResultDetails.PathwayName);
            model.SummaryCoreResult.Value.Should().Be(Mockresult.OverallResultDetails.PathwayResult);

            // Overall Specialism result details
            model.SummarySpecialismResult.Title.Should().Be(Mockresult.OverallResultDetails.SpecialismDetails[0].SpecialismName);
            model.SummarySpecialismResult.Value.Should().Be(Mockresult.OverallResultDetails.SpecialismDetails[0].SpecialismResult);

            // Overall Result
            model.SummaryOverallResult.Title.Should().Be(LearnerRecordDetailsContent.Title_OverallResult_Text);
            model.SummaryOverallResult.Value.Should().Be(Mockresult.OverallResultDetails.OverallResult);


            // Back link
            model.BackLink.Should().NotBeNull();
            model.BackLink.RouteName.Should().Be(RouteConstants.SearchLearnerRecord);
        }
    }
}
