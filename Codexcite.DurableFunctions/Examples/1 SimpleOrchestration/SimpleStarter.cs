using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Serilog.Events;
using System.Net.Http;
using System.Threading.Tasks;

namespace Codexcite.DurableFunctions.Examples.SimpleOrchestration
{
	// Not a static class - can use DI
	public class SimpleStarter : BaseLogged 
	{
		// Changed to member function instead of static function
		[FunctionName(nameof(SimpleStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req, // IMPORTANT: the function key is ignored for localhost, don't forget about it when publishing
			[DurableClient] IDurableOrchestrationClient starter)
		{
			string instanceId = await starter.StartNewAsync(nameof(SimpleOrchestrator), null); // using "nameof()" instead of magic strings

			_log.Exit(LogEventLevel.Information, returnValue: instanceId, message: $"Starting {instanceId}");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
