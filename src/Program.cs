using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using Helpers;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddCors();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params (e.g. User update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    // configure DI for application services
    services.Configure<ApodConfiguration>(builder.Configuration.GetSection(nameof(ApodConfiguration)));
    services.AddHttpClient<IAstronomyPictureOfTheDayService, AstronomyPictureOfTheDayService>();

    // Register KafkaConfiguration
    services.Configure<KafkaConfiguration>(builder.Configuration.GetSection(nameof(KafkaConfiguration)));

    // Register KafkaConsumer as BackgroundService
    services.AddSingleton<KafkaBackgroundService>();
    services.AddSingleton<IHostedService>(p => p.GetService<KafkaBackgroundService>());

    // Kafka Producer
    services.AddSingleton<KafkaHandler>();
    services.AddSingleton<IKafkaProducer, KafkaProducer>();

}

var app = builder.Build();

// Configure HTTP Requests
{
    // CORS
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // Error Handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapControllers();
}

var platform = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" : "Windows";
var url = (platform == "Windows") ? "http://0.0.0.0:4100" : "http://0.0.0.0:4000";

app.Run(url);