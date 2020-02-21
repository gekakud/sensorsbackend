using System;
using Core.Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Sentry;
using Serilog;
using Serilog.Events;

namespace Core.Common.Logging
{
    public static class Extensions
    {
        public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder, string applicationName = null)
        {
            return webHostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var appOptions = context.Configuration.GetOptions<AppOptions>("app");
                var seqOptions = context.Configuration.GetOptions<SeqConfig>("seq");
                var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("serilog");
                var sentryOptions = context.Configuration.GetOptions<SentryConfig>("sentry");

                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                {
                    level = LogEventLevel.Information;
                }

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
                loggerConfiguration.Enrich.FromLogContext()
                    .MinimumLevel.Is(level)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ApplicationName", applicationName)
                    .WriteTo.Console();

                Configure(loggerConfiguration, seqOptions, serilogOptions,sentryOptions);
            });
        }

        private static void Configure(LoggerConfiguration loggerConfiguration, SeqConfig seqConfig, SerilogOptions serilogOptions,SentryConfig sentryConfig)
        {
            
//            .UseSentry("https://1b2e2d88569b4339a33b5727a7050dc0@sentry.io/1514436")
            if (seqConfig.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(seqConfig.Url, apiKey: seqConfig.ApiKey);
            }

            if (serilogOptions.ConsoleEnabled)
            {
                loggerConfiguration.WriteTo.Console();
            }

            if (sentryConfig.Enabled)
            {
                loggerConfiguration.WriteTo.Sentry(o =>
                {
                    o.MinimumBreadcrumbLevel =
                        LogEventLevel.Debug; // Debug and higher are stored as breadcrumbs (default os Information)
                    o.MinimumEventLevel = LogEventLevel.Error; // Error and higher is sent as event (default is Error)
                    // If DSN is not set, the SDK will look for an environment variable called SENTRY_DSN. If nothing is found, SDK is disabled.
                    o.Dsn = new Dsn(sentryConfig.SentryDns);
                    o.AttachStacktrace = true;
                    o.SendDefaultPii = true; // send PII like the username of the user logged in to the device
                    // Other configuration
                });
            }
            
        }
    }
}