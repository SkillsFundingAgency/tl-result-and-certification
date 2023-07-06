﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.DualSpecialismToSpecialism
{
    public class When_GetMany_Is_Called : BaseTest<TlDualSpecialismToSpecialism>
    {
        private IEnumerable<TlDualSpecialismToSpecialism> _result;
        private IEnumerable<TlDualSpecialismToSpecialism> _data;

        public override void Given()
        {
            _data = new TlDualSpecialismToSpecialismBuilder().BuildList();
            DbContext.AddRange(_data);
            DbContext.SaveChanges();
        }

        public async override Task When()
        {
            _result = await Repository.GetManyAsync().ToListAsync();
        }

        [Fact]
        public void Then_Results_Not_Null()
        {
            _result.Should().NotBeNull();
        }


        [Fact]
        public void Then_The_Expected_Number_Of_Records_Are_Returned()
        {
            _result.Count().Should().Be(_data.Count());
        }

        [Fact]
        public void Then_First_Record_Fields_Have_Expected_Values()
        {
            var expectedResult = _data.FirstOrDefault();
            var actualResult = _result.FirstOrDefault();
            
            expectedResult.Should().NotBeNull();
            actualResult.Should().NotBeNull();
            actualResult.Id.Should().Be(1);
            actualResult.TlDualSpecialismId.Should().Be(expectedResult.TlDualSpecialismId);
            actualResult.TlSpecialismId.Should().Be(expectedResult.TlSpecialismId);
            actualResult.CreatedBy.Should().Be(expectedResult.CreatedBy);
            actualResult.CreatedOn.Should().Be(expectedResult.CreatedOn);
            actualResult.ModifiedBy.Should().Be(expectedResult.ModifiedBy);
            actualResult.ModifiedOn.Should().Be(expectedResult.ModifiedOn);
        }
    }
}
