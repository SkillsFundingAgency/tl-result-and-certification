﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using System;

namespace Sfa.Tl.ResultsAndCertification.Functions.Helpers
{
    public static class CommonHelper
    {

        public static FunctionLogDetails CreateFunctionLogRequest(string functionName)
        {
            return new FunctionLogDetails
            {
                Name = functionName,
                StartDate = DateTime.UtcNow,
                Status = FunctionStatus.Processing,
                PerformedBy = "System"
            };
        }

        public static FunctionLogDetails UpdateFunctionLogRequest(FunctionLogDetails functionLogDetails, FunctionStatus status, string message = null)
        {
            if(functionLogDetails != null)
            {
                functionLogDetails.Status = status;
                functionLogDetails.EndDate = DateTime.UtcNow;
                functionLogDetails.Message = message;
            }
            return functionLogDetails;
        }
    }
}
