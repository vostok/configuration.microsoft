using System;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Microsoft
{
    internal class HealingConfigurationSource : IConfigurationSource
    {
        private readonly IConfigurationSource baseConfigurationSource;

        public HealingConfigurationSource(IConfigurationSource baseConfigurationSource)
            => this.baseConfigurationSource = baseConfigurationSource;

        public IObservable<(ISettingsNode settings, Exception error)> Observe()
            => new HealingObservable(baseConfigurationSource);
    }
}