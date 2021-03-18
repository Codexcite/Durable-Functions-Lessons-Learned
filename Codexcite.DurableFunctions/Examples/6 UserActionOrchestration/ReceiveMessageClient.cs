using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Services.Time;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public class ReceiveMessageClient : BaseLogged
	{
		protected ITimeService TimeService { get; }

		public ReceiveMessageClient(ITimeService timeService)
		{
			TimeService = timeService;
		}

		[FunctionName(nameof(ReceiveMessage))]
		public async Task<HttpResponseMessage> ReceiveMessage(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ReceiveMessage/{phone}/{message}")] HttpRequestMessage req,
			string phone, string message,
			[DurableClient] IDurableEntityClient entityClient, // need the durable entity client
			[DurableClient] IDurableOrchestrationClient orchestrationClient) // and the Orchestration client
		{
			if (!message.Equals("APPROVE", StringComparison.InvariantCultureIgnoreCase)
					&& !message.Equals("REJECT", StringComparison.InvariantCultureIgnoreCase))
				return new HttpResponseMessage(HttpStatusCode.BadRequest);

			var notifyPayload = new NotifyPayload(){ Phone = phone, EventName = message.ToUpperInvariant(), Timestamp = TimeService.UtcNow};

			var notifySubscriptionEntityId = NotifySubscriptionEntity.BuildId(phone, notifyPayload.EventName);
			var entity = await entityClient.ReadEntityStateAsync<NotifySubscriptionEntity>(notifySubscriptionEntityId);
			await entityClient.SignalEntityAsync(notifySubscriptionEntityId, nameof(NotifySubscriptionEntity.RegisterNotification), 
																					 notifyPayload); // send a signal to the entity
			
			if (entity.EntityExists && entity.EntityState.InstancesToNotify.Length > 0) // we have live subscribers
			{
				foreach (var instanceId in entity.EntityState.InstancesToNotify)
				{
					await orchestrationClient.RaiseEventAsync(instanceId, notifyPayload.EventName, notifyPayload); // notify all subscribers
				}
			}
			else
			{
				_log.Trace(LogEventLevel.Verbose, message: "No subscribed instances to notify");
			}


			return new HttpResponseMessage(HttpStatusCode.OK);
		}

	}
}