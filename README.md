# Resilient.HttpClient

Package with the goal to help you implement resilient http requests by abstracting the configuration of the [polly](https://github.com/App-vNext/Polly) policies to a setting

[![NuGet version (Resilient.HttpClient)](https://img.shields.io/nuget/v/Resilient.HttpClient.svg?style=flat-square)](https://www.nuget.org/packages/Resilient.HttpClient/)

## Usage

### Bare minimum

```CSharp

    var settings = new ResilienceSettings(new ResilienceSettings.CircuitBreakerSettings(3), 
                                          new ResilienceSettings.RetrySettings(3));
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient("resilientHttpClient").WithResilience(settings);
    }

```

### Observable
```CSharp

    var settings = new ResilienceSettings(new ResilienceSettings.CircuitBreakerSettings(3, onBreak: (exception, duration) => Console.WriteLine($"circuit open for {duration:g}: {exception.Message}")),
                                        new ResilienceSettings.RetrySettings(3));
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient("resilientHttpClient").WithResilience(settings);
    }

```