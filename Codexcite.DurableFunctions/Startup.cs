using System;
using Codexcite.DurableFunctions;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Services.MessageSerializer;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Codexcite.DurableFunctions
{
	public class Startup : FunctionsStartup
	{
		protected string ApplicationName => nameof(Codexcite.DurableFunctions);

		protected IConfiguration? Configuration;

		public override void Configure(IFunctionsHostBuilder builder)
		{
			var services = builder.Services;
			Configuration = services.GetConfiguration();

			builder.Services.AddSingleton<IMessageSerializerSettingsFactory, MessageSerializerSettings>();
			JsonConvert.DefaultSettings = () => Json.MessageJsonSerializerSettings; // shouldn't be required, but some message serializing ignores IMessageSerializerSettingsFactory

			ServiceRegistration.Configuration = Configuration;

			Log.Logger = ConfigureLogger(Configuration);
			services.AddLogging(lb => lb.AddSerilog(Log.Logger));

			builder.Services
							.AddTime()
							.AddVersionedOptions()
						;
		}

		protected ILogger ConfigureLogger(IConfiguration? configuration)
		{
			if (!Enum.TryParse(configuration?.GetValue<string>("Serilog:MinimumLevel:Default"), out LogEventLevel configuredMinimumLevel))
				configuredMinimumLevel = LogEventLevel.Information;
			var minimumLevelSwitch = new LoggingLevelSwitch(configuredMinimumLevel);
			return new LoggerConfiguration()
					//						.ReadFrom.Configuration(configRoot)
					//.MinimumLevel.Information()
					.MinimumLevel.ControlledBy(minimumLevelSwitch)
					.MinimumLevel.Override("Host.Results", LogEventLevel.Warning)
					.MinimumLevel.Override("Host.Aggregator", LogEventLevel.Warning)
					.MinimumLevel.Override("Host.Triggers", LogEventLevel.Warning)
					.MinimumLevel.Override("System", LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
					.Destructure.ToMaximumCollectionCount(50)
					.Destructure.ToMaximumDepth(10)
#if DEBUG
					.WriteTo.Console(LogEventLevel.Information,
																	"[{Timestamp:HH:mm:ss.fff} {Level:u3}] [{SourceContext:l}].[{MemberName:l}] {Message}{NewLine}{Exception}")
					.WriteTo.Seq("http://localhost:5341/")
#else
									.WriteTo.ApplicationInsights(TelemetryConverter.Traces)
#endif
					.Enrich.FromLogContext()
					.Enrich.WithExceptionDetails()
					.Enrich.WithProperty("Application", ApplicationName)
					.Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("APPLICATION_ENVIRONMENT"))
					.CreateLogger();
		}


	}
}