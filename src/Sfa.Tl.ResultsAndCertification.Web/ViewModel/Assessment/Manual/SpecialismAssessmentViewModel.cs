﻿using System;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.Assessment.Manual
{
    public class SpecialismAssessmentViewModel
    {
        public int AssessmentId { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
