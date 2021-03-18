using System;
using System.Collections.Generic;
using System.Text;

namespace Codexcite.DurableFunctions.Examples.VersionedOrchestration
{
	public class VersionedOptions
	{
		public string VersionOrchestrator { get; set; } = nameof(VersionedOrchestratorV1); // default versions, can be overriden in settings.json
		public string VersionActivity { get; set; } = nameof(VersionedActivityV1);
		public string VersionSubOrchestrator { get; set; } = nameof(VersionedSubOrchestratorV1);
	}
}
