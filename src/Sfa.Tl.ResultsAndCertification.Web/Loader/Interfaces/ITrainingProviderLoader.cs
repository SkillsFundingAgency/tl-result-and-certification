﻿using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces
{
    public interface ITrainingProviderLoader
    {
        Task<SearchLearnerDetailsListViewModel> SearchLearnerDetailsAsync(long providerUkprn, SearchCriteriaViewModel searchCriteriaViewModel);
        Task<SearchLearnerFiltersViewModel> GetSearchLearnerFiltersAsync(long providerUkprn);
        Task<FindLearnerRecord> FindLearnerRecordAsync(long providerUkprn, long uln);
        Task<T> GetLearnerRecordDetailsAsync<T>(long providerUkprn, int profileId, int? pathwayId = null);
        Task<bool> UpdateLearnerSubjectAsync(long providerUkprn, AddMathsStatusViewModel model);
        Task<bool> UpdateLearnerSubjectAsync(long providerUkprn, AddEnglishStatusViewModel model);
        Task<bool> UpdateLearnerWithdrawnStatusAsync(long providerUkprn, ChangeWithdrawnStatusHaveYouToldAwardingOrganisationViewModel model);
        Task<bool> CreateReplacementDocumentPrintingRequestAsync(long providerUkprn, RequestReplacementDocumentViewModel viewModel);
        Task<bool> ReinstateRegistrationFromPendingWithdrawalAsync(ChangeBackToActiveStatusHaveYouToldAwardingOrganisationViewModel model);
    }
}
