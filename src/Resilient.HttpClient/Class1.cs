using System;

namespace Resilient.HttpClient
{
    public class Class1
    {
        public Class1()
        {
            var settings = new ResilienceSettings(new ResilienceSettings.CircuitBreakerSettings(3, onBreak: (exception, duration) => Console.WriteLine($"circuit open for {duration:g}: {exception.Message}")),
                                                  new ResilienceSettings.RetrySettings(3));
        }
    }
}