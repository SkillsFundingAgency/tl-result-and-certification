﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using System;

namespace Sfa.Tl.ResultsAndCertification.Models.Contracts
{
    public class AssessmentSeriesDetails
    {
        public int Id { get; set; }
        public ComponentType ComponentType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AppealEndDate { get; set; }
        public DateTime ResultPublishDate { get; set; }
        public int ResultCalculationYear { get { return Year - 2; } } // TODO: TempCode.

    }
}
