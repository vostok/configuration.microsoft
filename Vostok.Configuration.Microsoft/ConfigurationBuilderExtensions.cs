using Microsoft.Extensions.Configuration;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddVostok(
            this IConfigurationBuilder configurationBuilder,
            IConfigurationSource vostokConfigurationSource) =>
            configurationBuilder.Add(new VostokConfigurationSource(vostokConfigurationSource));
    }
}