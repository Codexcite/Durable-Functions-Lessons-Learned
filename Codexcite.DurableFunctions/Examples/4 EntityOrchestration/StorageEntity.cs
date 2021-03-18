using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration
{
	public interface IStorageEntity
	{
		public void NotifyUpdated(DateTime updateDateTime);
		public DateTime? GetLastUpdated();
	}
	public class StorageEntity : IStorageEntity
	{
		public DateTime? LastUpdated { get; set; }

		internal static EntityId BuildId(string typeName, string id)
			=> new EntityId(nameof(StorageEntity), $"{typeName}|{id}");

		[FunctionName(nameof(StorageEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx)
			=> ctx.DispatchAsync<StorageEntity>();


		public void NotifyUpdated(DateTime updateDateTime)
		{
			LastUpdated = updateDateTime;
		}

		public DateTime? GetLastUpdated() => LastUpdated;
	}
}
