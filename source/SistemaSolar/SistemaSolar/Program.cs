﻿using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemaSolar.Extensions;
using System;
using System.IO;
//using Hangfire.MySql.Core;
using Hangfire;
using Hangfire.MySql.Core;
using Contracts;
using System.Threading.Tasks;
using Entities;

namespace SistemaSolar
{
    public class Program
    {
        public static IHostingEnvironment HostingEnvironment { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        public static string GcpProjectId { get; private set; }
        public static bool HasGcpProjectId => !string.IsNullOrEmpty(GcpProjectId);

        public static void Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder().Build();

            // Create a new scope
            using (var scope = host.Services.CreateScope())
            {
                // Get the DbContext instance
                var service = scope.ServiceProvider.GetService<IPronosticoService>();

                //Do the migration asynchronously
                service.RunJob();
            }

            // Run the WebHost, and start accepting requests
            // There's an async overload, so we may as well use it
            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                            .UseKestrel()
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseIISIntegration()
                            .ConfigureAppConfiguration((context, configBuilder) =>
                            {
                                HostingEnvironment = context.HostingEnvironment;

                                configBuilder.SetBasePath(HostingEnvironment.ContentRootPath)
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", optional: true)
                                    .AddEnvironmentVariables();

                                Configuration = configBuilder.Build();
                                GcpProjectId = GetProjectId(Configuration);
                            })
                            .ConfigureServices(services =>
                            {
                                services.ConfigureMySqlContext(Configuration);
                                //services.AddHangfire(config => config.UseStorage(new MySqlStorage(Configuration.GetConnectionString("SistemaSolar"))));
                                services.ConfigureRepositoryWrapper();
                                services.ConfigureServices();

                                // Add framework services.Microsoft.VisualStudio.ExtensionManager.ExtensionManagerService
                                services.AddMvc();

                                // Add functionality to inject IOptions<T>
                                services.AddOptions();

                                // Add our Config object so it can be injected
                                services.Configure<MyConfig>(Configuration.GetSection("Data"));

                                if (HasGcpProjectId)
                                {
                                    // Enables Stackdriver Trace.
                                    services.AddGoogleTrace(options => options.ProjectId = GcpProjectId);
                                    // Sends Exceptions to Stackdriver Error Reporting.
                                    services.AddGoogleExceptionLogging(
                                                    options =>
                                                    {
                                                        options.ProjectId = GcpProjectId;
                                                        options.ServiceName = GetServiceName(Configuration);
                                                        options.Version = GetVersion(Configuration);
                                                    });
                                    services.AddSingleton<ILoggerProvider>(sp => GoogleLoggerProvider.Create(GcpProjectId));
                                }
                            })
                            .ConfigureLogging(loggingBuilder =>
                            {
                                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                                if (HostingEnvironment.IsDevelopment())
                                {
                                    // Only use Console and Debug logging during development.
                                    loggingBuilder.AddConsole(options =>
                                                    options.IncludeScopes = Configuration.GetValue<bool>("Logging:IncludeScopes"));
                                    loggingBuilder.AddDebug();
                                }
                            })
                            .Configure((app) =>
                            {
                                var logger = app.ApplicationServices.GetService<ILoggerFactory>().CreateLogger("Startup");

                                if (HasGcpProjectId)
                                {

                                    logger.LogInformation("Stackdriver Logging enabled: https://console.cloud.google.com/logs/");

                                    // Sends logs to Stackdriver Error Reporting.
                                    app.UseGoogleExceptionLogging();
                                    logger.LogInformation(
                                        "Stackdriver Error Reporting enabled: https://console.cloud.google.com/errors/");
                                    // Sends logs to Stackdriver Trace.
                                    app.UseGoogleTrace();
                                    logger.LogInformation("Stackdriver Trace enabled: https://console.cloud.google.com/traces/");
                                }
                                else
                                {
                                    logger.LogWarning(
                                        "Stackdriver Logging not enabled. Missing Google:ProjectId in configuration.");
                                    logger.LogWarning(
                                        "Stackdriver Error Reporting not enabled. Missing Google:ProjectId in configuration.");
                                    logger.LogWarning(
                                        "Stackdriver Trace not enabled. Missing Google:ProjectId in configuration.");
                                }

                                app.UseMvc();
                            });
        }

        /// <summary>
        /// Get the Google Cloud Platform Project ID from the platform it is running on,
        /// or from the appsettings.json configuration if not running on Google Cloud Platform.
        /// </summary>
        /// <param name="config">The appsettings.json configuration.</param>
        /// <returns>
        /// The ID of the GCP Project this service is running on, or the Google:ProjectId
        /// from the configuration if not running on GCP.
        /// </returns>
        private static string GetProjectId(IConfiguration config)
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var projectId = instance?.ProjectId ?? config["Google:ProjectId"];
            if (string.IsNullOrEmpty(projectId))
            {
                // Set Google:ProjectId in appsettings.json to enable stackdriver logging outside of GCP.
                return null;
            }
            return projectId;
        }

        /// <summary>
        /// Gets a service name for error reporting.
        /// </summary>
        /// <param name="config">The appsettings.json configuration to read a service name from.</param>
        /// <returns>
        /// The name of the Google App Engine service hosting this application,
        /// or the Google:ErrorReporting:ServiceName configuration field if running elsewhere.
        /// </returns>
        /// <seealso href="https://cloud.google.com/error-reporting/docs/formatting-error-messages#FIELDS.service"/>
        private static string GetServiceName(IConfiguration config)
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var serviceName = instance?.GaeDetails?.ServiceId ?? config["Google:ErrorReporting:ServiceName"];
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new InvalidOperationException(
                    "The error reporting library requires a service name. " +
                    "Update appsettings.json by setting the Google:ErrorReporting:ServiceName property with your " +
                    "Service Id, then recompile.");
            }
            return serviceName;
        }

        /// <summary>
        /// Gets a version id for error reporting.
        /// </summary>
        /// <param name="config">The appsettings.json configuration to read a version id from.</param>
        /// <returns>
        /// The version of the Google App Engine service hosting this application,
        /// or the Google:ErrorReporting:Version configuration field if running elsewhere.
        /// </returns>
        /// <seealso href="https://cloud.google.com/error-reporting/docs/formatting-error-messages#FIELDS.version"/>
        private static string GetVersion(IConfiguration config)
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var versionId = instance?.GaeDetails?.VersionId ?? config["Google:ErrorReporting:Version"];
            if (string.IsNullOrEmpty(versionId))
            {
                throw new InvalidOperationException(
                    "The error reporting library requires a version id. " +
                    "Update appsettings.json by setting the Google:ErrorReporting:Version property with your " +
                    "service version id, then recompile.");
            }
            return versionId;
        }
    }
}
