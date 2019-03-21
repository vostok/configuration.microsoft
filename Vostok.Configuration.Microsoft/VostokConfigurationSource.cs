using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Vostok.Configuration.Microsoft
{
    /// <inheritdoc />
    public class VostokConfigurationSource : IConfigurationSource
    {
        private readonly Abstractions.IConfigurationSource vostokConfigurationSource;

        /// <summary>
        ///     Creates new configuration source for <see cref="Configuration" /> using vostok <see cref="IConfigurationSource" />.
        /// </summary>
        public VostokConfigurationSource([NotNull] Abstractions.IConfigurationSource vostokConfigurationSource)
        {
            this.vostokConfigurationSource =
                vostokConfigurationSource ?? throw new ArgumentNullException(nameof(vostokConfigurationSource));
        }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new VostokConfigurationProvider(vostokConfigurationSource);
    }
}