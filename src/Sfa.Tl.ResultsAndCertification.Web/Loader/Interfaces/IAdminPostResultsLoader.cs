﻿using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminPostResults;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces
{
    public interface IAdminPostResultsLoader
    {
        Task<AdminOpenPathwayRommViewModel> GetAdminOpenPathwayRommAsync(int registrationPathwayId, int pathwayAssessmentId);

        AdminOpenPathwayRommReviewChangesViewModel GetAdminOpenPathwayRommReviewChangesAsync(AdminOpenPathwayRommViewModel openPathwayRommViewModel);

        Task<bool> ProcessAdminOpenPathwayRommAsync(AdminOpenPathwayRommReviewChangesViewModel openPathwayRommReviewChangesViewModel);

        Task<AdminOpenSpecialismRommViewModel> GetAdminOpenSpecialismRommAsync(int registrationPathwayId, int specialismAssessmentId);

        AdminOpenSpecialismRommReviewChangesViewModel GetAdminOpenSpecialismRommReviewChangesAsync(AdminOpenSpecialismRommViewModel openSpecialismRommViewModel);

        Task<bool> ProcessAdminOpenSpecialismRommAsync(AdminOpenSpecialismRommReviewChangesViewModel openSpecialismRommReviewChangesViewModel);

        Task<AdminAddCoreRommOutcomeViewModel> GetAdminAddPathwayRommOutcomeAsync(int registrationPathwayId, int pathwayAssessmentId);

        Task<AdminAddSpecialismRommOutcomeViewModel> GetAdminAddSpecialismRommOutcomeAsync(int registrationPathwayId, int pathwayAssessmentId);

        Task<AdminAddRommOutcomeChangeGradeCoreViewModel> GetAdminAddRommOutcomeChangeGradeCoreAsync(int registrationPathwayId, int assessmentId);
        Task LoadAdminAddRommOutcomeChangeGradeCoreGrades(AdminAddRommOutcomeChangeGradeCoreViewModel model);

        Task<AdminAddRommOutcomeChangeGradeSpecialismViewModel> GetAdminAddRommOutcomeChangeGradeSpecialismAsync(int registrationPathwayId, int assessmentId);
        Task LoadAdminAddRommOutcomeChangeGradeSpecialismGrades(AdminAddRommOutcomeChangeGradeSpecialismViewModel model);

        AdminReviewChangesRommOutcomeCoreViewModel GetAdminReviewChangesRommOutcomeCoreAsync(AdminAddRommOutcomeChangeGradeCoreViewModel revieChangeRommOutcomeCoreViewModel);

        Task<bool> ProcessAdminReviewChangesRommOutcomeCoreAsync(AdminReviewChangesRommOutcomeCoreViewModel revieChangeRommOutcomeCoreViewModel);

        AdminReviewChangesRommOutcomeSpecialismViewModel GetAdminReviewChangesRommOutcomeSpecialismAsync(AdminAddRommOutcomeChangeGradeSpecialismViewModel revieChangeRommOutcomeSpecialismViewModel);

        Task<bool> ProcessAdminReviewChangesRommOutcomeSpecialismAsync(AdminReviewChangesRommOutcomeSpecialismViewModel revieChangeRommOutcomeSpecialismViewModel);
    }
}