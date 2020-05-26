using System;
using System.Collections.Generic;
using System.Linq;

namespace Ignitor.Tests.TestClasses
{
    public interface ISimpleTestClass
    {
        int Prop1 { get; set; }
    }

    public class SimpleTestClass : ISimpleTestClass
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public Guid Prop3 { get; set; }
        public object Prop4 { get; set; } = null;
    }

    public class ComplexTestClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SimpleTestClass Things { get; set; } = new SimpleTestClass
        {
            Prop1 = 100,
            Prop2 = "Blah"
        };
        public int[] LotsOfInts { get; set; } = Enumerable.Range(0, 10000).ToArray();
        public SimpleTestClass[] SomeThings { get; set; } = new SimpleTestClass[3]
        {
                new SimpleTestClass
                {
                    Prop1 = 1
                },
                new SimpleTestClass
                {
                    Prop1 = 2
                },
                new SimpleTestClass
                {
                    Prop1 = 3
                },
        };
    }

    public enum TestEnum
    {
        First,
        Second,
        Third
    }

    public class RecursiveTestClass
    {
        public Guid Id { get; set; }
        public RecursiveTestClass Recurse { get; set; }
    }

    public class ContainsListTestClass
    {
        public int Id { get; set; }
        public List<SimpleTestClass> MyThings { get; set; } = new List<SimpleTestClass>
            {
                new SimpleTestClass
                {
                    Prop1 = 1
                }
            };
    }

    public class ContainsDictionaryTestClass
    {
        public int Id { get; set; }
        public Dictionary<Guid, SimpleTestClass> MyThings { get; set; } = new Dictionary<Guid, SimpleTestClass>
        (
            new KeyValuePair<Guid, SimpleTestClass>[] {
                    new KeyValuePair<Guid, SimpleTestClass>(Guid.NewGuid(), new SimpleTestClass
                    {
                        Prop1 = 1
                    })
            }
        );
    }

    public class ContainsInterfaceTestClass
    {
        public int Id { get; set; }
        public ISimpleTestClass MyThing { get; set; } = new SimpleTestClass
        {
            Prop1 = 1
        };
    }
}
