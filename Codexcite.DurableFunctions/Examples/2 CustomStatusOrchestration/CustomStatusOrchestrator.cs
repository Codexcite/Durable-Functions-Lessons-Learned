using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Utils;

namespace Codexcite.DurableFunctions.Examples.CustomStatusOrchestration
{
	public class CustomStatusOrchestrator : BaseLogged
	{
		[FunctionName(nameof(CustomStatusOrchestrator))]
		public async Task<List<string>> RunOrchestrator(
				[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var outputs = new List<string>();
			
			for (int i = 0; i < 50; i++)
			{
				outputs.Add(await context.CallActivityAsync<string>(nameof(CustomStatusActivity), i.ToString()));

				context.SetCustomStatus(outputs); // pass any object with progress information, for example. This will be included in the HTTP 202 responses.

				// make sure your orchestration is easy to read as a workflow - extract methods to hide implementation details
				await WaitFor(context, TimeSpan.FromSeconds(2)); // can await other methods, as long as they only await context calls
			}

			return outputs;
		}

		// only await context calls inside this method
		private static async Task WaitFor(IDurableOrchestrationContext context, TimeSpan duration)
		{
			var cts = new CancellationTokenSource();
			DateTime now = context.CurrentUtcDateTime; // context.CurrentUtcDateTime is a DateTime, not a DateTimeOffset. Take great care when doing DateTime operations.
			var deadline = now.Add(duration); // the timer works with deadlines, not durations
			await context.CreateTimer(deadline, cts.Token);
		}
	}
}