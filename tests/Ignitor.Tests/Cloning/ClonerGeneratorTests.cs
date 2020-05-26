using FluentAssertions;
using Ignitor.Cloning;
using Ignitor.Tests.TestClasses;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Ignitor.Tests.Cloning
{
#pragma warning disable xUnit1024 // Test methods cannot have overloads
    public class ClonerGeneratorTests
    {
        public class Int32Clones : ClonerGeneratorTests<int>
        {
            [Theory]
            [InlineData(100)]
            public override void ShouldDeepCloneObject_WhenGivenObject(int value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Fact]
            public void ShouldBeTwoDifferentObject_WhenCloned()
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(100, (v) => 60);
            }
        }

        public class StringClones : ClonerGeneratorTests<string>
        {
            [Theory]
            [InlineData("Blah")]
            public override void ShouldDeepCloneObject_WhenGivenObject(string value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Fact]
            public void ShouldBeTwoDifferentObject_WhenCloned()
            {
                base.ShouldBeTwoDifferentObject_WhenCloned("Blah", (v) => "Test");
            }
        }

        public class GuidClones : ClonerGeneratorTests<Guid>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        Guid.NewGuid()
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(Guid value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(Guid value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => Guid.NewGuid());
            }
        }

        public class dateTimeClones : ClonerGeneratorTests<DateTime>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        DateTime.UtcNow
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(DateTime value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(DateTime value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => DateTime.UtcNow);
            }
        }

        public class EnumClones : ClonerGeneratorTests<TestEnum>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        TestEnum.First
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(TestEnum value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(TestEnum value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => TestEnum.Second);
            }
        }

        public class SimpleTestClassClones : ClonerGeneratorTests<SimpleTestClass>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        new SimpleTestClass
                        {
                            Prop1 = 10,
                            Prop2 = "Blah"
                        }
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(SimpleTestClass value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(SimpleTestClass value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => { v.Prop1 = 2; v.Prop2 = "Test"; return v; });
            }
        }

        public class ArrayOfSimpleTestClassClones : ClonerGeneratorTests<SimpleTestClass[]>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        new SimpleTestClass[2]
                        {
                            new SimpleTestClass
                            {
                            Prop1 = 10,
                            Prop2 = "Blah"
                            },
                            new SimpleTestClass
                            {
                            Prop1 = 20,
                            Prop2 = "Blah Blah"
                            }
                        }
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(SimpleTestClass[] value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(SimpleTestClass[] value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => { v[0].Prop1 = 2; v[1].Prop2 = "Test"; return v; });
            }
        }

        public class ComplexTestClassClones : ClonerGeneratorTests<ComplexTestClass>
        {
            public static IEnumerable<object[]> TestOperands =>
                new List<object[]>
                {
                    new object[]
                    {
                        new ComplexTestClass
                        {
                            Id = Guid.NewGuid(),
                            Name = "John Doe"
                        }
                    }
                };

            [Theory]
            [MemberData(nameof(TestOperands))]
            public override void ShouldDeepCloneObject_WhenGivenObject(ComplexTestClass value)
            {
                base.ShouldDeepCloneObject_WhenGivenObject(value);
            }

            [Theory]
            [MemberData(nameof(TestOperands))]
            public void ShouldBeTwoDifferentObject_WhenCloned(ComplexTestClass value)
            {
                base.ShouldBeTwoDifferentObject_WhenCloned(value, (v) => { v.Things.Prop1 = 10; return v; });
            }
        }

        public class UnsupportedClones
        {
            [Theory]
            [InlineData(typeof(RecursiveTestClass), "- Maximum Object graph depth exceeded. Please flatten your object structure to continue.")]
            [InlineData(typeof(ContainsListTestClass), "- Unable to clone types derived from IList. Consider using an array instead.")]
            [InlineData(typeof(ContainsDictionaryTestClass), "- Unable to clone types derived from IDictionary. Consider moving these objects to their own state.")]
            [InlineData(typeof(ContainsInterfaceTestClass), "- Unable to clone interfaces. Use concrete types and implementations instead.")]
            public void ShouldFailToCompile_WhenGetClonerIsCalled(Type type, string expectedError)
            {
                // Arrange
                var clonerGeneratorType = typeof(ClonerGenerator<>).MakeGenericType(new[] { type });
                var clonerGenerator = Activator.CreateInstance(clonerGeneratorType);
                var clonerGetClonerMethod = clonerGenerator.GetType().GetMethod("GetCloner");

                // Act
                try
                {
                    clonerGetClonerMethod.Invoke(clonerGenerator, null);
                }
                catch (TargetInvocationException tiex)
                {
                    if (tiex.InnerException.GetType() == typeof(InvalidOperationException))
                    {
                        tiex.InnerException.Message.Should().EndWith(expectedError);
                        return;
                    }
                    throw;
                }

                throw new Exception("No exception was thrown by the test");
            }
        }
    }
#pragma warning restore xUnit1024 // Test methods cannot have overloads

    public abstract class ClonerGeneratorTests<T>
    {
        [Fact]
        public void ShouldCompileClonerFunction_WhenGivenAType()
        {
            // Arrange
            var cloneGenerator = new ClonerGenerator<T>();

            // Act
            Action result = () => cloneGenerator.GetCloner();

            // Assert
            result.Should().NotThrow();
        }

        public virtual void ShouldDeepCloneObject_WhenGivenObject(T value)
        {
            // Arrange
            var cloneGenerator = new ClonerGenerator<T>();
            var cloner = cloneGenerator.GetCloner();

            // Act
            var clone = cloner(value);

            // Assert
            clone.Should().BeEquivalentTo(value);
        }

        public virtual void ShouldBeTwoDifferentObject_WhenCloned(T value, Func<T, T> change)
        {
            // Arrange
            var cloneGenerator = new ClonerGenerator<T>();
            var cloner = cloneGenerator.GetCloner();

            // Act
            var clone = cloner(value);
            clone = change(clone);

            // Assert
            clone.Should().NotBeEquivalentTo(value);
        }   
    }
}
