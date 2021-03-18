using Codexcite.DurableFunctions.Examples.EntityOrchestration.Model;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration
{
	class CosmosConstants
	{
		internal const string DatabaseName = "ExampleDb";
		internal const string ContainerName = "People";
		internal const string ConnectionStringName = "CosmosDBConnection";
	}
	public class StorageActivity : BaseLogged
	{
		[FunctionName(nameof(StorePerson))]
		public void StorePerson([ActivityTrigger] IDurableActivityContext context,
														[CosmosDB(
																			 CosmosConstants.DatabaseName,
																			 CosmosConstants.ContainerName,
																			 ConnectionStringSetting = CosmosConstants.ConnectionStringName)]
														out Person item)
		{
			var input = context.GetInput<Person>();
			item = input;
		}

		[FunctionName(nameof(GetPersonById))]
		public Person? GetPersonById([ActivityTrigger] string id,
														 [CosmosDB(
																				CosmosConstants.DatabaseName,
																				CosmosConstants.ContainerName,
																				ConnectionStringSetting = CosmosConstants.ConnectionStringName, 
																				PartitionKey = "{id}", 
																				Id = "{id}")]
														 Person item)
		{
			return item;
		}
	}
}