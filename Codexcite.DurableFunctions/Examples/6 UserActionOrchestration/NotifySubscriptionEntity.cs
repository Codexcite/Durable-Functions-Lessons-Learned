using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Services.Time;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public interface INotifySubscriptionEntity
	{
		Task Subscribe(string instanceId);
		Task Unsubscribe(string instanceId);
		Task RegisterNotification(NotifyPayload payload);
		Task<NotifyPayload?> GetLatestNotification();
		Task<string[]> GetInstancesToNotify();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class NotifySubscriptionEntity : BaseLogged, INotifySubscriptionEntity
	{
		private readonly ITimeService _timeService;
		private readonly TimeSpan _latestNotificationsSpan;

		public NotifySubscriptionEntity(ITimeService timeService)
		{
			_timeService = timeService;
			_latestNotificationsSpan = TimeSpan.FromDays(7); // how long to keep notifications
		}

		[JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
		internal string[] InstancesToNotify { get; set; } = Array.Empty<string>();

		[JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
		internal NotifyPayload[] LatestNotifications { get; set; } = Array.Empty<NotifyPayload>();

		internal static EntityId BuildId(string? userId, string eventName)
			=> new EntityId(nameof(NotifySubscriptionEntity), BuildKey(userId, eventName));

		internal static string BuildKey(string? userId, string eventName)
			=> $"{eventName}|{userId}";

		public Task Subscribe(string instanceId)
		{
			InstancesToNotify = InstancesToNotify.Union(new[] {instanceId}).ToArray();
			return Task.CompletedTask;
		}

		public Task Unsubscribe(string instanceId)
		{
			InstancesToNotify = InstancesToNotify.Except(new[] {instanceId}).ToArray();
			return Task.CompletedTask;
		}

		public Task RegisterNotification(NotifyPayload payload)
		{
			LatestNotifications = LatestNotifications.Union(new[] {payload})
																							 .Where(x => x.Timestamp > _timeService.UtcNow - _latestNotificationsSpan)
																							 .ToArray();
			return Task.CompletedTask;
		}

		public Task<NotifyPayload?> GetLatestNotification()
		{
			return Task.FromResult((NotifyPayload?) LatestNotifications
																										 .Where(x => x.Timestamp > _timeService.UtcNow - _latestNotificationsSpan)
																										 .OrderByDescending(x => x.Timestamp)
																										 .FirstOrDefault());
		}

		public Task<string[]> GetInstancesToNotify() => Task.FromResult(InstancesToNotify);

		[FunctionName(nameof(NotifySubscriptionEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx)
			=> ctx.DispatchAsync<NotifySubscriptionEntity>();

	}
}
