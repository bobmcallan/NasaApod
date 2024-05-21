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
    // services.AddScoped<IAstronomyPictureOfTheDayService, AstronomyPictureOfTheDayService>();
    services.AddHttpClient<IAstronomyPictureOfTheDayService, AstronomyPictureOfTheDayService>();
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

app.Run("http://localhost:4000");