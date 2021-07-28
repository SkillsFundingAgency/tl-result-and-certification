﻿using System;

namespace Sfa.Tl.ResultsAndCertification.Domain.Models
{
    public partial class AssessmentSeries : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AppealEndDate { get; set; }

        //public virtual TlAwardingOrganisation TlAwardingOrganisation { get; set; }
    }
}
