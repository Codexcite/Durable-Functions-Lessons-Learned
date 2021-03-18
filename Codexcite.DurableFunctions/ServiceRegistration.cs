using Codexcite.DurableFunctions.Examples.VersionedOrchestration;
using Codexcite.DurableFunctions.Services.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codexcite.DurableFunctions
{
	internal static class ServiceRegistration
	{
		internal static IConfiguration? Configuration { get; set; }

		internal static IServiceCollection AddTime(this IServiceCollection services)
		{
			services.AddSingleton<ITimeService, TimeService>();
			return services;
		}
		internal static IServiceCollection AddVersionedOptions(this IServiceCollection services)
		{
			services.AddOptions<VersionedOptions>()
							.Configure<IConfiguration>((settings, configuration) => { configuration.GetSection(nameof(VersionedOptions)).Bind(settings); });
			return services;
		}
	}
}