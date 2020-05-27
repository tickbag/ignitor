using FluentAssertions;
using Ignitor.State;
using Ignitor.StateMonitor;
using Ignitor.Transient;
using Moq;
using System;
using Xunit;

namespace Ignitor.Tests.State
{
    public class ScopedStateFactoryTests
    {
        public class TheCreateScopeMethod
        {
            [Fact]
            public void ShouldReturnScopeStateInstance_WhenCalled()
            {
                // Arrange
                var services = new Mock<IServiceProvider>();
                services.Setup(s => s.GetService(It.Is<Type>(t => t == typeof(IIgnitorStore<Guid, object>))))
                    .Returns(() => new IgnitorStore<Guid, object>());
                services.Setup(s => s.GetService(It.Is<Type>(t => t == typeof(IStateMonitor<Guid, object>))))
                    .Returns(() => new StateMonitor<Guid, object>());

                var factory = new ScopedStateFactory(services.Object);

                // Act
                var result = factory.CreateScope<Guid, object>(null);

                // Assert
                result.Should().BeOfType<ScopedState<Guid, object>>();
            }

            [Fact]
            public void ShouldRequestServices_WhenCalled()
            {
                // Arrange
                var services = new Mock<IServiceProvider>();
                services.Setup(s => s.GetService(It.Is<Type>(t => t == typeof(IIgnitorStore<Guid, object>))))
                    .Returns(() => new IgnitorStore<Guid, object>());
                services.Setup(s => s.GetService(It.Is<Type>(t => t == typeof(IStateMonitor<Guid, object>))))
                    .Returns(() => new StateMonitor<Guid, object>());

                var factory = new ScopedStateFactory(services.Object);

                // Act
                var result = factory.CreateScope<Guid, object>(null);

                // Assert
                services.Verify(s => s.GetService(It.Is<Type>(t => t == typeof(IIgnitorStore<Guid, object>))), Times.Once);
                services.Verify(s => s.GetService(It.Is<Type>(t => t == typeof(IStateMonitor<Guid, object>))), Times.Once);
            }
        }
    }
}
