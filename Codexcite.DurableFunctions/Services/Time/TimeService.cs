using System;
using System.Collections.Generic;
using System.Text;

namespace Codexcite.DurableFunctions.Services.Time
{
	internal class TimeService : ITimeService
	{
		public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
	}
}
