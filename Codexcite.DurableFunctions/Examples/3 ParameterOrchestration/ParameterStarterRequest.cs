using System;
using System.Collections.Generic;
using System.Text;
using Codexcite.DurableFunctions.Examples.ParameterOrchestration.Model;

namespace Codexcite.DurableFunctions.Examples.ParameterOrchestration
{
	class ParameterStarterRequest
	{
		public int? Age { get; set; }
		public Gender? Gender { get; set; }
	}
}
