using System;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public class NotifyPayload
	{
		public string? Phone { get; set; }
		public string? EventName { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
	}
}