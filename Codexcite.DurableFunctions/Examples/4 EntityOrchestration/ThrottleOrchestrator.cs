using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Examples.EntityOrchestration;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples._4_EntityOrchestration
{
	public class ThrottleOrchestrator : BaseLogged
	{
		[FunctionName(nameof(ThrottleResult))]
		public async Task<object?> ThrottleResult(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var input = context.GetInput<ThrottledFunctionCall>();
			using var propsDisposable = context.PushLoggerProperties();
			_log.Enter(LogEventLevel.Information, arguments: new object?[] { input });

			var throttleProxy = context.CreateEntityProxy<IThrottleEntity>(input.ThrottleKey);
			var scheduleTime = await throttleProxy.GetNextScheduleTime();

			await context.CreateTimer(scheduleTime.UtcDateTime, CancellationToken.None);
			var result = await context.CallActivityAsync<object?>(input.Name, input.Input);

			_log.Exit(LogEventLevel.Information, arguments: new object?[] { input }, returnValue: result);
			return result;
		}

		[FunctionName(nameof(ThrottleVoid))]
		public async Task ThrottleVoid(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var input = context.GetInput<ThrottledFunctionCall>();

			using var propsDisposable = context.PushLoggerProperties();
			_log.Enter(LogEventLevel.Information, arguments: new object?[] { input });

			var throttleProxy = context.CreateEntityProxy<IThrottleEntity>(input.ThrottleKey);
			var scheduleTime = await throttleProxy.GetNextScheduleTime();

			await context.CreateTimer(scheduleTime.UtcDateTime, CancellationToken.None);
			await context.CallActivityAsync(input.Name, input.Input);
			_log.Exit(LogEventLevel.Information, arguments: new object?[] { input });
		}
	}
}
