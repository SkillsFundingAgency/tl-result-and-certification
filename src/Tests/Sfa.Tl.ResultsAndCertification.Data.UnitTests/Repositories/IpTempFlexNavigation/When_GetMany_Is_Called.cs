﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.IpTempFlexNavigation
{
    public class When_GetMany_Is_Called : BaseTest<Domain.Models.IpTempFlexNavigation>
    {
        private IEnumerable<Domain.Models.IpTempFlexNavigation> _result;
        private IEnumerable<Domain.Models.IpTempFlexNavigation> _data;

        public override void Given()
        {
            _data = new IpTempFlexNavigationBuilder().BuildList(EnumAwardingOrganisation.Pearson);
            DbContext.AddRange(_data);
            DbContext.SaveChanges();
        }

        public async override Task When()
        {
            _result = await Repository.GetManyAsync().ToListAsync();
        }

        [Fact]
        public void Then_EntityFields_Are_As_Expected()
        {
            var expectedResult = _data.FirstOrDefault();
            var actualResult = _result.FirstOrDefault();

            expectedResult.Should().NotBeNull();
            actualResult.Should().NotBeNull();
            actualResult.Id.Should().Be(1);
            actualResult.TlPathwayId.Should().Be(expectedResult.TlPathwayId);
            actualResult.AcademicYear.Should().Be(expectedResult.AcademicYear);
            actualResult.AskTempFlexibility.Should().Be(expectedResult.AskTempFlexibility);
            actualResult.AskBlendedPlacement.Should().Be(expectedResult.AskBlendedPlacement);
            actualResult.IsActive.Should().Be(expectedResult.IsActive);
            actualResult.CreatedBy.Should().BeEquivalentTo(expectedResult.CreatedBy);
            actualResult.CreatedOn.Should().Be(expectedResult.CreatedOn);
            actualResult.ModifiedBy.Should().BeEquivalentTo(expectedResult.ModifiedBy);
            actualResult.ModifiedOn.Should().Be(expectedResult.ModifiedOn);
        }
    }
}
