﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.AssessmentSeries
{
    public class When_Update_Is_Called : BaseTest<Domain.Models.AssessmentSeries>
    {
        private Domain.Models.AssessmentSeries _result;
        private Domain.Models.AssessmentSeries _data;
        private const string ModifiedUserName = "Modified User";

        public override void Given()
        {
            _data = new AssessmentSeriesBuilder().Build();
            DbContext.Add(_data);
            DbContext.SaveChanges();

            _data.Name = "Xyx 2021";
            _data.ModifiedOn = DateTime.UtcNow;
            _data.ModifiedBy = ModifiedUserName;
        }

        public async override Task When()
        {
            await Repository.UpdateAsync(_data);
            _result = await Repository.GetSingleOrDefaultAsync(x => x.Id == 1);
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
            _result.CreatedBy.Should().Be(_data.CreatedBy);
            _result.CreatedOn.Should().Be(_data.CreatedOn);
            _result.ModifiedBy.Should().Be(_data.ModifiedBy);
            _result.ModifiedOn.Should().Be(_data.ModifiedOn);
        }
    }
}
