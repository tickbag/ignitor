using Castle.DynamicProxy.Generators;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Ignitor.Immutables;
using Ignitor.Tests.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Ignitor.Tests.Immutables
{
    public class ImmutableTests
    {
        public class TheConstructor
        {
            [Theory]
            [InlineData(typeof(int))]
            [InlineData(typeof(string))]
            [InlineData(typeof(Guid))]
            [InlineData(typeof(DateTime))]
            [InlineData(typeof(TestEnum))]
            [InlineData(typeof(SimpleTestClass))]
            [InlineData(typeof(SimpleTestClass[]))]
            [InlineData(typeof(ComplexTestClass))]
            public void ShouldConstructAnImmutable_WhenGivenAValidObject(Type type)
            {
                // Arrange
                object value;
                switch (type)
                {
                    case Type strType when strType == typeof(string):
                        value = "Blah";
                        break;
                    case Type arrType when arrType.IsArray:
                        value = Activator.CreateInstance(type, new object[] { 3 });
                        break;
                    default:
                        value = Activator.CreateInstance(type);
                        break;
                }
                var expectedType = typeof(Immutable<>).MakeGenericType(new[] { type });
                
                // Act
                var result = Immutable<object>.CreateImmutable(type, value);

                // Assert
                result.Should().BeOfType(expectedType);
            }

            [Theory]
            [InlineData(typeof(RecursiveTestClass))]
            [InlineData(typeof(ContainsListTestClass))]
            [InlineData(typeof(ContainsDictionaryTestClass))]
            [InlineData(typeof(ContainsInterfaceTestClass))]
            public void ShouldThrowException_WhenGivenAnInvalidObject(Type type)
            {
                // Arrange
                var value = Activator.CreateInstance(type);
                var expectedType = typeof(Immutable<>).MakeGenericType(new[] { type });

                // Act
                Action result = () => Immutable<object>.CreateImmutable(type, value);

                // Assert
                result.Should().Throw<TargetInvocationException>();
            }
        }

        public class TheIsArrayProperty
        {
            [Fact]
            public void ShouldReturnTrue_WhenObjectIsAnArray()
            {
                // Arrange
                var value = new SimpleTestClass[3];
                var immutable = new Immutable<SimpleTestClass[]>(value);

                // Act
                var result = immutable.IsArray;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnFalse_WhenObjectIsNotAnArray()
            {
                // Arrange
                var value = new SimpleTestClass();
                var immutable = new Immutable<SimpleTestClass>(value);

                // Act
                var result = immutable.IsArray;

                // Assert
                result.Should().BeFalse();
            }
        }

        public class TheIsValueTypeProperty
        {
            [Fact]
            public void ShouldReturnTrue_WhenObjectIsAnInt()
            {
                // Arrange
                var value = 100;
                var immutable = new Immutable<int>(value);

                // Act
                var result = immutable.IsValueType;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnTrue_WhenObjectIsAString()
            {
                // Arrange
                var value = "Blah";
                var immutable = new Immutable<string>(value);

                // Act
                var result = immutable.IsValueType;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnTrue_WhenObjectIsAnEnum()
            {
                // Arrange
                var value = TestEnum.First;
                var immutable = new Immutable<TestEnum>(value);

                // Act
                var result = immutable.IsValueType;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnTrue_WhenObjectIsAnDateTime()
            {
                // Arrange
                var value = DateTime.UtcNow;
                var immutable = new Immutable<DateTime>(value);

                // Act
                var result = immutable.IsValueType;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnFalse_WhenObjectIsNotAValueType()
            {
                // Arrange
                var value = new SimpleTestClass();
                var immutable = new Immutable<SimpleTestClass>(value);

                // Act
                var result = immutable.IsValueType;

                // Assert
                result.Should().BeFalse();
            }
        }

        public class TheIsNullProperty
        {
            [Fact]
            public void ShouldReturnTrue_WhenObjectIsNull()
            {
                // Arrange
                var immutable = new Immutable<SimpleTestClass>(null);

                // Act
                var result = immutable.IsNull;

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public void ShouldReturnFalse_WhenObjectIsNotNull()
            {
                // Arrange
                var value = new SimpleTestClass();
                var immutable = new Immutable<SimpleTestClass>(value);

                // Act
                var result = immutable.IsNull;

                // Assert
                result.Should().BeFalse();
            }
        }

        public class TheIndexer
        {
            [Fact]
            public void ShouldReturnImmutableObject_WhenObjectIsArray()
            {
                // Arrange
                var value = new SimpleTestClass[3]
                {
                    new SimpleTestClass(),
                    new SimpleTestClass(),
                    new SimpleTestClass()
                };
                var immutable = new Immutable<SimpleTestClass[]>(value);

                // Act
                var result = immutable[0];

                // Assert
                result.Should().BeOfType<Immutable<SimpleTestClass>>();
            }

            [Fact]
            public void ShouldReturnTheSameImmutableObjectThatWasEncapsulated_WhenObjectIsArray()
            {
                // Arrange
                var value = new SimpleTestClass[3]
                {
                    new SimpleTestClass { Prop1 = 1 },
                    new SimpleTestClass { Prop1 = 2 },
                    new SimpleTestClass { Prop1 = 3 }
                };
                var immutable = new Immutable<SimpleTestClass[]>(value);

                // Act
                var result = immutable[0];

                // Assert
                result.Emit().Should().BeEquivalentTo(value[0]);
            }

            [Fact]
            public void ShouldThrowException_WhenObjectIsNotAnArray()
            {
                // Arrange
                var value = new SimpleTestClass();
                var immutable = new Immutable<SimpleTestClass>(value);

                // Act
                Action result = () => _ = immutable[0];

                // Assert
                result.Should().Throw<InvalidOperationException>().WithMessage("Immutable of type 'SimpleTestClass' is not an array.");
            }

            [Fact]
            public void ShouldThrowException_WhenObjectIsArrayAndIndexIsOutOfBounds()
            {
                // Arrange
                var value = new SimpleTestClass[3]
                {
                    new SimpleTestClass(),
                    new SimpleTestClass(),
                    new SimpleTestClass()
                };
                var immutable = new Immutable<SimpleTestClass[]>(value);

                // Act
                Action result = () => _ = immutable[4];

                // Assert
                result.Should().Throw<IndexOutOfRangeException>().WithMessage("Index was out of range. Must be non-negative and less than the size of the collection.");
            }
        }

        public class TheEmitMethod
        {
            [Fact]
            public void ShouldReturnObjectWithSameType_WhenCalled()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Emit();

                // Assert
                result.Should().BeOfType<ComplexTestClass>();
            }

            [Fact]
            public void ShouldReturnClonedObject_WhenCalled()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Emit();

                // Assert
                result.Should().BeEquivalentTo(value);
            }
        }

        public class TheValueMethod
        {
            [Fact]
            public void ShouldReturnValue_WhenPropertyExistsAndIsAValueType()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Value("Id");

                // Assert
                result.Should().Be(value.Id);
            }

            [Fact]
            public void ShouldThrowException_WhenPropertyDoesNotExist()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Value("Thing");

                // Assert
                result.Should().Throw<ArgumentException>().WithMessage("Property 'Thing' does not exist on this object.");
            }

            [Fact]
            public void ShouldThrowException_WhenPropertyIsNotAValueType()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Value("Things");

                // Assert
                result.Should().Throw<ArgumentException>().WithMessage("Property 'Things' is not a immutable value type.");
            }
        }

        public class TheRefMethod
        {
            [Fact]
            public void ShouldReturnImmutable_WhenPropertyExists()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Ref("Things");

                // Assert
                result.Should().BeOfType<Immutable<SimpleTestClass>>();
            }

            [Fact]
            public void ShouldThrowException_WhenPropertyDoesNotExist()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Ref("Thing");

                // Assert
                result.Should().Throw<ArgumentException>().WithMessage("Property 'Thing' does not exist on this object.");
            }
        }

        public class TheArrayLengthMethod
        {
            [Fact]
            public void ShouldReturnArrayLength_WhenObjectIsAnArray()
            {
                // Arrange
                var value = new SimpleTestClass[3]
                {
                    new SimpleTestClass { Prop1 = 1 },
                    new SimpleTestClass { Prop1 = 2 },
                    new SimpleTestClass { Prop1 = 3 }
                };
                var immutable = new Immutable<SimpleTestClass[]>(value);

                // Act
                var result = immutable.ArrayLength();

                // Assert
                result.Should().Be(3);
            }

            [Fact]
            public void ShouldThrowException_WhenObjectIsNotAnArray()
            {
                // Arrange
                var value = new SimpleTestClass();
                var immutable = new Immutable<SimpleTestClass>(value);

                // Act
                Action result = () => immutable.ArrayLength();

                // Assert
                result.Should().Throw<InvalidOperationException>().WithMessage("Immutable of type 'SimpleTestClass' is not an array.");
            }
        }

        public class TheArrayMethod
        {
            [Fact]
            public void ShouldReturnArrayOfImmutables_WhenPropertyIsAnArray()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Array<SimpleTestClass>("SomeThings");

                // Assert
                result.Should().BeOfType<IImmutable<SimpleTestClass>[]>();
            }

            [Fact]
            public void ShouldReturnArrayOfSameSize_WhenPropertyIsAnArray()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Array<SimpleTestClass>("SomeThings");

                // Assert
                result.Length.Should().Be(value.SomeThings.Length);
            }

            [Fact]
            public void ShouldThrowException_WhenPropertyIsNotAnArray()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Array<SimpleTestClass>("Things");

                // Assert
                result.Should().Throw<ArgumentException>().WithMessage("Property 'Things' is not an Array type.");
            }

            [Fact]
            public void ShouldThrowException_WhenPropertyDoesNotExist()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Array<SimpleTestClass>("Thing");

                // Assert
                result.Should().Throw<ArgumentException>().WithMessage("Property 'Thing' does not exist on this object.");
            }
        }

        public class TheCheckMethod
        {
            [Theory]
            [InlineData("Blah", true)]
            [InlineData("Test", false)]
            public void ShouldReturnResultOfCheckFunction_WhenCalled(string name, bool expected)
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Check(i => i.Things.Prop2 == name);

                // Assert
                result.Should().Be(expected);
            }

            [Fact]
            public void ShouldNotAffectTheImmutable_WhenCheckFunctionTriesToAssign()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Check(i => { i.Name = "My Name"; return true; });

                // Assert
                immutable.Value("Name").As<string>().Should().BeNull();
            }
        }

        public class TheExtractMethod
        {
            [Fact]
            public void ShouldReturnValueOfPropertySelectByFunction_WhenCalled()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Extract(i => i.Things);

                // Assert
                result.Should().BeEquivalentTo(value.Things);
            }

            [Fact]
            public void ShouldReturnClonedValue_WhenCalled()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Extract(i => i.Things);
                result.Prop1 = 75;

                // Assert
                value.Things.Prop1.Should().NotBe(75);
            }

            [Fact]
            public void ShouldNotAffectTheImmutable_WhenExtractFunctionTriesToAssign()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                var result = immutable.Extract(i => { i.Name = "My Name"; return i.Things; });

                // Assert
                immutable.Value("Name").As<string>().Should().BeNull();
            }

            [Fact]
            public void ShouldThrowException_WhenExtractFunctionReturnsUnsupportedType()
            {
                // Arrange
                var value = new ComplexTestClass();
                var immutable = new Immutable<ComplexTestClass>(value);

                // Act
                Action result = () => immutable.Extract(i => i.LotsOfInts.Where(l => l > 100));

                // Assert
                result.Should().Throw<InvalidOperationException>().WithMessage("*- Unable to clone interfaces. Use concrete types and implementations instead.");
            }
        }

        public class TheEqualsMethod
        {
            private static SimpleTestClass SimpleReferenec = new SimpleTestClass();
            private static IImmutable SimpleImmutableReference = new Immutable<SimpleTestClass>(SimpleReferenec);

            public static IEnumerable<object[]> ImmutableOperands = new List<object[]>
            {
                new object[]
                {
                    SimpleImmutableReference,
                    SimpleImmutableReference,
                    true
                },
                new object[]
                {
                    SimpleImmutableReference,
                    new Immutable<SimpleTestClass>(new SimpleTestClass()),
                    false
                },
                new object[]
                {
                    new Immutable<SimpleTestClass>(new SimpleTestClass()),
                    SimpleImmutableReference,
                    false
                },
                new object[]
                {
                    new Immutable<SimpleTestClass>(new SimpleTestClass()),
                    new Immutable<SimpleTestClass>(new SimpleTestClass()),
                    false
                }
            };

            public static IEnumerable<object[]> ClassOperands = new List<object[]>
            {
                new object[]
                {
                    SimpleImmutableReference,
                    SimpleReferenec,
                    true
                },
                new object[]
                {
                    SimpleImmutableReference,
                    new SimpleTestClass(),
                    false
                }
            };

            [Theory]
            [MemberData(nameof(ImmutableOperands))]
            public void ShouldBeEqual_WhenImmutablesAreAsExpected(IImmutable a, IImmutable b, bool expected)
            {
                // Arrange / Act
                var result = a.Equals(b);

                // Assert
                result.Should().Be(expected);
            }

            [Theory]
            [MemberData(nameof(ClassOperands))]
            public void ShouldBeEqual_WhenImmutablesAndClassAreAsExpected(IImmutable<SimpleTestClass> a, SimpleTestClass b, bool expected)
            {
                // Arrange / Act
                var result = a.Equals(b);

                // Assert
                result.Should().Be(expected);
            }
        }
    }
}
