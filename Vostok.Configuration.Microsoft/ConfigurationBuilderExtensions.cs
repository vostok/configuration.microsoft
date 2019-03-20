using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    [PublicAPI]
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds vostok configuration source as provider for <see cref="IConfiguration"/>.
        /// </summary>
        public static IConfigurationBuilder AddVostok(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] IConfigurationSource vostokConfigurationSource)
        {
            if (configurationBuilder == null)
                throw new ArgumentNullException(nameof(configurationBuilder));
            if (vostokConfigurationSource == null)
                throw new ArgumentNullException(nameof(vostokConfigurationSource));

            return configurationBuilder.Add(new VostokConfigurationSource(vostokConfigurationSource));
        }
    }
}