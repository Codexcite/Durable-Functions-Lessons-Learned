using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.VersionedOrchestration
{
	public class VersionedActivityV2 : BaseLogged
	{

		[FunctionName(nameof(VersionedActivityV2))]
		public string SayHello([ActivityTrigger] string name) 
		{
			_log.Trace(LogEventLevel.Information, message: $"Saying goodbye to {name}.");
			return $"Bye {name}!";
		}

	}
}