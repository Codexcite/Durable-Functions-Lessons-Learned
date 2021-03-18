using System.Collections.Generic;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Examples.EntityOrchestration.Model;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration
{
	public class EntityOrchestrator : BaseLogged
	{
		[FunctionName(nameof(EntityOrchestrator))]
		public async Task<List<string>> RunOrchestrator(
					[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			using var disposable = context.PushLoggerProperties();
			var input = context.GetInput<Person>();

			var outputs = new List<string>();

			var storageEntityId = StorageEntity.BuildId(nameof(Person), input.Id);
			var storageEntityProxy = context.CreateEntityProxy<IStorageEntity>(storageEntityId);
			
			// here we can call the method that returns a result
			var previousUpdate = storageEntityProxy.GetLastUpdated();
			outputs.Add($"Previous update on {previousUpdate}");

			// ensure no one updates the person while we operate on it
			using (await context.LockAsync(storageEntityId))
			{
				var existingPerson = await context.CallActivityAsync<Person>(nameof(StorageActivity.GetPersonById), input.Id);

				if (existingPerson != null)
				{
					// only update the age
					existingPerson.Age = input.Age;

					await context.CallActivityAsync(nameof(StorageActivity.StorePerson), existingPerson);
					outputs.Add("Person exists, updated Age");
				}
				else
				{
					await context.CallActivityAsync(nameof(StorageActivity.StorePerson), input);
					outputs.Add("Added new person");
				}

				storageEntityProxy.NotifyUpdated(context.CurrentUtcDateTime);
			}

			return outputs;
		}
	}
}