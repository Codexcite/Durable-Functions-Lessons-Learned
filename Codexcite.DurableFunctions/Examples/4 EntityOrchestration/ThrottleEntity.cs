using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Services.Time;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.EntityOrchestration
{
	public class ThrottleOptions
	{
		public TimeSpan TimeFrame { get; set; } = TimeSpan.FromMinutes(1);
		public IDictionary<string, int> Services { get; set; } = new Dictionary<string, int>();
	}
	public interface IThrottleEntity
	{
		Task<DateTimeOffset> GetNextScheduleTime();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ThrottleEntity : IThrottleEntity
	{
		protected ITimeService TimeService { get; }
		protected readonly ThrottleOptions _settings;

		public ThrottleEntity(IOptions<ThrottleOptions> configuredOptions, ITimeService timeService)
		{
			TimeService = timeService;
			_settings = configuredOptions.Value;
		}

		[JsonProperty]
		internal DateTimeOffset? LastScheduledOn { get; set; }


		public Task<DateTimeOffset> GetNextScheduleTime()
		{
			using var contextProps = Entity.Current.PushLoggerProperties();
			if (!_settings.Services.ContainsKey(Entity.Current.EntityKey))
				return Task.FromResult(TimeService.UtcNow); // no throttling required

			var targetTime = TimeService.UtcNow;
			if (LastScheduledOn.HasValue)
			{
				_settings.Services.TryGetValue(Entity.Current.EntityKey, out var limit);

				var timeFrame = _settings.TimeFrame;
				var throttledTarget = LastScheduledOn.Value.AddMilliseconds(limit == 0 ? 0d : timeFrame.TotalMilliseconds / limit);
				if (throttledTarget > targetTime)
					targetTime = throttledTarget;
			}

			LastScheduledOn = targetTime;
			return Task.FromResult(targetTime);
		}

		[FunctionName(nameof(ThrottleEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx)
			=> ctx.DispatchAsync<ThrottleEntity>();
	}
}
