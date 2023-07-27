using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Functions.Helpers;
using Sfa.Tl.ResultsAndCertification.Functions.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;

namespace Sfa.Tl.ResultsAndCertification.Functions
{
    public class AnalystCoreResultExtraction
    {
        private readonly IAnalystCoreResultExtractionService _analystCoreResultExtractionService;
        private readonly ICommonService _commonService;
        private readonly AnalystCoreResultExtractSettings _configuration;


        public AnalystCoreResultExtraction(ResultsAndCertificationConfiguration configuration, IAnalystCoreResultExtractionService analystCoreResultExtractionService, ICommonService commonService)
        {
            _analystCoreResultExtractionService = analystCoreResultExtractionService;
            _commonService = commonService;
            _configuration = configuration.AnalystCoreResultExtractSettings;
        }

        [FunctionName(Constants.AnalystCoreResultExtract)]
        public async Task AnalystCoreResultExtractAsync([TimerTrigger("%AnalystCoreResultExtractTrigger%")] TimerInfo timer, ExecutionContext context, ILogger logger)
        {
            logger.LogInformation($"Function {context.FunctionName} started");

            logger.LogInformation($"Function timer: {timer}");

            if (timer == null) throw new ArgumentNullException(nameof(timer));

            var today = DateTime.UtcNow.Date;
            logger.LogInformation($"Function date: {today}");

            logger.LogInformation($"Function CoreValidDateRanges: {_configuration.CoreValidDateRanges.Select(x => x.ToString())}");

            bool shouldFunctionRunToday = _configuration.CoreValidDateRanges.Any(r => r.Contains(today));

            logger.LogInformation($"Function shouldFunctionRunToday 1: {shouldFunctionRunToday}");

            if (!shouldFunctionRunToday)
            {
                await Task.CompletedTask;
                return;
            }

            logger.LogInformation($"Function shouldFunctionRunToday 2: {shouldFunctionRunToday}");

            var functionLogDetails = CommonHelper.CreateFunctionLogRequest(context.FunctionName, FunctionType.AnalystCoreResultExtract);
            try
            {
                logger.LogInformation($"Function {context.FunctionName} started");
                var stopwatch = Stopwatch.StartNew();

                await _commonService.CreateFunctionLog(functionLogDetails);

                var response = await _analystCoreResultExtractionService.ProcessAnalystCoreResultExtractsAsync(_configuration.CoreAcademicYearsToProcess);
                var message = $"Function {context.FunctionName} completed processing.\n" +
                                     $"\tStatus: {(response.IsSuccess ? FunctionStatus.Processed.ToString() : FunctionStatus.Failed.ToString())}";

                CommonHelper.UpdateFunctionLogRequest(functionLogDetails, response.IsSuccess ? FunctionStatus.Processed : FunctionStatus.Failed, message);

                await _commonService.UpdateFunctionLog(functionLogDetails);

                stopwatch.Stop();

                logger.LogInformation($"Function {context.FunctionName} completed processing. Time taken: {stopwatch.ElapsedMilliseconds: #,###}ms");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Function {context.FunctionName} failed to process with the following exception = {ex}";
                logger.LogError(errorMessage);

                CommonHelper.UpdateFunctionLogRequest(functionLogDetails, FunctionStatus.Failed, errorMessage);

                _ = functionLogDetails.Id > 0 ? await _commonService.UpdateFunctionLog(functionLogDetails) : await _commonService.CreateFunctionLog(functionLogDetails);

                await _commonService.SendFunctionJobFailedNotification(context.FunctionName, errorMessage);
            }

        }
    }
}
