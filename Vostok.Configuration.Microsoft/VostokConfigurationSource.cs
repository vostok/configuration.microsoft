using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Vostok.Configuration.Microsoft
{
    /// <inheritdoc />
    [PublicAPI]
    public class VostokConfigurationSource : IConfigurationSource
    {
        private readonly Abstractions.IConfigurationSource vostokConfigurationSource;

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