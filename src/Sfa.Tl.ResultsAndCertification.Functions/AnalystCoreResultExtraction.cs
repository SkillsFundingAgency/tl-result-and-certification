using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Functions.Helpers;
using Sfa.Tl.ResultsAndCertification.Functions.Interfaces;

namespace Sfa.Tl.ResultsAndCertification.Functions
{
    public class AnalystCoreResultExtraction
    {
        private readonly IAnalystCoreResultExtractionService _analystCoreResultExtractionService;
        private readonly ICommonService _commonService;

        public AnalystCoreResultExtraction(IAnalystCoreResultExtractionService analystCoreResultExtractionService, ICommonService commonService)
        {
            _analystCoreResultExtractionService = analystCoreResultExtractionService;
            _commonService = commonService;
        }

        [FunctionName(Constants.AnalystCoreResultExtraction)]
        public async Task AnalystCoreResultExtractionServiceAsync([TimerTrigger("%AnalystCoreResultExtractionTrigger%")] TimerInfo timer, ExecutionContext context, ILogger logger)
        {
            if (timer == null) throw new ArgumentNullException(nameof(timer));

            if (_commonService.IsAnalystCoreResultExtractionTriggerValid())
            {
                var functionLogDetails = CommonHelper.CreateFunctionLogRequest(context.FunctionName, FunctionType.AnalystCoreResultExtract);
                try
                {
                    logger.LogInformation($"Function {context.FunctionName} started");
                    var stopwatch = Stopwatch.StartNew();

                    await _commonService.CreateFunctionLog(functionLogDetails);

                    var response = await _analystCoreResultExtractionService.ProcessAnalystCoreResultExtractsAsync();
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
}
