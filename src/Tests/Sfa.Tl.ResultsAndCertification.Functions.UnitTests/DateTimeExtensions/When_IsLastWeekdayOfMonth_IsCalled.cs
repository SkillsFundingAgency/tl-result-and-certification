﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using System;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Functions.UnitTests.DateTimeExtensions
{
    public class When_IsLastWeekdayOfMonth_IsCalled : TestSetup
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Then_Returns_Expected_Results(DateTime inputDate, DayOfWeek dayOfWeek, bool isValid)
        {
            var result = inputDate.IsLastWeekdayOfMonth(dayOfWeek);

            result.Should().Be(isValid);
        }
    }
}