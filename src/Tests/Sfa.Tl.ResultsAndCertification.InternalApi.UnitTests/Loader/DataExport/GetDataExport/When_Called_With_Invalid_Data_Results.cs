﻿using FluentAssertions;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Models.BlobStorage;
using Sfa.Tl.ResultsAndCertification.Models.DataExport;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.UnitTests.Loader.DataExport.GetDataExport
{
    public class When_Called_With_Invalid_Data_Results : TestSetup
    {
        private IList<CoreResultsExport> _coreResults;
        private IList<SpecialismResultsExport> _specialismResults;

        public override void Given()
        {
            DataExportType = Common.Enum.DataExportType.Results;

            _coreResults = new List<CoreResultsExport> { new CoreResultsExport { Uln = 1111112548, CoreCode = "45678941", CoreAssessmentEntry = "Summer 2021", CoreGrade = "A" } };
            _specialismResults = new List<SpecialismResultsExport>();

            DataExportRepository.GetDataExportCoreResultsAsync(AoUkprn).Returns(_coreResults);
            DataExportRepository.GetDataExportSpecialismResultsAsync(AoUkprn).Returns(_specialismResults);
        }

        [Fact]
        public void Then_Expected_Results_Returned()
        {
            Response.Should().NotBeNull();
            Response.Count.Should().Be(2);

            Response.FirstOrDefault().IsDataFound.Should().BeTrue();
            Response.FirstOrDefault().ComponentType.Should().Be(Common.Enum.ComponentType.Core);

            Response.LastOrDefault().IsDataFound.Should().BeFalse();
            Response.LastOrDefault().ComponentType.Should().Be(Common.Enum.ComponentType.Specialism);
        }

        [Fact]
        public void Then_Expected_Methods_Are_Called()
        {
            DataExportRepository.Received(1).GetDataExportCoreResultsAsync(AoUkprn);
            DataExportRepository.Received(1).GetDataExportSpecialismResultsAsync(AoUkprn);
            BlobService.Received(1).UploadFromByteArrayAsync(Arg.Any<BlobStorageData>());
        }
    }
}
