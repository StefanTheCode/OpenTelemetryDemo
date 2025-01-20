using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetryDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("NewsletterApi"))
                .WithMetrics(metric =>
                {
                    metric.AddAspNetCoreInstrumentation()
                          .AddHttpClientInstrumentation()
                          .AddOtlpExporter(options =>
                          {
                              options.Endpoint = new Uri("https://localhost:21099");
                          });
                });

            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddOtlpExporter(options =>
                           {
                               options.Endpoint = new Uri("https://localhost:21099");
                           });
                });

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("https://localhost:21099");
                });
            });

            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext, ILogger<Program> logger) =>
            {
                logger.LogInformation("This is some test logging for Weather Forecast");

                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();

                var firstForecastTemperature = forecast[0].TemperatureC;

                logger.LogInformation("Structure log for the first Forecast: {firstForecastTemperature}", firstForecastTemperature);

                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.Run();
        }
    }
}
