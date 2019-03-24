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
        private readonly IConfigurationSource configurationSource;
        private IObserver<(ISettingsNode settings, Exception error)> configurationObserver;
        private IDisposable activeObservableSubscription;

        public HealingObservable(IConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }

        public IDisposable Subscribe(IObserver<(ISettingsNode settings, Exception error)> observer)
        {
            configurationObserver = observer;
            EnsureSubscribedToInnerSource();
            return this;
        }

        public void OnCompleted()
        {
            configurationObserver.OnCompleted();
        }

        public void OnError(Exception error)
        {
            OnNext((null, error));
            ResubscribeAfterDelay();
        }

        public void OnNext((ISettingsNode settings, Exception error) value)
        {
            configurationObserver.OnNext(value);
        }

        public void Dispose()
        {
            activeObservableSubscription?.Dispose();
        }

        private void EnsureSubscribedToInnerSource()
        {
            if (activeObservableSubscription == null)
                activeObservableSubscription = configurationSource.Observe().Subscribe(this);
        }

        private void ResubscribeAfterDelay()
        {
            if (activeObservableSubscription == null)
                return;

            activeObservableSubscription.Dispose();
            activeObservableSubscription = null;

            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ => EnsureSubscribedToInnerSource());
        }
    }
}