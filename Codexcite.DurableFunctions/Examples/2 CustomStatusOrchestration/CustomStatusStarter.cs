using System.Net.Http;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.CustomStatusOrchestration
{
	public class CustomStatusStarter : BaseLogged
	{
		[FunctionName(nameof(CustomStatusStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter)
		{
			// Function input comes from the request content.
			string instanceId = await starter.StartNewAsync(nameof(CustomStatusOrchestrator), null);

			_log.Exit(LogEventLevel.Information, returnValue: instanceId, message: "Starting {instanceId}",
								 propertyValues: new (string Name, object? Value)[] { ("instanceId", instanceId) });

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
