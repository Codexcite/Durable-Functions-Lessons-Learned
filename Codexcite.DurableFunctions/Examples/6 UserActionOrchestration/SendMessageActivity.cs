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
		private static int CallCount = 0;
		[FunctionName(nameof(SendMessageActivity))]
		public string SayHello([ActivityTrigger] string phone) // Logger now comes from DI
		{
			CallCount += 1;
			_log.Enter(LogEventLevel.Information, message: $"Sending message count:{CallCount}");
			if (CallCount % 3 != 0)
			{
				var exception = new InfoException(CallCount, "Random failure");
				_log.Exception(LogEventLevel.Error, exception: exception, message: "Fired random failure");
				throw exception; // just to demonstrate retry functionality
			}
		

			_log.Exit(LogEventLevel.Information, message: $"Sending message to {phone}.");
			return $"Sent message to {phone}!";
		}

	}
}