using System;
using System.Net.Http;
using System.Threading.Tasks;

using Polly;

namespace Resilient.HttpClient
{
    public class ResilienceSettings
    {
        public CircuitBreakerSettings? CircuitBreaker { get; }
        public RetrySettings? Retry { get; }

        public ResilienceSettings(CircuitBreakerSettings? circuitBreaker = null, 
                                  RetrySettings? retry = null)
        {
            CircuitBreaker = circuitBreaker;
            Retry = retry;
        }

        public class CircuitBreakerSettings
        {
            public CircuitBreakerSettings(int circuitBreakerAllowedAttempts,
                                          TimeSpan circuitBreakerDuration,
                                          Action<Exception, TimeSpan>? onBreak = null,
                                          Action? onReset = null,
                                          Action? onHalfOpen = null)
            {
                CircuitBreakerAllowedAttempts = circuitBreakerAllowedAttempts;
                CircuitBreakerDuration = circuitBreakerDuration;
                OnBreak = onBreak ?? ((_, _) => { });
                OnReset = onReset ?? (() => {});
                OnHalfOpen = onHalfOpen ?? (() => { });
            }

            public int CircuitBreakerAllowedAttempts { get; }
            public TimeSpan CircuitBreakerDuration { get; }
            public Action<Exception, TimeSpan>? OnBreak { get; }
            public Action? OnReset { get; }
            public Action? OnHalfOpen { get; }
        }

        public class RetrySettings
        {
            public RetrySettings(int retryCount,
                                 TimeSpan retrySeedDelay,
                                 TimeSpan retryDelay,
                                 Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task>? onRetryAsync = null)
            {
                RetryCount = retryCount;
                RetrySeedDelay = retrySeedDelay;
                RetryDelay = retryDelay;

                OnRetryAsync = onRetryAsync ?? ((_, _, _, _) => Task.CompletedTask);
            }

            public int RetryCount { get; }
            public TimeSpan RetrySeedDelay { get; }
            public TimeSpan RetryDelay { get; }

            public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetryAsync { get; }
        }
    }
}