﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Web.Helpers;
using System;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService
{
    public class PrsComponentExamViewModel
    {
        public int ProfileId { get; set; }
        public int AssessmentId { get; set; }
        public string AssessmentSeries { get; set; }
        public string Grade { get; set; }        
        public string LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime RommEndDate { get; set; }
        public DateTime AppealEndDate { get; set; }
        public PrsStatus? PrsStatus { get; set; }
        public ComponentType ComponentType { get; set; }
        public string PrsDisplayText { get { return CommonHelper.GetPrsStatusDisplayText(PrsStatus, AppealEndDate); } }
        public bool IsAddRommAllowed => IsGradeExists && CommonHelper.IsAppealsAllowed(RommEndDate);
        private bool IsGradeExists => AssessmentId > 0 && !string.IsNullOrWhiteSpace(Grade);
    }
}
