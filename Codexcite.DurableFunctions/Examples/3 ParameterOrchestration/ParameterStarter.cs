using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.ParameterOrchestration
{
	public class ParameterStarter : BaseLogged
	{
		[FunctionName(nameof(ParameterStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ParameterStarter/{name}")] HttpRequestMessage req,
			string name, // direct parameters from Route
			[DurableClient] IDurableOrchestrationClient starter)
		{
			string requestBody = await req.Content.ReadAsStringAsync(); // request body content 
			var body = JsonConvert.DeserializeObject<ParameterStarterRequest>(requestBody); 

			//string instanceId = await starter.StartNewAsync(nameof(ParameterOrchestrator), input: (name, body)); // not possible, input must be class, can't use tuples 
			string instanceId = await starter.StartNewAsync(nameof(ParameterOrchestrator), instanceId: null, input: (name, body)); // can use tuple as input

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
