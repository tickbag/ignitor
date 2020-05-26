using FluentAssertions;
using Ignitor.Immutables;
using System;
using Xunit;

namespace Ignitor.Tests
{
    public class ImmutableExtensionsTests
    {
        public class TheMakeImmutableMethod
        {
            [Fact]
            public void ShouldCreateImmutable_WhenCalledWithInt()
            {
                ShouldCreateImmutable_WhenCalled(100);
            }

            [Fact]
            public void ShouldCreateImmutable_WhenCalledWithString()
            {
                ShouldCreateImmutable_WhenCalled("Blah");
            }

            [Fact]
            public void ShouldCreateImmutable_WhenCalledWithGuid()
            {
                ShouldCreateImmutable_WhenCalled(Guid.NewGuid());
            }

            [Fact]
            public void ShouldCreateImmutable_WhenCalledWithClass()
            {
                ShouldCreateImmutable_WhenCalled(new TestClass());
            }

            [Fact]
            public void ShouldCreateImmutable_WhenCalledWithNull()
            {
                ShouldCreateImmutable_WhenCalled((object)null);
            }

            private void ShouldCreateImmutable_WhenCalled<T>(T subject)
            {
                // Arrange
                var subjectType = typeof(Immutable<T>);

                // Act
                var immutable = subject.MakeImmutable();

                // Ssert
                immutable.Should().BeOfType(subjectType);
            }

            private class TestClass
            {
                public int Prop1 { get; set; }
                public string Prop2 { get; set; }
                public Guid Prop3 { get; set; }
                public object Prop4 { get; set; } = null;
            }
        }
    }
}
