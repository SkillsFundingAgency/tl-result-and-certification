﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BreadcrumbContent = Sfa.Tl.ResultsAndCertification.Web.Content.ViewComponents.Breadcrumb;
using SearchLearnerDetailContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.SearchLearnerDetails;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.TrainingProviderControllerTests.SearchLearnerDetailsGet
{
    public class When_Cache_Found_With_SearchKey : TestSetup
    {
        private SearchCriteriaViewModel _searchCriteria;
        private SearchLearnerFiltersViewModel _searchFilters;
        private SearchLearnerDetailsListViewModel _searchLearnersList;
        private List<string> _expectedSelectedFilters;

        public override void Given()
        {
            AcademicYear = 2020;

            _searchFilters = new SearchLearnerFiltersViewModel
            {
                AcademicYears = new List<FilterLookupData>
                {
                    new FilterLookupData { Id = 2020, Name = "2020 to 2021", IsSelected = true },
                    new FilterLookupData { Id = 2021, Name = "2021 to 2022", IsSelected = false }
                },
                Status = new List<FilterLookupData>
                {
                    new FilterLookupData { Id = 1, Name = "English", IsSelected = false },
                    new FilterLookupData { Id = 2, Name = "Maths", IsSelected = true }
                },
                Tlevels = new List<FilterLookupData>
                {
                    new FilterLookupData { Id = 1, Name = "Education", IsSelected = true },
                    new FilterLookupData { Id = 2, Name = "Construction", IsSelected = true }
                },
                IsApplyFiltersSelected = true
            };

            TrainingProviderLoader.GetSearchLearnerFiltersAsync(ProviderUkprn).Returns(_searchFilters);

            _searchLearnersList = new SearchLearnerDetailsListViewModel
            {
                TotalRecords = 1,
                SearchLearnerDetailsList = new List<SearchLearnerDetailsViewModel>
                {
                    new SearchLearnerDetailsViewModel
                    {
                        ProfileId = 1,
                        LearnerName = "John Smith",
                        Uln = 1234567890,
                        StartYear = "2020 to 2021",
                        TlevelName = "Design, Surveying and Planning for Construction",
                        IsEnglishAdded = true,
                        IsMathsAdded = true,
                        IsIndustryPlacementAdded = true
                    }
                },
                PagerInfo = new ViewModel.Common.PagerViewModel
                {
                    CurrentPage = 1,
                    StartPage = 1,
                    TotalPages = 1,
                    PageSize = 10,
                    RecordFrom = 1,
                    RecordTo = 0,
                    TotalItems = 1
                }
            };

            _expectedSelectedFilters = new List<string> { "2020 to 2021", "Maths", "Education", "Construction" };

            _searchCriteria = new SearchCriteriaViewModel { SearchLearnerFilters = _searchFilters, AcademicYear = AcademicYear, PageNumber = PageNumber, SearchKey = "Smith", IsSearchKeyApplied = true };
            CacheService.GetAsync<SearchCriteriaViewModel>(CacheKey).Returns(_searchCriteria);
            TrainingProviderLoader.GetSearchLearnerFiltersAsync(ProviderUkprn).Returns(_searchFilters);
            TrainingProviderLoader.SearchLearnerDetailsAsync(ProviderUkprn, _searchCriteria).Returns(_searchLearnersList);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            CacheService.Received(1).GetAsync<SearchCriteriaViewModel>(CacheKey);
            TrainingProviderLoader.Received(1).GetSearchLearnerFiltersAsync(ProviderUkprn);
            TrainingProviderLoader.Received(1).SearchLearnerDetailsAsync(ProviderUkprn, _searchCriteria);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            var viewResult = Result as ViewResult;
            var model = viewResult.Model as RegisteredLearnersViewModel;

            model.Should().NotBeNull();
            model.SearchCriteria.Should().NotBeNull();

            var searchFilters = model.SearchCriteria.SearchLearnerFilters;
            searchFilters.Should().NotBeNull();
            searchFilters.AcademicYears.Should().NotBeNull();
            searchFilters.AcademicYears.Should().HaveCount(_searchFilters.AcademicYears.Count);
            searchFilters.AcademicYears.Should().BeEquivalentTo(_searchFilters.AcademicYears);

            searchFilters.Status.Should().NotBeNull();
            searchFilters.Status.Should().HaveCount(_searchFilters.Status.Count);
            searchFilters.Status.Should().BeEquivalentTo(_searchFilters.Status);

            searchFilters.Tlevels.Should().NotBeNull();
            searchFilters.Tlevels.Should().HaveCount(_searchFilters.Tlevels.Count);
            searchFilters.Tlevels.Should().BeEquivalentTo(_searchFilters.Tlevels);

            searchFilters.IsApplyFiltersSelected.Should().BeTrue();
            searchFilters.SelectedFilters.Should().BeEquivalentTo(_expectedSelectedFilters);

            model.SearchCriteria.SearchKey.Should().Be("Smith");
            model.SearchCriteria.IsSearchKeyApplied.Should().BeTrue();

            model.SearchLearnerDetailsList.TotalRecords.Should().Be(_searchLearnersList.TotalRecords);
            model.SearchLearnerDetailsList.SearchLearnerDetailsList.Count.Should().Be(1);

            var actualLearner = model.SearchLearnerDetailsList.SearchLearnerDetailsList.First();
            var expectedLearner = _searchLearnersList.SearchLearnerDetailsList.First();
            actualLearner.ProfileId.Should().Be(expectedLearner.ProfileId);
            actualLearner.LearnerName.Should().Be(expectedLearner.LearnerName);
            actualLearner.StartYear.Should().Be(expectedLearner.StartYear);
            actualLearner.TlevelName.Should().Be(expectedLearner.TlevelName);
            actualLearner.IsEnglishAdded.Should().Be(expectedLearner.IsEnglishAdded);
            actualLearner.IsMathsAdded.Should().Be(expectedLearner.IsMathsAdded);
            actualLearner.IsIndustryPlacementAdded.Should().Be(expectedLearner.IsIndustryPlacementAdded);
            actualLearner.IsStatusCompleted.Should().BeTrue();

            model.Breadcrumb.Should().NotBeNull();
            model.Breadcrumb.BreadcrumbItems.Count.Should().Be(2);
            model.Breadcrumb.BreadcrumbItems[0].DisplayName.Should().Be(BreadcrumbContent.Home);
            model.Breadcrumb.BreadcrumbItems[0].RouteName.Should().Be(RouteConstants.Home);
            model.Breadcrumb.BreadcrumbItems[1].DisplayName.Should().Be(BreadcrumbContent.Search_Learner_Records);
            model.Breadcrumb.BreadcrumbItems[1].RouteName.Should().Be(RouteConstants.SearchLearnerRecord);

            model.Pagination.Should().NotBeNull();
            model.Pagination.PagerInfo.Should().BeEquivalentTo(_searchLearnersList.PagerInfo);
            model.Pagination.PaginationSummary.Should().Be(SearchLearnerDetailContent.PaginationSummary_Text);
            model.Pagination.RouteName.Should().Be(RouteConstants.SearchLearnerDetails);

            model.Pagination.RouteAttributes.Count.Should().Be(1);
            model.Pagination.RouteAttributes.TryGetValue(Constants.AcademicYear, out string routeValue);
            routeValue.Should().Be(AcademicYear.ToString());
        }
    }
}
