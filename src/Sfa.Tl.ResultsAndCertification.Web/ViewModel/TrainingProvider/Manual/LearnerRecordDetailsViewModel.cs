﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.BackLink;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.NotificationBanner;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Summary.SummaryItem;
using System;
using System.Collections.Generic;
using LearnerRecordDetailsContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.LearnerRecordDetails;
using SubjectStatusContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.SubjectStatus;
using IpStatus = Sfa.Tl.ResultsAndCertification.Common.Enum.IndustryPlacementStatus;
using IndustryPlacementStatusContent = Sfa.Tl.ResultsAndCertification.Web.Content.TrainingProvider.IndustryPlacementStatus;
using Sfa.Tl.ResultsAndCertification.Models.OverallResults;
using System.Linq;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.ProviderAddress;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual
{
    public class LearnerRecordDetailsViewModel
    {
        // Header
        public int ProfileId { get; set; }
        public int RegistrationPathwayId { get; set; }
        public int TlPathwayId { get; set; }
        public long Uln { get; set; }
        public string LearnerName { get; set; }
        public DateTime DateofBirth { get; set; }
        public string ProviderName { get; set; }
        public long ProviderUkprn { get; set; }
        public string TlevelTitle { get; set; }        
        public int AcademicYear { get; set; }
        public string AwardingOrganisationName { get; set; }
        public SubjectStatus MathsStatus { get; set; }
        public SubjectStatus EnglishStatus { get; set; }

        public int IndustryPlacementId { get; set; }
        public IpStatus IndustryPlacementStatus { get; set; }

        public OverallResultDetail OverallResultDetails { get; set; }
        public DateTime? OverallResultPublishDate { get; set; }

        // PrintCertificate Info
        public int? PrintCertificateId { get; set; }
        public PrintCertificateType? PrintCertificateType { get; set; }        
        public AddressViewModel ProviderAddress { get; set; }

        public string StartYear => string.Format(LearnerRecordDetailsContent.Start_Year_Value, AcademicYear, AcademicYear + 1);

        /// <summary>
        /// True when status is Active or Withdrawn
        /// </summary>
        public bool IsLearnerRegistered { get; set; }
        public bool IsStatusCompleted => IsMathsAdded && IsEnglishAdded && IsIndustryPlacementAdded;
        public bool IsIndustryPlacementAdded => IndustryPlacementStatus != IpStatus.NotSpecified;
        public bool IsMathsAdded => MathsStatus != SubjectStatus.NotSpecified;
        public bool IsEnglishAdded => EnglishStatus != SubjectStatus.NotSpecified;
        public bool CanAddIndustryPlacement => IndustryPlacementStatus == IpStatus.NotSpecified || IndustryPlacementStatus == IpStatus.NotCompleted;
        public bool DisplayOverallResults => OverallResultDetails != null && OverallResultPublishDate.HasValue && DateTime.UtcNow >= OverallResultPublishDate;
        public NotificationBannerModel SuccessBanner { get; set; }

        #region Summary Header
        public SummaryItemModel SummaryDateofBirth => new SummaryItemModel
        {
            Id = "dateofbirth",
            Title = LearnerRecordDetailsContent.Title_DateofBirth_Text,
            Value = DateofBirth.ToDobFormat()
        };

        public SummaryItemModel SummaryProviderName => new SummaryItemModel
        {
            Id = "providername",
            Title = LearnerRecordDetailsContent.Title_Provider_Name_Text,
            Value = ProviderName,
        };

        public SummaryItemModel SummaryProviderUkprn => new SummaryItemModel
        {
            Id = "providerukprn",
            Title = LearnerRecordDetailsContent.Title_Provider_Ukprn_Text,
            Value = ProviderUkprn.ToString(),
        };

        public SummaryItemModel SummaryTlevelTitle => new SummaryItemModel
        {
            Id = "tleveltitle",
            Title = LearnerRecordDetailsContent.Title_TLevel_Text,
            Value = TlevelTitle
        };

        public SummaryItemModel SummaryStartYear => new SummaryItemModel
        {
            Id = "startyear",
            Title = LearnerRecordDetailsContent.Title_StartYear_Text,
            Value = StartYear
        };

        public SummaryItemModel SummaryAoName => new SummaryItemModel
        {
            Id = "aoname",
            Title = LearnerRecordDetailsContent.Title_AoName_Text,
            Value = AwardingOrganisationName
        };

        #endregion

        # region Summary English & Maths
        public SummaryItemModel SummaryMathsStatus => IsMathsAdded ?
            new SummaryItemModel
            {
                Id = "mathsstatus",
                Title = LearnerRecordDetailsContent.Title_Maths_Text,
                Value = GetSubjectStatus(MathsStatus),
            }
            : new SummaryItemModel
            {
                Id = "mathsstatus",
                Title = LearnerRecordDetailsContent.Title_Maths_Text,
                Value = GetSubjectStatus(MathsStatus),
                ActionText = LearnerRecordDetailsContent.Action_Text_Link_Add,
                RouteName = IsMathsAdded ? string.Empty : RouteConstants.AddMathsStatus,
                RouteAttributes = IsMathsAdded ? null : new Dictionary<string, string> { { Constants.ProfileId, ProfileId.ToString() } },
                HiddenActionText = LearnerRecordDetailsContent.Hidden_Action_Text_Maths
            };

        public SummaryItemModel SummaryEnglishStatus => IsEnglishAdded ?
            new SummaryItemModel
            {
                Id = "englishstatus",
                Title = LearnerRecordDetailsContent.Title_English_Text,
                Value = GetSubjectStatus(EnglishStatus),
            }
            : new SummaryItemModel
            {
                Id = "englishstatus",
                Title = LearnerRecordDetailsContent.Title_English_Text,
                Value = GetSubjectStatus(EnglishStatus),
                ActionText = LearnerRecordDetailsContent.Action_Text_Link_Add,
                HiddenActionText = LearnerRecordDetailsContent.Hidden_Action_Text_English,
                RouteName = IsEnglishAdded ? string.Empty : RouteConstants.AddEnglishStatus,
                RouteAttributes = IsEnglishAdded ? null : new Dictionary<string, string> { { Constants.ProfileId, ProfileId.ToString() } },
            };

        #endregion

        // Industry Placement
        public SummaryItemModel SummaryIndustryPlacementStatus => CanAddIndustryPlacement ?
            new SummaryItemModel
            {
                Id = "industryplacement",
                Title = LearnerRecordDetailsContent.Title_IP_Status_Text,
                Value = GetIndustryPlacementDisplayText,
                ActionText = LearnerRecordDetailsContent.Action_Text_Link_Add,
                RouteName = CanAddIndustryPlacement ? RouteConstants.AddIndustryPlacement : string.Empty,
                RouteAttributes = CanAddIndustryPlacement ? new Dictionary<string, string> { { Constants.ProfileId, ProfileId.ToString() } } : null,
                HiddenActionText = LearnerRecordDetailsContent.Hidden_Action_Text_Industry_Placement
            }
            :
            new SummaryItemModel
            {
                Id = "industryplacement",
                Title = LearnerRecordDetailsContent.Title_IP_Status_Text,
                Value = GetIndustryPlacementDisplayText,
            };

        // Overall Result

        public SummaryItemModel SummaryCoreResult => new SummaryItemModel
        {
            Id = "coreResult",
            Title = OverallResultDetails?.PathwayName,
            Value = OverallResultDetails?.PathwayResult
        };

        public SummaryItemModel SummarySpecialismResult => new SummaryItemModel
        {
            Id = "specialismResult",
            Title = HasSpecialismInfo ? OverallResultDetails.SpecialismDetails.FirstOrDefault().SpecialismName : null ,
            Value = HasSpecialismInfo ? OverallResultDetails.SpecialismDetails.FirstOrDefault().SpecialismResult : null
        };

        public SummaryItemModel SummaryOverallResult => new SummaryItemModel
        {
            Id = "overallResult",
            Title = LearnerRecordDetailsContent.Title_OverallResult_Text,
            Value = OverallResultDetails?.OverallResult
        };

        public DateTime? LastPrintRequestedDate { get; set; }

        public bool IsDocumentReprintEligible { get; set; }

        public BackLinkModel BackLink => new()
        {
            RouteName = RouteConstants.SearchLearnerRecord
        };

        private bool HasSpecialismInfo => DisplayOverallResults && OverallResultDetails.SpecialismDetails != null && OverallResultDetails.SpecialismDetails.Any();

        private static string GetSubjectStatus(SubjectStatus subjectStatus)
        {
            return subjectStatus switch
            {
                SubjectStatus.Achieved => SubjectStatusContent.Achieved_Display_Text,
                SubjectStatus.NotAchieved => SubjectStatusContent.Not_Achieved_Display_Text,
                SubjectStatus.AchievedByLrs => SubjectStatusContent.Achieved_Lrs_Display_Text,
                SubjectStatus.NotAchievedByLrs => SubjectStatusContent.Not_Achieved_Lrs_Display_Text,
                _ => SubjectStatusContent.Not_Yet_Recevied_Display_Text,
            };
        }

        private string GetIndustryPlacementDisplayText => IndustryPlacementStatus switch
        {
            IpStatus.Completed => IndustryPlacementStatusContent.Completed_Display_Text,
            IpStatus.CompletedWithSpecialConsideration => IndustryPlacementStatusContent.CompletedWithSpecialConsideration_Display_Text,
            IpStatus.NotCompleted => IndustryPlacementStatusContent.Still_To_Be_Completed_Display_Text,
            _ => IndustryPlacementStatusContent.Not_Yet_Received_Display_Text,
        };
    }
}