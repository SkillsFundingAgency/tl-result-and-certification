﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.Interfaces
{
    public interface IRegistrationController
    {
        Task<RegistrationDetails> GetRegistrationDetailsAsync(long aoUkprn, int profileId, RegistrationPathwayStatus? status = null);
        Task<bool> AddRegistrationAsync(RegistrationRequest model);
        Task<bool> UpdateRegistrationAsync(ManageRegistration model);
        Task<bool> DeleteRegistrationAsync(long aoUkprn, int profileId);
        Task<bool> WithdrawRegistrationAsync(WithdrawRegistrationRequest model);
        Task<bool> RejoinRegistrationAsync(RejoinRegistrationRequest model);

        // Bulk process
        Task<BulkRegistrationResponse> ProcessBulkRegistrationsAsync(BulkRegistrationRequest request);
    }
}
