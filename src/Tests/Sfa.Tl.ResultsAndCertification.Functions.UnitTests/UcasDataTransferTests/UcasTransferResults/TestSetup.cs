﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Functions.UnitTests.UcasDataTransferTests.UcasTransferResults
{
    public abstract class TestSetup : UcasDataTransferTestBase
    {
        public async override Task When()
        {
            await UcasDataTransferFunction.UcasTransferResultsAsync(new TimerInfo(TimerSchedule, new ScheduleStatus()), new ExecutionContext(), new NullLogger<UcasDataTransfer>());
        }
    }
}
