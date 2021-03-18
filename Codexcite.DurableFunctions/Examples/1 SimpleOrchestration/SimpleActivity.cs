using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.SimpleOrchestration
{
	public class SimpleActivity : BaseLogged
	{
		// Changed to member function instead of static function

		[FunctionName(nameof(SimpleActivity))]
		public string SayHello([ActivityTrigger] string name) // Logger now comes from DI
		{
			_log.Trace(LogEventLevel.Information, message: $"Saying hello to {name}.");
			return $"Hello {name}!";
		}

		public string SayHelloWithContext([ActivityTrigger] IDurableActivityContext context) // context version
		{
			using var disposable = context.PushLoggerProperties();
			var name = context.GetInput<string>();

			_log.Trace(LogEventLevel.Information, message: $"Saying hello to {name}.");
			return $"Hello {name}!";
		}
	}
}