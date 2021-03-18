using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Codexcite.DurableFunctions.Utils
{
	static class Json
	{
		public static readonly JsonSerializerSettings MessageJsonSerializerSettings = new JsonSerializerSettings
																																									{
																																										ContractResolver = new CamelCasePropertyNamesContractResolver(),
																																										NullValueHandling = NullValueHandling.Ignore,
																																										Formatting = Formatting.None,
																																										Converters = new List<JsonConverter>(new[] { new StringEnumConverter() }),
																																										TypeNameHandling = TypeNameHandling.Objects
																																									};
	}
}
