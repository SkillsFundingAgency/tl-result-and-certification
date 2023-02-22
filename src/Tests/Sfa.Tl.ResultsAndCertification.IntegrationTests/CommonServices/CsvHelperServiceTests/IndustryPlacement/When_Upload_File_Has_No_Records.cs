﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.DataParser;
using Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.DataValidators;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.Service;
using Sfa.Tl.ResultsAndCertification.Models.BulkProcess;
using System.IO;
using Xunit;
using Sfa.Tl.ResultsAndCertification.Models.IndustryPlacement.BulkProcess;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.CommonServices.CsvHelperServiceTests.IndustryPlacement
{
    public class When_Upload_File_Has_No_Records : IndustryPlacementStatusCsvHelperServiceBaseTest
    {
        private const string _dataFilePath = @"CommonServices\CsvHelperServiceTests\IndustryPlacement\TestData\IndustryPlacement_Stage2_Has_No_records.csv";

        public override void Given()
        {
            Logger = new Logger<CsvHelperService<IndustryPlacementCsvRecordRequest, CsvResponseModel<IndustryPlacementCsvRecordResponse>, IndustryPlacementCsvRecordResponse>>(new NullLoggerFactory());
            DataParser = new IndustryPlacementParser();
            Validator = new IndustryPlacementValidator();
            Service = new CsvHelperService<IndustryPlacementCsvRecordRequest, CsvResponseModel<IndustryPlacementCsvRecordResponse>, IndustryPlacementCsvRecordResponse>(Validator, DataParser, Logger);
            FilePath = Path.Combine(Path.GetDirectoryName(GetCodeBaseAbsolutePath()), _dataFilePath);
        }

        [Fact]
        public async Task Then_Returns_Expected_Results()
        {
            await WhenAsync();

            ReadAndParseFileResponse.Should().NotBeNull();
            ReadAndParseFileResponse.IsDirty.Should().BeTrue();
            ReadAndParseFileResponse.ErrorMessage.Should().Be(ValidationMessages.NoRecordsFound);
        }
    }
}
