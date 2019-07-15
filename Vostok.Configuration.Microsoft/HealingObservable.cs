using System;
using System.Threading.Tasks;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Microsoft
{
    internal class HealingObservable : IObservable<(ISettingsNode settings, Exception error)>,
        IObserver<(ISettingsNode settings, Exception error)>,
        IDisposable
    {
        private readonly IConfigurationSource source;
        private volatile IDisposable sourceSubscription;
        private volatile IObserver<(ISettingsNode settings, Exception error)> externalObserver;

        public HealingObservable(IConfigurationSource source)
            => this.source = source;

        public IDisposable Subscribe(IObserver<(ISettingsNode settings, Exception error)> observer)
        {
            externalObserver = observer;
            EnsureSubscribedToInnerSource();
            return this;
        }

        public void OnCompleted()
            => externalObserver.OnCompleted();

        public void OnError(Exception error)
        {
            OnNext((null, error));
            ResubscribeAfterDelay();
        }

        public void OnNext((ISettingsNode settings, Exception error) value)
            => externalObserver.OnNext(value);

        public void Dispose()
            => sourceSubscription?.Dispose();

        private void EnsureSubscribedToInnerSource()
        {
            if (sourceSubscription == null)
                sourceSubscription = source.Observe().Subscribe(this);
        }

        private void ResubscribeAfterDelay()
        {
            if (sourceSubscription == null)
                return;

            sourceSubscription.Dispose();
            sourceSubscription = null;

            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ => EnsureSubscribedToInnerSource());
        }
    }
}