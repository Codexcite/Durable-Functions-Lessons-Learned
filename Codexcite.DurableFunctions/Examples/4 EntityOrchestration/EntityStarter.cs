using System;
using System.Net.Http;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Examples.EntityOrchestration.Model;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration
{
	public class EntityStarter : BaseLogged 
	{
		[FunctionName(nameof(EntityStarter))]
		public async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req, 
			[DurableClient] IDurableOrchestrationClient starter,
			[DurableClient] IDurableEntityClient entityClient)
		{
			string requestBody = await req.Content.ReadAsStringAsync(); 
			var person = JsonConvert.DeserializeObject<Person>(requestBody); 

			var storageEntityId = StorageEntity.BuildId(nameof(Person), person.Id);
			// can read the entity state
			var entity = await entityClient.ReadEntityStateAsync<StorageEntity>(storageEntityId);
			if (entity.EntityExists)
				_log.Trace(LogEventLevel.Information, message: $"Person was last updated {entity.EntityState.LastUpdated}");
			// can signal entities, but can't wait for response - only in Orchestrations
			await entityClient.SignalEntityAsync(storageEntityId, nameof(StorageEntity.NotifyUpdated), DateTime.UtcNow); // can use DateTime.UtcNow here, we're not in an orchestration

			string instanceId = await starter.StartNewAsync(nameof(EntityOrchestrator), person); 

			_log.Exit(LogEventLevel.Information, returnValue: instanceId, message: "Starting {instanceId}");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
