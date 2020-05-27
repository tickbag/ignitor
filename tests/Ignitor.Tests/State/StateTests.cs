using FluentAssertions;
using Ignitor.State;
using Ignitor.Transient;
using Moq;
using System;
using Xunit;
using StateInstance = Ignitor.State.State;

namespace Ignitor.Tests.State
{
    public class StateTests
    {
        public class TheScopeMethod
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("My Scope")]
            [InlineData(300)]
            public void ShouldCreateAScope_WhenItDoesntExist(object scopeName)
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);

                // Act
                var scope = scopeName == null ? state.Scope<Guid, object>() : state.Scope<Guid, object>(scopeName);
                var result = scopeName == null ? state.HasScope<Guid, object>() : state.HasScope<Guid, object>(scopeName);

                // Assert
                result.Should().BeTrue();
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("My Scope")]
            [InlineData(300)]
            public void ShouldReturnSameScope_WhenScopeMatches(object scopeName)
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);

                // Act
                var scope1 = scopeName == null ? state.Scope<Guid, object>() : state.Scope<Guid, object>(scopeName);
                var scope2 = scopeName == null ? state.Scope<Guid, object>() : state.Scope<Guid, object>(scopeName);

                // Assert
                scope1.Should().BeSameAs(scope2);
            }

            [Fact]
            public void ShouldReturnDifferentScopes_WhenTypesMatchButScopeIsDifferent()
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);

                // Act
                var scope1 = state.Scope<Guid, object>();
                var scope2 = state.Scope<Guid, object>("My Scope");

                // Assert
                scope1.Should().NotBeSameAs(scope2);
            }

            [Fact]
            public void ShoudlReturnDifferentScopes_WhenTypesAreDifferent()
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));
                factoryMock.Setup(f => f.CreateScope<Guid, Guid[]>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, Guid[]>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);

                // Act
                var scope1 = state.Scope<Guid, object>();
                var scope2 = state.Scope<Guid, Guid[]>();

                // Assert
                scope1.Should().NotBeSameAs(scope2);
            }
        }

        public class TheHasScopeMethod
        {
            [Theory]
            [InlineData(null, null, true)]
            [InlineData(null, "", true)]
            [InlineData("", null, true)]
            [InlineData(null, "My Scope", false)]
            [InlineData("My Scope 2", null, false)]
            [InlineData("My Scope 2", "My Scope", false)]
            [InlineData("My Scope", "My Scope", true)]
            public void ShouldReturnExpected_WhenCalledWithSameTypeParams(object createScope, object checkScope, bool expected)
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);
                var scope = createScope == null ? state.Scope<Guid, object>() : state.Scope<Guid, object>(createScope);

                // Act
                var result = checkScope == null ? state.HasScope<Guid, object>() : state.HasScope<Guid, object>(checkScope);

                // Assert
                result.Should().Be(expected);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("My Scope")]
            public void ShouldReturnFalse_WhenTypeParamsDontMatch(object checkScope)
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, null, null));

                var state = new StateInstance(factoryMock.Object);
                var scope = checkScope == null ? state.Scope<Guid, object>() : state.Scope<Guid, object>(checkScope);

                // Act
                var result = checkScope == null ? state.HasScope<Guid, Guid[]>() : state.HasScope<Guid, Guid[]>(checkScope);

                // Assert
                result.Should().BeFalse();
            }
        }

        public class TheRemoveScopeMethod
        {
            [Fact]
            public void ShouldRemoveSpecificScope_WhenCalled()
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, new IgnitorStore<Guid, object>(), null));

                var state = new StateInstance(factoryMock.Object);
                var scope1 = state.Scope<Guid, object>("1");
                var scope2 = state.Scope<Guid, object>("2");
                var scope3 = state.Scope<Guid, object>("3");

                // Act
                state.RemoveScope<Guid, object>("2");

                // Assert
                state.HasScope<Guid, object>("1").Should().BeTrue();
                state.HasScope<Guid, object>("2").Should().BeFalse();
                state.HasScope<Guid, object>("3").Should().BeTrue();
            }
        }

        public class TheDisposeMethod
        {
            [Fact]
            public void ShouldRemoveAllScopes_WhenCalled()
            {
                // Arrange
                var factoryMock = new Mock<IScopedStateFactory>();
                factoryMock.Setup(f => f.CreateScope<Guid, object>(It.IsAny<IState>()))
                    .Returns(() => new ScopedState<Guid, object>(null, null, new IgnitorStore<Guid, object>(), null));

                var state = new StateInstance(factoryMock.Object);
                var scope1 = state.Scope<Guid, object>("1");
                var scope2 = state.Scope<Guid, object>("2");
                var scope3 = state.Scope<Guid, object>("3");

                // Act
                state.Dispose();

                // Assert
                state.HasScope<Guid, object>("1").Should().BeFalse();
                state.HasScope<Guid, object>("2").Should().BeFalse();
                state.HasScope<Guid, object>("3").Should().BeFalse();
            }
        }
    }
}
