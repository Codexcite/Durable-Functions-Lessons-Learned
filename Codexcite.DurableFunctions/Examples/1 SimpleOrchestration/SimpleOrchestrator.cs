using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.SimpleOrchestration
{
	public class SimpleOrchestrator : BaseLogged
	{
		// Changed to member function instead of static function
		[FunctionName(nameof(SimpleOrchestrator))]
		public async Task<List<string>> RunOrchestrator(
					[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			using var disposable = context.PushLoggerProperties(); // include durable context data in your logging

			var outputs = new List<string>();

			_log.Trace(LogEventLevel.Information, message: "Calling Tokyo");
			outputs.Add(await context.CallActivityAsync<string>(nameof(SimpleActivity), "Tokyo")); // using "nameof()" instead of magic strings
			
			_log.Trace(LogEventLevel.Information, message: "Calling Seattle");
			outputs.Add(await context.CallActivityAsync<string>(nameof(SimpleActivity), "Seattle"));
			
			_log.Trace(LogEventLevel.Information, message: "Calling London");
			outputs.Add(await context.CallActivityAsync<string>(nameof(SimpleActivity), "London")); // keep in mind the overhead of every Activity call

			// returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
			return outputs;
		}
	}
}