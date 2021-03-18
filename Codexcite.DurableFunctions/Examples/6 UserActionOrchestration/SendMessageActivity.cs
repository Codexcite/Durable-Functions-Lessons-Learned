using System;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public class SendMessageActivity : BaseLogged
	{
		// Changed to member function instead of static function

		[FunctionName(nameof(SendMessageActivity))]
		public string SayHello([ActivityTrigger] string phone) // Logger now comes from DI
		{
			var random = new Random().Next(100);
			if (random % 3 == 0)
				throw new InfoException(random, "Random failure"); // just to demonstrate retry functionality

			_log.Trace(LogEventLevel.Information, message: $"Sending message to {phone}.");
			return $"Sent message to {phone}!";
		}

	}
}