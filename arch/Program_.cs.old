using System.Diagnostics;

using static System.Formats.Asn1.AsnWriter;

using static System.Net.Mime.MediaTypeNames;


using Microsoft.Extensions.Configuration;


using Serilog;


namespace CaelumServer
{

    using CaelumServer.Utilitatem;
    using CaelumServer.Utilitatem.Logging;

    public class Program
    {
        private static ILogger _logger = Log.Logger._AddContext<Program>();

        private static Scope _scope;

        private static IConfiguration _config;

        private static string appGuid = "7d3122d5-4a48-41a9-b34f-a4bd5dc2d549";

        static void Main()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            IConfigurationRoot configuration = builder.Build();


            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show($"{AppDomain.CurrentDomain.FriendlyName}.exe already running.");
                    return;
                }
                Application.Run(new Deamon(configuration));
            }
        }
        
        private static void initialise()
        {
            _scope = (Debugger.IsAttached) ? Scope.DEV : Environment.GetEnvironmentVariable("SCOPE")._ToEnum<Scope>(Scope.DEV);

            _config = new ConfigurationBuilder()
                .AddDefaults()
                .AddJsonSettings(scope: _scope, mergeArrayHandling: MergeArrayHandling.Replace, defaultSettingsFile: "settings.json")
                .Build();
            Environment.SetEnvironmentVariable("SCOPE", _scope.ToString());

            Log.Logger = _config.CreateLogger();

            _logger = Log.Logger._AddContext<Program>();

            var platform = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ? "Linux" : "Windows";

        }

    }
}