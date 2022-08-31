﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.AssessmentSeries
{
    public class When_GetFirstOrDefault_Is_Called : BaseTest<Domain.Models.AssessmentSeries>
    {
        private Domain.Models.AssessmentSeries _result;
        private Domain.Models.AssessmentSeries _data;

        public override void Given()
        {
            _data = new AssessmentSeriesBuilder().Build();
            DbContext.AddRange(_data);
            DbContext.SaveChanges();
        }

        public async override Task When()
        {
            _result = await Repository.GetFirstOrDefaultAsync(x => x.Id == 1);
        }

        [Fact]
        public void Then_Fields_Are_As_Expected()
        {
            _data.Should().NotBeNull();
            _result.Should().NotBeNull();
            _result.Id.Should().Be(1);
            _result.ComponentType.Should().Be(_data.ComponentType);
            _result.Name.Should().Be(_data.Name);
            _result.Description.Should().Be(_data.Description);
            _result.Year.Should().Be(_data.Year);            
            _result.StartDate.Should().Be(_data.StartDate);
            _result.EndDate.Should().Be(_data.EndDate);
            _result.RommEndDate.Should().Be(_data.RommEndDate);
            _result.AppealEndDate.Should().Be(_data.AppealEndDate);
            _result.ResultCalculationYear.Should().Be(_data.ResultCalculationYear);
            _result.ResultPublishDate.Should().Be(_data.ResultPublishDate);
            _result.PrintAvailableDate.Should().Be(_data.PrintAvailableDate);
            _result.CreatedBy.Should().Be(Constants.CreatedByUser);
            _result.CreatedOn.Should().Be(Constants.CreatedOn);
            _result.ModifiedBy.Should().Be(Constants.ModifiedByUser);
            _result.ModifiedOn.Should().Be(Constants.ModifiedOn);
        }
    }
}
