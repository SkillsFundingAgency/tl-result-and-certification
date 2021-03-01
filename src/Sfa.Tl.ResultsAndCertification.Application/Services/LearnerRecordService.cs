﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Models.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Application.Services
{
    public class LearnerRecordService : ILearnerRecordService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRegistrationRepository _tqRegistrationRepository;
        private readonly IRepository<Qualification> _qualificationRepository;

        public LearnerRecordService(IMapper mapper, ILogger<ILearnerRecordService> logger,
            IRegistrationRepository tqRegistrationRepository,
            IRepository<Qualification> qualificationRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _tqRegistrationRepository = tqRegistrationRepository;
            _qualificationRepository = qualificationRepository;
        }
        
        public async Task<IList<RegistrationLearnerDetails>> GetPendingVerificationAndLearningEventsLearnersAsync()
        {
            var registrationLearners = await _tqRegistrationRepository.GetManyAsync(r => r.IsLearnerVerified == null || r.IsLearnerVerified.Value == false ||
                                                                     ((r.IsEnglishAndMathsAchieved == null || r.IsEnglishAndMathsAchieved.Value == false) &&
                                                                     (r.IsRcFeed == null || r.IsRcFeed.Value == false))).ToListAsync();

            if (registrationLearners == null) return null;

            return _mapper.Map<IList<RegistrationLearnerDetails>>(registrationLearners);
        }

        public async Task<LearnerVerificationAndLearningEventsResponse> ProcessLearnerRecordsAsync(List<LearnerRecordDetails> learnerRecords)
        {
            if (learnerRecords == null || !learnerRecords.Any())
            {
                var message = $"No learners data retrived from LRS to process. Method: ProcessLearnerRecords()";
                _logger.LogWarning(LogEvent.NoDataFound, message);
                return new LearnerVerificationAndLearningEventsResponse { IsSuccess = true, Message = message };
            }

            var registrationProfilesToProcessLrsData = new List<TqRegistrationProfile>();
            var qualifications = await GetAllQualifications();
            var registrationProfiles = await GetRegistrationProfilesByIds(learnerRecords.Select(x => x.ProfileId).ToList());

            foreach (var learnerRecord in learnerRecords)
            {
                var registrationProfile = registrationProfiles.FirstOrDefault(p => p.Id == learnerRecord.ProfileId);

                if (IsValidLearner(registrationProfile))
                {
                    ProcessLearningEvents(qualifications, learnerRecord);

                    var modifiedProfile = ProcessProfileAndQualificationsAchieved(learnerRecord, registrationProfile);

                    if (modifiedProfile != null)
                        registrationProfilesToProcessLrsData.Add(modifiedProfile);
                }
            }

            if(registrationProfilesToProcessLrsData.Any())
            {
                var isSuccess = await _tqRegistrationRepository.UpdateManyAsync(registrationProfilesToProcessLrsData) > 0;
                return new LearnerVerificationAndLearningEventsResponse { IsSuccess = isSuccess, LrsRecordsCount = learnerRecords.Count(), ModifiedRecordsCount = registrationProfilesToProcessLrsData.Count(), SavedRecordsCount = registrationProfilesToProcessLrsData.Count() };
            }
            else
            {
                return new LearnerVerificationAndLearningEventsResponse { IsSuccess = true, LrsRecordsCount = learnerRecords.Count(), ModifiedRecordsCount = 0, SavedRecordsCount = 0 };
            }
        }

        private static void ProcessLearningEvents(List<Qualification> qualifications, LearnerRecordDetails learnerRecord)
        {
            if (learnerRecord.IsLearnerVerified == false) return;

            foreach (var learnerEvent in learnerRecord.LearningEventDetails)
            {
                var qualification = qualifications.FirstOrDefault(q => q.IsActive && q.Code.Equals(learnerEvent.QualificationCode, StringComparison.InvariantCultureIgnoreCase));
                var qualificationGrade = qualification?.QualificationType?.QualificationGrades?.FirstOrDefault(g => g.IsActive && g.Grade.Equals(learnerEvent.Grade, StringComparison.InvariantCultureIgnoreCase));

                if (qualification != null && qualificationGrade != null)
                {
                    learnerEvent.IsQualificationAllowed = true;
                    learnerEvent.IsAchieved = qualificationGrade.IsAllowable;
                    learnerEvent.QualificationGradeId = qualificationGrade.Id;
                    learnerEvent.QualificationId = qualification.Id;
                    learnerEvent.IsEnglishSubject = qualification.TlLookup?.Code.Equals("Eng", StringComparison.InvariantCultureIgnoreCase) ?? false;
                    learnerEvent.IsMathsSubject = qualification.TlLookup?.Code.Equals("Math", StringComparison.InvariantCultureIgnoreCase) ?? false;
                }
            }
        }

        private static TqRegistrationProfile ProcessProfileAndQualificationsAchieved(LearnerRecordDetails learnerRecord, TqRegistrationProfile profile)
        {
            if (learnerRecord.IsLearnerVerified == false && learnerRecord.IsLearnerVerified == profile.IsLearnerVerified)
                return null;

            var isProfileChanged = false;
            var learnerLearningEvents = learnerRecord.LearningEventDetails.Where(x => x.IsQualificationAllowed);

            if (learnerRecord.IsLearnerVerified != profile.IsLearnerVerified)
            {
                profile.IsLearnerVerified = learnerRecord.IsLearnerVerified;
                profile.ModifiedOn = DateTime.UtcNow;
                profile.ModifiedBy = learnerRecord.PerformedBy;
                isProfileChanged = true;
            }

            if (!profile.IsEnglishAndMathsAchieved.HasValue || !profile.IsEnglishAndMathsAchieved.Value)
            {
                if (learnerLearningEvents.Any())
                {
                    var isQualificationAchievedChanged = false;

                    foreach (var learnerLearningEvent in learnerLearningEvents)
                    {
                        var existingQualificationAchieved = profile.QualificationAchieved.FirstOrDefault(q => q.QualificationId == learnerLearningEvent.QualificationId);

                        if (existingQualificationAchieved != null)
                        {
                            if (existingQualificationAchieved.QualificationGradeId != learnerLearningEvent.QualificationGradeId || existingQualificationAchieved.IsAchieved != learnerLearningEvent.IsAchieved)
                            {
                                existingQualificationAchieved.QualificationGradeId = learnerLearningEvent.QualificationGradeId;
                                existingQualificationAchieved.IsAchieved = learnerLearningEvent.IsAchieved;
                                existingQualificationAchieved.ModifiedBy = learnerRecord.PerformedBy;
                                existingQualificationAchieved.ModifiedOn = DateTime.UtcNow;
                                isQualificationAchievedChanged = true;
                            }
                        }
                        else
                        {
                            profile.QualificationAchieved.Add(new QualificationAchieved
                            {
                                TqRegistrationProfileId = profile.Id,
                                QualificationId = learnerLearningEvent.QualificationId,
                                QualificationGradeId = learnerLearningEvent.QualificationGradeId,
                                IsAchieved = learnerLearningEvent.IsAchieved,
                                CreatedBy = learnerRecord.PerformedBy
                            });
                            isQualificationAchievedChanged = true;
                        }
                    }

                    if (isQualificationAchievedChanged)
                    {
                        profile.IsEnglishAndMathsAchieved = learnerLearningEvents.Any(e => e.IsAchieved && e.IsEnglishSubject) && learnerLearningEvents.Any(e => e.IsAchieved && e.IsMathsSubject);
                        profile.IsRcFeed = false;
                        profile.ModifiedOn = DateTime.UtcNow;
                        profile.ModifiedBy = learnerRecord.PerformedBy;
                        isProfileChanged = true;
                    }
                }
            }

            return isProfileChanged ? profile : null;
        }

        private bool IsValidLearner(TqRegistrationProfile profile)
        {
            return profile != null && (profile.IsRcFeed == null || profile.IsRcFeed.Value == false);
        }

        private bool HasAnyChangesForProfile(TqRegistrationProfile profile, IEnumerable<LearningEventDetails> learningEventDetails)
        {
            if (profile == null || learningEventDetails == null) return false;

            var hasEnglishAndMatchAchieved = learningEventDetails.Any(e => e.IsAchieved && e.IsEnglishSubject) && learningEventDetails.Any(e => e.IsAchieved && e.IsEnglishSubject);

            var anyChanges = profile.IsEnglishAndMathsAchieved != hasEnglishAndMatchAchieved || profile.QualificationAchieved.Count() != learningEventDetails.Count() || profile.IsRcFeed != false;

            return true;
        }

        private async Task<List<TqRegistrationProfile>> GetRegistrationProfilesByIds(List<int> profileIds)
        {
            return await _tqRegistrationRepository.GetManyAsync(p => profileIds.Contains(p.Id), p => p.QualificationAchieved).ToListAsync();
        }

        private async Task<List<Qualification>> GetAllQualifications()
        {
            return await _qualificationRepository.GetManyAsync(x => x.QualificationType.IsActive && x.IsActive, x => x.QualificationType, x => x.QualificationType.QualificationGrades, x => x.TlLookup).ToListAsync();
        }
    }
}
