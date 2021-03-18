using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Examples.ParameterOrchestration.Model;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.ParameterOrchestration
{
	public class ParameterOrchestrator : BaseLogged
	{
		[FunctionName(nameof(ParameterOrchestrator))]
		public async Task<List<string>> RunOrchestrator(
				[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var input = context.GetInput<(string? Name, ParameterStarterRequest Request)>();

			var person = new Person() {Age = input.Request.Age, Gender = input.Request.Gender, Name = input.Name};
			var outputs = new List<string>();

			try
			{
				//var result = await context.CallActivityAsync<string>(nameof(ParameterActivity), input); // build passes but analyzer correctly fails because of type checking 
				var result = await context.CallActivityAsync<string>(nameof(ParameterActivity), person);
				outputs.Add(result);
				outputs.Add($"Hello from Orchestrator {person.Name}, {person.Gender} of age {person.Age}!"); // person.Age is unchanged here
			}
			catch (FunctionFailedException functionFailedException) // if an activity fails, the exception is wrapped inside a FunctionFailedException
			{
				_log.Exception(LogEventLevel.Error, exception: functionFailedException.InnerException ?? functionFailedException);
				outputs.Add((functionFailedException.InnerException ?? functionFailedException).Message);
			}

			return outputs;
		}

	}
}