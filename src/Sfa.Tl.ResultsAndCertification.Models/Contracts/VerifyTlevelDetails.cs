﻿
namespace Sfa.Tl.ResultsAndCertification.Models.Contracts
{
    public class VerifyTlevelDetails : BaseModel
    {
        public int TqAwardingOrganisationId { get; set; }
        public int PathwayStatusId { get; set; }    
        public string Query{ get; set; }
        public string QueriedUserEmail { get; set; }
        public string ModifiedBy { get; set; }
    }
}
