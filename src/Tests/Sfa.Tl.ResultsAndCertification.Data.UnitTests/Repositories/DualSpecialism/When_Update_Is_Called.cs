﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Data.UnitTests.Repositories.DualSpecialism
{
    public class When_Update_Is_Called : BaseTest<TlDualSpecialism>
    {
        private TlDualSpecialism _result;
        private TlDualSpecialism _data;
        private readonly EnumAwardingOrganisation _awardingOrganisation = EnumAwardingOrganisation.Ncfe;

        private const string DualSpecialismName = "Dual Specialism Updated";
        private const string LarId = "999";
        private const string ModifiedBy = "Modified User Updated";

        public override void Given()
        {
            var specialisms = new TlDualSpecialismBuilder().BuildList(_awardingOrganisation);
            _data = specialisms.FirstOrDefault();
            DbContext.Add(_data);
            DbContext.SaveChanges();

            // Update data
            _data.Name = DualSpecialismName;
            _data.LarId = LarId;
            _data.ModifiedBy = ModifiedBy;
        }

        public async override Task When()
        {
            await Repository.UpdateAsync(_data);
            _result = await Repository.GetFirstOrDefaultAsync(x => x.Id == _data.Id);
        }

        [Fact]
        public void Then_Fields_Are_As_Expected()
        {
            _data.Should().NotBeNull();
            _result.Should().NotBeNull();
            _result.Id.Should().Be(1);
            _result.Name.Should().Be(_data.Name);
            _result.CreatedBy.Should().Be(_data.CreatedBy);
            _result.CreatedOn.Should().Be(_data.CreatedOn);
            _result.ModifiedBy.Should().Be(_data.ModifiedBy);
            _result.ModifiedOn.Should().Be(_data.ModifiedOn);
        }
    }
}
