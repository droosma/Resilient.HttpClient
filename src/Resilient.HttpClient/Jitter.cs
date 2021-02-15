using System;
using System.Collections.Generic;

namespace Resilient.HttpClient
{
    internal static class Jitter
    {
        public static IEnumerable<TimeSpan> Decorrelated(int maxRetries, TimeSpan seedDelay, TimeSpan maxDelay)
        {
            var jitterer = new Random();

            var seedDelayTotalMilliseconds = seedDelay.TotalMilliseconds;
            var maxDelayTotalMilliseconds = maxDelay.TotalMilliseconds;
            var current = seedDelayTotalMilliseconds;

            var timespans = new List<TimeSpan>();
            for(var attempt = 0;attempt < maxRetries;attempt++)
            {
                var cap = Math.Max(seedDelayTotalMilliseconds, current * 3 * jitterer.NextDouble());
                current = Math.Min(maxDelayTotalMilliseconds, cap);
                timespans.Add(TimeSpan.FromMilliseconds(current));
            }

            return timespans;
        }
    }
}