namespace Codexcite.DurableFunctions.Examples._4_EntityOrchestration
{
	public class ThrottledFunctionCall
	{
		public ThrottledFunctionCall(string name, string? throttleKey = null, object? input = null)
		{
			Name = name;
			ThrottleKey = throttleKey ?? name;
			Input = input;
		}

		public string Name { get; set; }
		public object? Input { get; set; }
		public string ThrottleKey { get; set; }
	}
}