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
	public class VersionedSubOrchestratorV1 : BaseLogged
	{
		protected readonly VersionedOptions _settings;

		// DI with the configured versions
		public VersionedSubOrchestratorV1(IOptions<VersionedOptions> configuredOptions) => this._settings = configuredOptions.Value;

		[FunctionName(nameof(VersionedSubOrchestratorV1))]
		public async Task<List<string>> RunOrchestrator(
					[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			using var disposable = context.PushLoggerProperties(); // includes parent instanceId

			var outputs = new List<string>();
			
			_log.Trace(LogEventLevel.Information, message: "Calling Monterrey");
			outputs.Add(await context.CallActivityAsync<string>(_settings.VersionActivity, "Monterrey")); 
			
			_log.Trace(LogEventLevel.Information, message: "Calling Mexico City");
			outputs.Add(await context.CallActivityAsync<string>(_settings.VersionActivity, "Mexico City"));

			return outputs;
		}
	}
}