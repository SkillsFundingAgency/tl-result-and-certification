﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Content.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Web.Utilities.CustomValidations;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.BackLink;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Registration.LearnerName;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Summary.SummaryItem;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.AdminDashboardControllerTests.ReviewChangeStartYearGet
{
    public class When_Called_With_Valid_Data : AdminDashboardControllerTestBase
    {
        public int PathwayId { get; set; }
        public IActionResult Result { get; private set; }
        protected AdminChangeStartYearViewModel AdminChangeStartYearViewModel = null;
        protected ReviewChangeStartYearViewModel Mockresult = null;

        public override void Given()
      {
            PathwayId = 10;
            AdminChangeStartYearViewModel = new() { AcademicYearTo = "2021" };
            Mockresult = new ReviewChangeStartYearViewModel
            {
                Uln = 1235469874,
                FirstName = "John",
                LastName = "Smith",
                ProviderName = "Barnsley College",
                ProviderUkprn = 58794528,
                TlevelName = "Tlevel in Test Pathway Name",
                AcademicYear = 2020,
                DisplayAcademicYear = "2021 to 2022",
                ContactName = "Lorem Ipsum",
                Day = "01",
                Month = "01",
                Year = "1970",
                ChangeReason = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                        "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, " +
                        "when an unknown printer took a galley of type and scrambled it to make a type specimen book. " +
                        "It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. " +
                        "It was popularised in the 1960s with the release of Letraset sheets containing " +
                        "Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                ZendeskId = "1234567890"
            };

            AdminDashboardLoader.GetAdminLearnerRecordAsync<ReviewChangeStartYearViewModel>(PathwayId).Returns(Mockresult);
            CacheService.GetAsync<AdminChangeStartYearViewModel>(CacheKey).Returns(AdminChangeStartYearViewModel);
        }

        public async override Task When()
        {
            Result = await Controller.ReviewChangeStartYearAsync(PathwayId);
        }


        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            AdminDashboardLoader.Received(1).GetAdminLearnerRecordAsync<ReviewChangeStartYearViewModel>(PathwayId);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            Result.Should().NotBeNull();
            (Result as ViewResult).Model.Should().NotBeNull();

            var model = (Result as ViewResult).Model as ReviewChangeStartYearViewModel;

            model.PathwayId.Should().Be(Mockresult.PathwayId);
            model.Uln.Should().Be(Mockresult.Uln);
            model.Learner.Should().Be(Mockresult.Learner);
            model.FirstName.Should().Be(Mockresult.FirstName);
            model.LastName.Should().Be(Mockresult.LastName);
            model.ProviderName.Should().Be(Mockresult.ProviderName);
            model.ProviderUkprn.Should().Be(Mockresult.ProviderUkprn);
            model.TlevelName.Should().Be(Mockresult.TlevelName);
            model.DisplayAcademicYear.Should().Be("2021 to 2022");

            model.AcademicYear.Should().Be(Mockresult.AcademicYear);
            model.ContactName.Should().Be(Mockresult.ContactName);
            model.Day.Should().Be(Mockresult.Day);
            model.Month.Should().Be(Mockresult.Month);
            model.Year.Should().Be(Mockresult.Year);
            model.ChangeReason.Should().Be(Mockresult.ChangeReason);
            model.ZendeskId.Should().Be(Mockresult.ZendeskId);

            // Learner
            model.SummaryLearner.Title.Should().Be(ReviewChangeStartYear.Title_Learner_Text);
            model.SummaryLearner.Value.Should().Be(Mockresult.Learner);

            //Uln
            model.SummaryULN.Title.Should().Be(ReviewChangeStartYear.Title_ULN_Text);
            model.SummaryULN.Value.Should().Be(Mockresult.Uln.ToString());

            // Provider
            model.SummaryProvider.Title.Should().Be(ReviewChangeStartYear.Title_Provider_Text);
            model.SummaryProvider.Value.Should().Be($"{Mockresult.ProviderName} ({Mockresult.ProviderUkprn.ToString()})");

            // TLevelTitle
            model.SummaryTlevel.Title.Should().Be(ReviewChangeStartYear.Title_TLevel_Text);
            model.SummaryTlevel.Value.Should().Be(Mockresult.TlevelName);

            // Start Year
            model.SummaryAcademicYear.Title.Should().Be(ReviewChangeStartYear.Title_StartYear_Text);
            model.SummaryAcademicYear.Value.Should().Be(Mockresult.DisplayAcademicYear);

            // Back link
            model.BackLink.Should().NotBeNull();
            model.BackLink.RouteName.Should().Be(RouteConstants.ChangeStartYear);

            // Contact Name
            model.SummaryContactName.Title.Should().Be(ReviewChangeStartYear.Title_Contact_Name_Text);
            model.SummaryContactName.Value.Should().Be(Mockresult.ContactName);

            // Day
            model.SummaryDay.Title.Should().Be(ReviewChangeStartYear.Title_Day_Text);
            model.SummaryDay.Value.Should().Be(Mockresult.Day);

            // Month
            model.SummaryMonth.Title.Should().Be(ReviewChangeStartYear.Title_Month_Text);
            model.SummaryMonth.Value.Should().Be(Mockresult.Month);

            // Year
            model.SummaryYear.Title.Should().Be(ReviewChangeStartYear.Title_Year_Text);
            model.SummaryYear.Value.Should().Be(Mockresult.Year);

            // Change Reason
            model.SummaryChangeReason.Title.Should().Be(ReviewChangeStartYear.Title_Reason_For_Change_Text);
            model.SummaryChangeReason.Value.Should().Be(Mockresult.ChangeReason);

            // Zendesk ID
            model.SummaryZendeskTicketId.Title.Should().Be(ReviewChangeStartYear.Title_Zendesk_Ticket_Id);
            model.SummaryZendeskTicketId.Value.Should().Be(Mockresult.ZendeskId);
        }
    }
}
