using System.Net.Http;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public class UserActionStarter : BaseLogged 
	{
		[FunctionName(nameof(UserActionStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ParameterStarter/{phone}")] HttpRequestMessage req,
			string phone,
			[DurableClient] IDurableOrchestrationClient starter)
		{
			
			string instanceId = await starter.StartNewAsync(nameof(UserActionOrchestrator), instanceId: null, input: phone); 

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
