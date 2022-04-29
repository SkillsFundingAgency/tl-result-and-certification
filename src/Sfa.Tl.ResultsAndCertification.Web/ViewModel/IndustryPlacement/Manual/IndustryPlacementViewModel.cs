﻿namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement.Manual
{
    public class IndustryPlacementViewModel
    {
        public IpCompletionViewModel IpCompletion { get; set; }
        public IpModelUsedViewModel IpModelUsed { get; set; }
        public SpecialConsiderationViewModel SpecialConsideration { get; set; }
    }

    public class SpecialConsiderationViewModel
    {
        public SpecialConsiderationViewModel()
        {
            PlacementHours = new PlacementHoursViewModel();
        }

        public PlacementHoursViewModel PlacementHours { get; set; }
    }
}