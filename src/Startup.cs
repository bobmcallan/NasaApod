using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaelumServer
{
    internal class Startup
    {
    }
}


using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Carter;

using Lamar;

namespace Monitor
{
    using Serilog;

    using Utilitatem.Http;

    using Utilitatem.Logging;

    using Utilitatem.Interfaces;

    using Utilitatem.Hosting;

    using Utilitatem.Services;

    using Monitor.Services;
    using Utilitatem.Data.RavenDB;
    using Monitor.Models;
    using CaelumServer.Utilitatem.Logging;
    using CaelumServer;
    using Microsoft.Extensions.Configuration;

    public class Startup
    {
        private readonly IConfiguration _config;

        private readonly IService _service;

        private readonly IEnumerable<ILogConfig> _logging;

        private readonly ILogger _logger = Log.Logger._AddContext<Startup>();

        public Startup(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException("Startup - Configuration is null");

            using (Logging._PushMethodContext("CTOR"))
            {
                _service = _config.GetServiceConfig();

                _logging = _config.GetLogConfig();

                _logger.Information("Service:{0} Version:{1} Scope:{2} Url:{3}", _service.Name, _service.Version, _service.Scope, _service.Url);

                foreach (var _log in _logging)
                {
                    _logger.Information("Logging {0}", _log.ToString());
                }

                _logger.Information("Current Directory:{0}", Directory.GetCurrentDirectory());
            }

        }

        public void ConfigureContainer(ServiceRegistry services)
        {
            using (Logging._PushMethodContext())
            {

                _logger.Debug("Scope:{0}", _service.Scope);

                services.AddOptions();

                // Cors -> Add service and create Policy with options
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                });

                services.AddResponseCompression();

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });

                if (_service.Scope != Utilitatem.Enums.Scope.DEV)
                {
                    services.AddLogging(builder =>
                    {
                        builder.AddFilter("Microsoft", LogLevel.Warning)
                                .AddFilter("System", LogLevel.Warning);
                    });
                }

                services.AddLazyCache();

                try
                {
                    services.AddScheduler();

                    services.AddSingleton<IConfiguration>(_config);

                    services.AddSingleton<ILogger>(Log.Logger);

                    // Database

                    services.AddSingleton<IDatabaseService, DatabaseService>();

                    // Models 

                    services.AddSingleton<IDocumentStore<Metimur>, DocumentStore<Metimur>>();
                    services.AddSingleton<IDocumentStore<Metimur_Search>, DocumentStore<Metimur_Search>>();

                    services.AddSingleton<IDocumentStore<Statum>, DocumentStore<Statum>>();

                    services.AddSingleton<IDocumentStore<Summary>, DocumentStore<Summary>>();

                    services.AddSingleton<IDocumentStore<SummaryHour>, DocumentStore<SummaryHour>>();

                    // Services

                    services.AddVariableService();

                    services.AddSingleton<INagiosService, NagiosService>();

                    services.AddSingleton<IMetimurService, MetimurService>();

                    services.AddSingleton<IStatumService, StatumService>();

                    services.AddSingleton<ISummaryService, SummaryService>();

                    services.AddCarter();
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Application Not Configured");

                    throw ex;
                }

                _logger.Debug("Complete");
            }

        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            using (Logging._PushMethodContext())
            {
                _logger.Debug("Scope:{0} env.EnvironmentName:{1} env.IsDevelopment:{2}", _service.Scope, env.EnvironmentName, env.IsDevelopment());

                app.UseLoggingContext();

                app.UseExceptionHandler();

                app.UseResponseCompression();

                try
                {
                    app.UseRouting();

                    app.UseScheduler();

                    app.UseVariableService();

                    app.StartNagiosService();

                    app.UseEndpoints(builder => builder.MapCarter());

                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Application Not Configured");

                    throw ex;
                }

                _logger.Debug("Complete");
            }
        }


    }

}
