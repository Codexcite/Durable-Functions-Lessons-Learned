using System.Net.Http;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.VersionedOrchestration
{
	public class VersionedStarter : BaseLogged 
	{
		protected readonly VersionedOptions _settings;

		// DI with the configured versions
		public VersionedStarter(IOptions<VersionedOptions> configuredOptions) => this._settings = configuredOptions.Value;

		[FunctionName(nameof(VersionedStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req, 
			[DurableClient] IDurableOrchestrationClient starter)
		{
			string instanceId = await starter.StartNewAsync(_settings.VersionOrchestrator, null); // using configured options

			_log.Exit(LogEventLevel.Information, returnValue: instanceId, message: $"Starting {instanceId}");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
