﻿using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.Content.AdminPostResults;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.BackLink;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Summary.SummaryItem;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminPostResults
{
    public class AdminOpenPathwayAppealViewModel
    {
        public int RegistrationPathwayId { get; set; }

        public int PathwayAssessmentId { get; set; }

        public int PathwayResultId { get; set; }

        public string PathwayName { get; set; }

        #region Personal details

        public string Learner { get; set; }

        public long Uln { get; set; }

        public string Provider { get; set; }

        public string Tlevel { get; set; }

        public string StartYear { get; set; }

        public SummaryItemModel SummaryLearner
            => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_Learner_Id, AdminOpenPathwayAppeal.Summary_Learner_Text, Learner);

        public SummaryItemModel SummaryUln
            => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_ULN_Id, AdminOpenPathwayAppeal.Summary_ULN_Text, Uln.ToString());

        public SummaryItemModel SummaryProvider
            => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_Provider_Id, AdminOpenPathwayAppeal.Summary_Provider_Text, Provider);

        public SummaryItemModel SummaryTlevel
            => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_TLevel_Id, AdminOpenPathwayAppeal.Summary_TLevel_Text, Tlevel);

        public SummaryItemModel SummaryStartYear
            => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_StartYear_Id, AdminOpenPathwayAppeal.Summary_StartYear_Text, StartYear);

        #endregion

        #region Assessment

        public string ExamPeriod { get; set; }

        public string Grade { get; set; }

        public SummaryItemModel SummaryExamPeriod
           => CreateSummaryItemModel(AdminOpenPathwayAppeal.Summary_Exam_Period_Id, AdminOpenPathwayAppeal.Summary_Exam_Period_Text, ExamPeriod);

        public SummaryItemModel SummaryGrade
           => CreateSummaryItemModel(
               AdminOpenPathwayAppeal.Summary_Grade_Id,
               AdminOpenPathwayAppeal.Summary_Grade_Text,
               string.IsNullOrWhiteSpace(Grade) ? AdminOpenPathwayAppeal.No_Grade_Entered : Grade);

        #endregion

        public bool IsValid
            => string.IsNullOrEmpty(ErrorMessage);

        public string ErrorMessage { get; set; }

        [Required(ErrorMessageResourceType = typeof(AdminOpenPathwayAppeal), ErrorMessageResourceName = "Validation_Message")]
        public bool? DoYouWantToOpenAppeal { get; set; }

        public BackLinkModel BackLink => new()
        {
            RouteName = RouteConstants.AdminLearnerRecord,
            RouteAttributes = new Dictionary<string, string> { { Constants.PathwayId, RegistrationPathwayId.ToString() } }
        };

        private static SummaryItemModel CreateSummaryItemModel(string id, string title, string value)
            => new()
            {
                Id = id,
                Title = title,
                Value = value
            };
    }
}