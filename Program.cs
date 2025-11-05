using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Register HttpClient and Logging
builder.Services.AddHttpClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.MapGet("/weather", async ([FromServices] HttpClient httpClient, [FromServices] ILogger<Program> logger, string name, string? city) =>
{
    string apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? "";
    string message;
    string usedCity = city ?? "your location";

    if (string.IsNullOrEmpty(apiKey))
    {
        logger.LogWarning("No OPENWEATHER_API_KEY found. Using fallback message.");
        message = $"Hello {name}! I can’t fetch live weather data right now because no API key is configured, but I hope it’s nice in {usedCity}!";
        return Results.Ok(new { name, city = usedCity, message });
    }

    try
    {
        // Default to Austin if city not provided
        if (string.IsNullOrEmpty(city))
        {
            city = "Austin";
        }

        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var reason = await response.Content.ReadAsStringAsync();
            logger.LogError("Weather API request failed: {StatusCode} - {Reason}", response.StatusCode, reason);
            message = $"Hello {name}! I tried to fetch weather data for {city}, but the API returned an error ({response.StatusCode}).";
            return Results.Ok(new { name, city, message });
        }

        var weatherData = await response.Content.ReadFromJsonAsync<WeatherResponse>();

        if (weatherData != null && weatherData.Main != null)
        {
            message = $"Hello {name}! The weather in {city} is {weatherData.Main.Temp}°C with {weatherData.Weather?[0].Description}.";
        }
        else
        {
            logger.LogWarning("Weather data structure was null or incomplete for city {City}.", city);
            message = $"Hello {name}! I couldn’t interpret the weather data for {city}.";
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Exception occurred while fetching weather data for {City}.", city);
        message = $"Hello {name}! I ran into a problem while trying to get the weather for {city}.";
    }

    return Results.Ok(new { name, city, message });
});

app.Run();

record WeatherResponse(Main Main, Weather[] Weather);
record Main(double Temp);
record Weather(string Description);