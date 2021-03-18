using System;

namespace Codexcite.DurableFunctions.Services.Time
{
	public interface ITimeService
	{
		DateTimeOffset UtcNow { get; }
	}
}