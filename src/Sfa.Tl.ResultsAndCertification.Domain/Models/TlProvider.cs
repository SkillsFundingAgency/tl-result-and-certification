﻿using System.Collections.Generic;

namespace Sfa.Tl.ResultsAndCertification.Domain.Models
{
    public partial class TlProvider : BaseEntity
    {
        public TlProvider()
        {
            TqProviders = new HashSet<TqProvider>();
            TlProviderAddresses = new HashSet<TlProviderAddress>();
            DocumentUploadHistorys = new HashSet<DocumentUploadHistory>();
        }

        public long UkPrn { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TqProvider> TqProviders { get; set; }
        public virtual ICollection<TlProviderAddress> TlProviderAddresses { get; set; }
        public virtual ICollection<DocumentUploadHistory> DocumentUploadHistorys { get; set; }
    }
}