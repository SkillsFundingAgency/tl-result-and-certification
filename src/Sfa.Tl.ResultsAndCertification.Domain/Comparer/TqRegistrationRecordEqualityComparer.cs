﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Tl.ResultsAndCertification.Domain.Comparer
{
    public class TqRegistrationRecordEqualityComparer : IEqualityComparer<TqRegistrationProfile>
    {
        public bool Equals(TqRegistrationProfile x, TqRegistrationProfile y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.GetType() != y.GetType())
                return false;
            else
            {
                var retVal = x.UniqueLearnerNumber == y.UniqueLearnerNumber
                            && string.Equals(x.Firstname, y.Firstname)
                            && Equals(x.Lastname, y.Lastname)
                            && Equals(x.DateofBirth, y.DateofBirth)
                            && ArePathwaysAndSpecialismsEqual(x, y);

                return retVal;
            }
        }

        public int GetHashCode(TqRegistrationProfile reg)
        {
            unchecked
            {
                var hashCode = reg.UniqueLearnerNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ (reg.Firstname != null ? reg.Firstname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (reg.Lastname != null ? reg.Lastname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ reg.DateofBirth.GetHashCode();

                foreach (var registrationPathway in reg.TqRegistrationPathways.Where(p => p.Status == RegistrationPathwayStatus.Active).OrderBy(p => p.TqProviderId))
                {
                    hashCode = (hashCode * 397) ^ registrationPathway.TqProviderId.GetHashCode();
                    hashCode = (hashCode * 397) ^ registrationPathway.AcademicYear.GetHashCode();
                    hashCode = (hashCode * 397) ^ registrationPathway.Status.GetHashCode();

                    foreach (var registrationSpecialism in registrationPathway.TqRegistrationSpecialisms.Where(p => p.IsOptedin && p.EndDate == null).OrderBy(p => p.TlSpecialismId))
                    {
                        hashCode = (hashCode * 397) ^ registrationSpecialism.TlSpecialismId.GetHashCode();
                        hashCode = (hashCode * 397) ^ registrationSpecialism.IsOptedin.GetHashCode();
                    }
                }
                return hashCode;
            }
        }

        private bool ArePathwaysAndSpecialismsEqual(TqRegistrationProfile x, TqRegistrationProfile y)
        {
            var xPathways = x.TqRegistrationPathways.Where(p => p.Status == RegistrationPathwayStatus.Active);
            var yPathways = y.TqRegistrationPathways.Where(p => p.Status == RegistrationPathwayStatus.Active);
            
            bool isPathwayEqual = (xPathways.Count() == yPathways.Count());

            if (isPathwayEqual)
            {
                foreach (var xpathway in xPathways)
                {
                    var ypathway = yPathways.FirstOrDefault(p => p.TqProviderId == xpathway.TqProviderId);
                    isPathwayEqual = EqualsTqRegistrationPathway(xpathway, ypathway);

                    if (!isPathwayEqual) break;

                    bool areSpecialismEqual = AreSpecialismsEqual(xpathway, ypathway);

                    if (!isPathwayEqual || !areSpecialismEqual)
                    {
                        isPathwayEqual = false;
                        break;
                    }
                }
            }
            return isPathwayEqual;
        }

        private bool AreSpecialismsEqual(TqRegistrationPathway xpathway, TqRegistrationPathway ypathway)
        {
            var xSpecialisms = xpathway.TqRegistrationSpecialisms.Where(s => s.IsOptedin && s.EndDate == null);
            var ySpecialisms = ypathway.TqRegistrationSpecialisms.Where(s => s.IsOptedin && s.EndDate == null);

            var isSpecialismEqual = (xSpecialisms.Count() == ySpecialisms.Count());
            if (isSpecialismEqual)
            {
                foreach (var xspecialism in xSpecialisms)
                {
                    var yspecialism = ySpecialisms.FirstOrDefault(s => s.TlSpecialismId == xspecialism.TlSpecialismId);
                    isSpecialismEqual = EqualsTqRegistrationSpecialism(xspecialism, yspecialism);

                    if (!isSpecialismEqual) break;
                }
            }
            return isSpecialismEqual;
        }

        private bool EqualsTqRegistrationPathway(TqRegistrationPathway x, TqRegistrationPathway y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.GetType() != y.GetType())
                return false;
            else
            {
                return x.TqProviderId == y.TqProviderId && Equals(x.AcademicYear, y.AcademicYear) && x.Status == y.Status;
            }
        }

        private bool EqualsTqRegistrationSpecialism(TqRegistrationSpecialism x, TqRegistrationSpecialism y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.GetType() != y.GetType())
                return false;
            else
            {
                return x.TlSpecialismId == y.TlSpecialismId && x.IsOptedin == y.IsOptedin;
            }
        }
    }
}
