using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using IConfigurationSource = Vostok.Configuration.Abstractions.IConfigurationSource;

namespace Vostok.Configuration.Microsoft.Tests
{
    public class ConfigurationProvider_Tests
    {
        private IConfiguration configuration;
        private IConfigurationSource vostokConfigurationSource;
        private VostokConfigurationProvider configurationProvider;

        private Action<(ISettingsNode settings, Exception error)> updateSettingsDelegate;
        private IObservable<(ISettingsNode settings, Exception error)> subscriptionObservable;

        [SetUp]
        public void Setup()
        {
            vostokConfigurationSource = Substitute.For<IConfigurationSource>();
            subscriptionObservable = Substitute.For<IObservable<(ISettingsNode settings, Exception error)>>();
            vostokConfigurationSource.Observe().Returns(subscriptionObservable);
            
            subscriptionObservable
                .Subscribe(Arg.Any<IObserver<(ISettingsNode settings, Exception error)>>())
                .Returns(
                    call =>
                    {
                        var observer = call.Arg<IObserver<(ISettingsNode settings, Exception error)>>();

                        updateSettingsDelegate = newSettings => observer.OnNext(newSettings);

                        return Substitute.For<IDisposable>();
                    });
            
            configurationProvider = new VostokConfigurationProvider(vostokConfigurationSource);
            
            configuration = new ConfigurationRoot(new List<IConfigurationProvider>() { configurationProvider });
        }

        [Test]
        public void Should_SubscribeToChangesOnCreation()
        {
            subscriptionObservable.Received(1).Subscribe(configurationProvider);
        }

        [Test]
        public void Should_SetSimpleValue()
        {
            SetSettings(new ValueNode("name", "value"));
            configuration["name"].Should().Be("value");
        }

        [Test]
        public void Should_UpdateSimpleValue()
        {
            SetSettings(new ValueNode("name", "value"));
            configuration["name"].Should().Be("value");
            
            SetSettings(new ValueNode("name", "new value"));
            configuration["name"].Should().Be("new value");
        }

        [Test]
        public void Should_IgnoreExceptions()
        {
            Assert.DoesNotThrow(() => SetException(new Exception()));
        }
        
        private void SetSettings(ISettingsNode settingsNode)
        {
            updateSettingsDelegate((settingsNode, null));
        }

        private void SetException(Exception exception)
        {
            updateSettingsDelegate((null, exception));
        }
    }
}