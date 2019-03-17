using Microsoft.Extensions.Configuration;

namespace Vostok.Configuration.Microsoft
{
    internal class VostokConfigurationSource : IConfigurationSource
    {
        private readonly Abstractions.IConfigurationSource vostokConfigurationSource;

        public VostokConfigurationSource(Abstractions.IConfigurationSource vostokConfigurationSource)
        {
            this.vostokConfigurationSource = vostokConfigurationSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new VostokConfigurationProvider(vostokConfigurationSource);
    }
}