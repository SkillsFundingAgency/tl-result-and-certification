﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Result;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Result.Manual;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces
{
    public interface IResultLoader
    {
        Task<UploadResultsResponseViewModel> ProcessBulkResultsAsync(UploadResultsRequestViewModel viewModel);
        Task<Stream> GetResultValidationErrorsFileAsync(long aoUkprn, Guid blobUniqueReference);

        Task<UlnResultsNotFoundViewModel> FindUlnResultsAsync(long aoUkprn, long Uln);
        Task<ResultDetailsViewModel> GetResultDetailsAsync(long aoUkprn, int profileId, RegistrationPathwayStatus? status = null);
        Task<AddResultResponse> AddCoreResultAsync(long aoUkprn, ManageCoreResultViewModel viewModel);
        Task<ManageCoreResultViewModel> GetManageCoreResultViewModelAsync(long aoUkprn, int profileId, int assessmentId, bool isChangeMode);
        Task<bool?> IsCoreResultChanged(long aoUkprn, ManageCoreResultViewModel model);
        Task<ChangeResultResponse> ChangeCoreResultAsync(long aoUkprn, ManageCoreResultViewModel viewModel);
    }
}
