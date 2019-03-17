using System;
using Microsoft.Extensions.Configuration;
using Vostok.Configuration.Abstractions.SettingsTree;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    internal class VostokConfigurationProvider : ConfigurationProvider, IObserver<ValueTuple<ISettingsNode, Exception>>
    {
        public VostokConfigurationProvider(IConfigurationSource vostokConfigurationSource)
        {
            vostokConfigurationSource.Observe().Subscribe(this);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext((ISettingsNode, Exception) value)
        {
            if (value.Item2 != null)
            {
                OnError(value.Item2);
                return;
            }

            Data = value.Item1.Flatten();
            OnReload();
        }
    }
}