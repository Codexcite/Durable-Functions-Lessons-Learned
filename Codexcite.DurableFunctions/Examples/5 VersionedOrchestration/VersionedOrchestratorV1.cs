using System.Collections.Generic;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Options;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.VersionedOrchestration
{
	public class VersionedOrchestratorV1 : BaseLogged
	{
		protected readonly VersionedOptions _settings;

		// DI with the configured versions
		public VersionedOrchestratorV1(IOptions<VersionedOptions> configuredOptions) => this._settings = configuredOptions.Value;

		[FunctionName(nameof(VersionedOrchestratorV1))]
		public async Task<List<string>> RunOrchestrator(
					[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			using var disposable = context.PushLoggerProperties(); 

			var outputs = new List<string>();

			_log.Trace(LogEventLevel.Information, message: "Calling Tokyo");
			outputs.Add(await context.CallActivityAsync<string>(_settings.VersionActivity, "Tokyo")); 
			
			_log.Trace(LogEventLevel.Information, message: "Calling Seattle");
			outputs.Add(await context.CallActivityAsync<string>(_settings.VersionActivity, "Seattle"));
			
			_log.Trace(LogEventLevel.Information, message: "Calling London");
			outputs.Add(await context.CallActivityAsync<string>(_settings.VersionActivity, "London"));


			return outputs;
		}
	}
}