﻿using Sfa.Tl.ResultsAndCertification.Common.Utils.Ranges;

namespace Sfa.Tl.ResultsAndCertification.Models.Configuration
{
    public class AnalystCoreResultExtractSettings
    {
        /// <summary>
        /// Gets or sets the academic years to process.
        /// </summary>
        /// <value>
        /// The academic years to process.
        /// </value>
        public int[] CoreAcademicYearsToProcess { get; set; }

        /// <summary>
        /// Gets or sets the valid date ranges to run the process.
        /// </summary>
        /// <value>
        /// The valid date ranges to run the process.
        /// </value>
        public DateTimeRange[] CoreValidDateRanges { get; set; }
    }
}
