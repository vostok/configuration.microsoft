using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Vostok.Configuration.Abstractions.SettingsTree;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    internal class VostokConfigurationProvider : ConfigurationProvider, IObserver<ValueTuple<ISettingsNode, Exception>>
    {
        private readonly TaskCompletionSource<byte> configurationInitialized = new TaskCompletionSource<byte>();

        public VostokConfigurationProvider(IConfigurationSource vostokConfigurationSource)
        {
            vostokConfigurationSource.Observe().Subscribe(this);
        }

        public override void Load()
        {
            var _ = configurationInitialized.Task.Result;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            configurationInitialized.TrySetException(error);
        }

        public void OnNext((ISettingsNode, Exception) value)
        {
            var (settings, exception) = value;

            if (exception != null)
            {
                OnError(exception);
                return;
            }

            Data = settings.Flatten();
            OnReload();

            configurationInitialized.TrySetResult(1);
        }
    }
}