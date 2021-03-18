using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Codexcite.DurableFunctions.Services.MessageSerializer
{

	public class MessageSerializerSettings : IMessageSerializerSettingsFactory
	{
		public JsonSerializerSettings CreateJsonSerializerSettings() => Json.MessageJsonSerializerSettings;
	}
}
