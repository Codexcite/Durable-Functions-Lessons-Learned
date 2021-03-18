using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Codexcite.DurableFunctions.Examples.CustomStatusOrchestration
{
	public class CustomStatusActivity : BaseLogged
	{
		[FunctionName(nameof(CustomStatusActivity))]
		public string SayHello([ActivityTrigger] string name)
		{
			_log.Information($"Saying hello to {name}.");
			return $"Hello {name}!";
		}
	}
}