using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

using Polly;

namespace Resilient.HttpClient
{
    public static class HttpClientBuilderExtensions
    {
        private static IAsyncPolicy<HttpResponseMessage> NoOperationPolicy
            => Policy.NoOpAsync<HttpResponseMessage>();

        private static IEnumerable<HttpStatusCode> RecoverableHttpStatusCodes
            => new List<HttpStatusCode>
               {
                   HttpStatusCode.InternalServerError,
                   HttpStatusCode.BadGateway,
                   HttpStatusCode.ServiceUnavailable,
                   HttpStatusCode.GatewayTimeout
               };

        public static IHttpClientBuilder WithResilience(this IHttpClientBuilder builder,
                                                        ResilienceSettings settings)
            => builder.AddPolicyHandler(settings.AsyncPolicy());

        private static IAsyncPolicy<HttpResponseMessage> AsyncPolicy(this ResilienceSettings.CircuitBreakerSettings? settings)
            => settings != null
                   ? Policy.HandleResult<HttpResponseMessage>(x => RecoverableHttpStatusCodes.Contains(x.StatusCode))
                           .Or<HttpRequestException>()
                           .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking:settings.CircuitBreakerAllowedAttempts,
                                                durationOfBreak:settings.CircuitBreakerDuration,
                                                onBreak:(response, timeSpan) => settings.OnBreak?.Invoke(new Exception("Circuit breaker tripped.", response.Exception), timeSpan),
                                                onReset:settings.OnReset,
                                                onHalfOpen:settings.OnHalfOpen)
                   : NoOperationPolicy;

        private static IAsyncPolicy<HttpResponseMessage> AsyncPolicy(this ResilienceSettings.RetrySettings? settings)
            => settings != null
                   ? Policy.HandleResult<HttpResponseMessage>(x => RecoverableHttpStatusCodes.Contains(x.StatusCode))
                           .Or<HttpRequestException>()
                           .WaitAndRetryAsync(sleepDurations:Jitter.Decorrelated(settings.RetryCount,
                                                                                 settings.RetrySeedDelay,
                                                                                 settings.RetryDelay),
                                              onRetryAsync:settings.OnRetryAsync)
                   : NoOperationPolicy;

        private static IAsyncPolicy<HttpResponseMessage> AsyncPolicy(this ResilienceSettings? settings)
            => settings != null
                   ? settings.CircuitBreaker.AsyncPolicy().WrapAsync(settings.Retry.AsyncPolicy())
                   : NoOperationPolicy;
    }
}