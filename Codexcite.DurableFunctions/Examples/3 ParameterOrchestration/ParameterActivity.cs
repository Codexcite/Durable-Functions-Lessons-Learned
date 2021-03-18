using System;
using Codexcite.DurableFunctions.Examples.ParameterOrchestration.Model;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.ParameterOrchestration
{
	public class ParameterActivity : BaseLogged
	{
		[FunctionName(nameof(ParameterActivity))]
		public string SayHello([ActivityTrigger] Person input)
		{
			if (!input.Age.HasValue || input.Age < 18)
				throw new InvalidOperationException("Must be over 18 to say hello!");

			input.Age += 10; // this change won't be reflected in the parent orchestration

			_log.Trace(LogEventLevel.Information, message: $"Saying hello to {input.Name}, {input.Gender} of age {input.Age}.");

			return $"Hello {input.Name}, {input.Gender} of age {input.Age}!";
		}
	}
}