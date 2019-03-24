using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Vostok.Configuration.Abstractions.SettingsTree;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft
{
    /// <inheritdoc cref="ConfigurationProvider" />
    public class VostokConfigurationProvider : ConfigurationProvider, IObserver<ValueTuple<ISettingsNode, Exception>>
    {
        private readonly TaskCompletionSource<byte> configurationInitialized = new TaskCompletionSource<byte>();

        /// <summary>
        ///     Creates new configuration provider for <see cref="Configuration" /> using vostok
        /// <see
        ///     cref="Abstractions.IConfigurationSource" />
        /// .
        /// </summary>
        public VostokConfigurationProvider([NotNull] IConfigurationSource vostokConfigurationSource)
        {
            if (vostokConfigurationSource == null)
                throw new ArgumentNullException(nameof(vostokConfigurationSource));

            new HealingConfigurationSource(vostokConfigurationSource).Observe().Subscribe(this);
        }

        /// <inheritdoc />
        public override void Load()
        {
            var _ = configurationInitialized.Task.Result;
        }

        /// <inheritdoc />
        public void OnCompleted()
        {
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            configurationInitialized.TrySetException(error);
        }

        /// <inheritdoc />
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