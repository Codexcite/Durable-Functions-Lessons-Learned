using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codexcite.DurableFunctions.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IConfiguration GetConfiguration(this IServiceCollection services)
		{
			var provider = services.BuildServiceProvider();
			var configuration = provider.GetService<IConfiguration>();
			return configuration;
		}

		public static IServiceCollection RegisterOptions<TOptions>(this IServiceCollection services, string? sectionName = null) where TOptions : class
		{
			services.AddOptions<TOptions>()
						.Configure<IConfiguration>((settings, configuration) =>
																			{
																				configuration.GetSection(sectionName ?? typeof(TOptions).Name).Bind(settings);
																			});
			return services;
		}
	}
}