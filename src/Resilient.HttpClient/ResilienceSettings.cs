using System;
using System.Net.Http;
using System.Threading.Tasks;

using Polly;

namespace Resilient.HttpClient
{
    public class ResilienceSettings
    {
        public ResilienceSettings(CircuitBreakerSettings? circuitBreaker = null,
                                  RetrySettings? retry = null)
        {
            CircuitBreaker = circuitBreaker;
            Retry = retry;
        }

        public CircuitBreakerSettings? CircuitBreaker { get; }
        public RetrySettings? Retry { get; }

        public class CircuitBreakerSettings
        {
            public CircuitBreakerSettings(int circuitBreakerAllowedAttempts,
                                          TimeSpan? circuitBreakerDuration = null,
                                          Action<Exception, TimeSpan>? onBreak = null,
                                          Action? onReset = null,
                                          Action? onHalfOpen = null)
            {
                CircuitBreakerAllowedAttempts = circuitBreakerAllowedAttempts;
                CircuitBreakerDuration = circuitBreakerDuration ?? TimeSpan.FromSeconds(5);
                OnBreak = onBreak ?? ((_, _) => { });
                OnReset = onReset ?? (() => { });
                OnHalfOpen = onHalfOpen ?? (() => { });
            }

            /// <summary>
            /// Number of failures before breaking
            /// </summary>
            public int CircuitBreakerAllowedAttempts { get; }

            /// <summary>
            /// Time the circuit breaks for
            /// </summary>
            public TimeSpan CircuitBreakerDuration { get; }

            /// <summary>
            /// Hook that triggers on trigger of circuit breaker
            /// </summary>
            public Action<Exception, TimeSpan>? OnBreak { get; }

            /// <summary>
            /// Hook that triggers on reset of circuit breaker
            /// </summary>
            public Action? OnReset { get; }

            /// <summary>
            /// Hook that triggers on half open circuit breaker
            /// </summary>
            public Action? OnHalfOpen { get; }
        }

        public class RetrySettings
        {
            public RetrySettings(int retryCount,
                                 TimeSpan? retrySeedDelay = null,
                                 TimeSpan? retryDelay = null,
                                 Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task>? onRetryAsync = null)
            {
                RetryCount = retryCount;
                RetrySeedDelay = retrySeedDelay ?? TimeSpan.FromSeconds(5);
                RetryDelay = retryDelay ?? TimeSpan.FromSeconds(10);

                OnRetryAsync = onRetryAsync ?? ((_,
                                                 _,
                                                 _,
                                                 _) => Task.CompletedTask);
            }

            /// <summary>
            /// Number of retries before breaking
            /// </summary>
            public int RetryCount { get; }

            /// <summary>
            /// Seed used for creating retry delay, a jitter is added to this
            /// </summary>
            public TimeSpan RetrySeedDelay { get; }

            /// <summary>
            /// Maximum time between retries
            /// </summary>
            public TimeSpan RetryDelay { get; }

            /// <summary>
            /// Hook for 
            /// </summary>
            public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetryAsync { get; }
        }
    }
}