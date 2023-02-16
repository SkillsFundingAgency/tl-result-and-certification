﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using System;

namespace Sfa.Tl.ResultsAndCertification.Web.Helpers
{
    public static class ModelStateHelper
    {
        public static Tuple<string, string> GetUploadErrorMessage(RequestErrorType errorType, Type errorResourceType, int MaxFileSizeInMb = Constants.MaxFileSizeInMb)
        {
            Tuple<string, string> errorMessage = null;
            if (errorResourceType != null)
            {
                errorMessage = errorType switch
                {
                    RequestErrorType.FileType => new Tuple<string, string>("File", CommonHelper.GetResourceMessage("Must_Be_Csv_Validation_Message", errorResourceType)),
                    RequestErrorType.FileSize => new Tuple<string, string>("File", string.Format(CommonHelper.GetResourceMessage("File_Size_Too_Large_Validation_Message", errorResourceType), MaxFileSizeInMb)),
                    RequestErrorType.NotSpecified => null,
                    _ => null
                };
            }
            return errorMessage;
        }

        public static void AddModelStateError(ModelStateDictionary modelState, string key, string errorMessage)
        {
            if(modelState != null && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(errorMessage))
            {
                modelState.AddModelError(key, errorMessage);
            }
        }
    }
}
