﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.IndustryPlacement
{
    public class When_Create_Is_Called : BaseTest<Domain.Models.IndustryPlacement>
    {
        private Domain.Models.IndustryPlacement _data;
        private int _result;

        public override void Given()
        {
            _data = new IndustryPlacementBuilder().Build();
        }

        public async override Task When()
        {
            _result = await Repository.CreateAsync(_data);
        }

        [Fact]
        public void Then_One_Record_Should_Have_Been_Created() =>
            _result.Should().Be(1);
    }
}
