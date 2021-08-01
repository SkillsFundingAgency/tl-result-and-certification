﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.PostResultsService;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader
{
    public class PostResultsServiceLoader : IPostResultsServiceLoader
    {
        private readonly IResultsAndCertificationInternalApiClient _internalApiClient;
        private readonly IMapper _mapper;

        public PostResultsServiceLoader(IResultsAndCertificationInternalApiClient internalApiClient, IMapper mapper)
        {
            _internalApiClient = internalApiClient;
            _mapper = mapper;
        }

        public async Task<FindPrsLearnerRecord> FindPrsLearnerRecordAsync(long aoUkprn, long? uln, int? profileId = null)
        {
            return await _internalApiClient.FindPrsLearnerRecordAsync(aoUkprn, uln, profileId);
        }

        public async Task<T> GetPrsLearnerDetailsAsync<T>(long aoUkprn, int profileId, int assessementId)
        {
            var prsLearnerDetails = await _internalApiClient.GetPrsLearnerDetailsAsync(aoUkprn, profileId, assessementId);

            if (typeof(T) == typeof(AppealUpdatePathwayGradeViewModel))
            {
                var grades = await _internalApiClient.GetLookupDataAsync(LookupCategory.PathwayComponentGrade);
                return _mapper.Map<T>(prsLearnerDetails, opt => opt.Items["grades"] = grades);
            }
            else
                return _mapper.Map<T>(prsLearnerDetails);
        }

        public async Task<bool> AppealCoreGradeAsync(long aoUkprn, AppealCoreGradeViewModel model)
        {
            var request = _mapper.Map<AppealGradeRequest>(model, opt => opt.Items["aoUkprn"] = aoUkprn);
            return await _internalApiClient.AppealGradeAsync(request);
        }

        public async Task<bool> AppealCoreGradeAsync(long aoUkprn, PrsPathwayGradeCheckAndSubmitViewModel model)
        {
            var request = _mapper.Map<AppealGradeRequest>(model, opt => opt.Items["aoUkprn"] = aoUkprn);

            // Assign new grade lookup id
            var grades = await _internalApiClient.GetLookupDataAsync(LookupCategory.PathwayComponentGrade);
            var newGrade = grades.FirstOrDefault(x => x.Value.Equals(model.NewGrade, StringComparison.InvariantCultureIgnoreCase));
            if (newGrade == null)
                return false;
            request.ResultLookupId = newGrade.Id;

            return await _internalApiClient.AppealGradeAsync(request);
        }

        public T TransformLearnerDetailsTo<T>(FindPrsLearnerRecord prsLearnerRecord)
        {
            return _mapper.Map<T>(prsLearnerRecord);
        }

        public async Task<bool> WithdrawAppealCoreGradeAsync(long aoUkprn, AppealOutcomePathwayGradeViewModel model)
        {
            var request = _mapper.Map<AppealGradeRequest>(model, opt => opt.Items["aoUkprn"] = aoUkprn);
            return await _internalApiClient.AppealGradeAsync(request);
        }
    }
}