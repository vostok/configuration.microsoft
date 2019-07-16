using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    [PublicAPI]
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="VostokConfigurationSource"/> based on given <paramref name="vostokSource"/> to given for <see cref="IConfigurationBuilder" />.
        /// </summary>
        [NotNull]
        public static IConfigurationBuilder AddVostok(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] IConfigurationSource vostokSource)
        {
            if (configurationBuilder == null)
                throw new ArgumentNullException(nameof(configurationBuilder));
            if (vostokSource == null)
                throw new ArgumentNullException(nameof(vostokSource));

            return configurationBuilder.Add(new VostokConfigurationSource(vostokSource));
        }
    }
}