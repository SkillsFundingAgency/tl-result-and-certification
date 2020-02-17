﻿using System.Linq;
using System.Security.Claims;

namespace Sfa.Tl.ResultsAndCertification.Common.Extensions
{
    public static class RolesExtensions
    {
        public const string SiteAdministrator = "Site Administrator";
        public const string TlevelsReviewer = "Tlevels Reviewer";
        public const string CentresEditor = "Centres Editor";

        public static bool HasAccessToService(this ClaimsPrincipal user)
        {
            var hasAccess = user.Claims.SingleOrDefault(c => c.Type == CustomClaimTypes.HasAccessToService)?.Value;
            
            if (bool.TryParse(hasAccess, out var result))
            {
                return result;
            }
            return false;
        }

        public static bool HasSiteAdministratorRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(SiteAdministrator);
        }

        public static bool HasTlevelsReviewerRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(TlevelsReviewer);
        }

        public static bool HasCentresEditorRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(CentresEditor);
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            var userNames = user.Claims.Where(c => c.Type == ClaimTypes.GivenName || c.Type == ClaimTypes.Surname).Select(c => c.Value);
            return string.Join(" ", userNames);
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Upn)?.Value;
        }

        public static long GetUkPrn(this ClaimsPrincipal user)
        {
            var ukprn = user?.Claims?.SingleOrDefault(c => c.Type == CustomClaimTypes.Ukprn)?.Value;
            return !string.IsNullOrWhiteSpace(ukprn) ? long.Parse(ukprn) : 0;
        }
    }
}
