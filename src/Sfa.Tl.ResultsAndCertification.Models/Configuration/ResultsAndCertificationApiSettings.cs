﻿namespace Sfa.Tl.ResultsAndCertification.Models.Configuration
{
    public class ResultsAndCertificationApiSettings
    {
        public string InternalApiUri { get; set; }

        public string InternalApiIssuer { get; set; }

        public string InternalApiSecret { get; set; }

        public int InternalApiTokenExpiryTime { get; set; }
    }
}
