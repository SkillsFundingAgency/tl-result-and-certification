﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.BackLink;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ErrorResource = Sfa.Tl.ResultsAndCertification.Web.Content.IndustryPlacement;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement.Manual
{
    public class IpCompletionViewModel
    {
        public int ProfileId { get; set; }
        public int RegistrationPathwayId { get; set; }
        public int PathwayId { get; set; }
        public int AcademicYear { get; set; }
        public string LearnerName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorResource.IpCompletion), ErrorMessageResourceName = "Validation_Message")]
        public IndustryPlacementStatus? IndustryPlacementStatus { get; set; }

        public bool IsChangeJourney { get; set; }
        public bool IsChangeMode { get; set; }

        public string PageTitle { get; set; }

        public bool IsValid
        {
            get
            {
                var hasStatusAlready = EnumExtensions.IsValidValue<IndustryPlacementStatus>(IndustryPlacementStatus, exclNotSpecified: true);
                return IsChangeJourney == hasStatusAlready;
            }
        }
        public bool IsIpStatusExists => IndustryPlacementStatus != null && IndustryPlacementStatus != ResultsAndCertification.Common.Enum.IndustryPlacementStatus.NotSpecified;
        
        public virtual BackLinkModel BackLink => new()
        {
            RouteName = IsChangeMode ? RouteConstants.IpCheckAndSubmit : RouteConstants.LearnerRecordDetails,
            RouteAttributes = IsChangeMode ? null : new Dictionary<string, string> { { Constants.ProfileId, ProfileId.ToString() } }
        };
    }
}