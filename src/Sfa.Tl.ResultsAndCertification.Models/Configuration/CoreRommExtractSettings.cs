﻿using Newtonsoft.Json;
using Sfa.Tl.ResultsAndCertification.Common.Utils.Ranges;
using Sfa.Tl.ResultsAndCertification.Models.JsonConverter;

namespace Sfa.Tl.ResultsAndCertification.Models.Configuration
{
    public class CoreRommExtractSettings
    {
        /// <summary>
        /// Gets or sets the assessment series years to process.
        /// </summary>
        /// <value>
        /// The academic years to process.
        /// </value>
        public int[] AssessmentSeriesYearsToProcess { get; set; }

        /// <summary>
        /// Gets or sets the valid date ranges to run the process.
        /// </summary>
        /// <value>
        /// The valid date ranges to run the process.
        /// </value>
        [JsonConverter(typeof(StringToDateTimeRangeArrayJsonConverter))]
        public DateTimeRange[] CoreRommValidDateRanges { get; set; }
    }
}