﻿using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Api.Client.Clients;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using Sfa.Tl.ResultsAndCertification.Tests.Common.BaseTest;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Api.Client.UnitTests.Clients.ResultsAndCertificationInternalApiClientTest
{
    public class When_SearchLearnerDetails_Called : BaseTest<ResultsAndCertificationInternalApiClient>
    {
        // inputs
        private readonly SearchLearnerRequest _apiRequest;

        // results
        private PagedResponse<SearchLearnerDetail> _acutalResult;
        private PagedResponse<SearchLearnerDetail> _expectedApiResult;

        // dependencies
        private ITokenServiceClient _tokenServiceClient;
        private ResultsAndCertificationConfiguration _configuration;
        private ResultsAndCertificationInternalApiClient _apiClient;

        public override void Setup()
        {
            _tokenServiceClient = Substitute.For<ITokenServiceClient>();
            _configuration = new ResultsAndCertificationConfiguration
            {
                ResultsAndCertificationInternalApiSettings = new ResultsAndCertificationInternalApiSettings { Uri = "http://tlevel.api.com" }
            };

            _expectedApiResult = new PagedResponse<SearchLearnerDetail>
            {
                TotalRecords = 2,
                Records = new List<SearchLearnerDetail>
                {
                    new SearchLearnerDetail
                    {
                        ProfileId = 1,
                        Uln = 1234567890,
                        LearnerName = "John Smith1",
                        TlevelTitle = "T level in Education and childcare",
                        AcademicYear = 2020,
                        EnglishStatus = SubjectStatus.Achieved,
                        MathsStatus = SubjectStatus.Achieved,
                        IndustryPlacementStatus = IndustryPlacementStatus.Completed
                    },
                    new SearchLearnerDetail
                    {
                        ProfileId = 2,
                        Uln = 2222267890,
                        LearnerName = "John Smith2",
                        TlevelTitle = "T level in Design and Survey",
                        AcademicYear = 2021,
                        EnglishStatus = null,
                        MathsStatus = null,
                        IndustryPlacementStatus = null
                    }
                }
            };
        }

        public override void Given()
        {
            HttpClient = new HttpClient(new MockHttpMessageHandler<PagedResponse<SearchLearnerDetail>>(_expectedApiResult, ApiConstants.SearchLearnerDetailsUri, HttpStatusCode.OK, JsonConvert.SerializeObject(_apiRequest)));
            _apiClient = new ResultsAndCertificationInternalApiClient(HttpClient, _tokenServiceClient, _configuration);
        }

        public async override Task When()
        {
            _acutalResult = await _apiClient.SearchLearnerDetailsAsync(_apiRequest);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            _acutalResult.Should().NotBeNull();
            _acutalResult.TotalRecords.Should().Be(_expectedApiResult.TotalRecords);
            _acutalResult.Records.Count.Should().Be(2);

            _acutalResult.Should().BeEquivalentTo(_expectedApiResult);
        }
    }
}
