using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using NUnit.Framework;

[assembly: CLSCompliant(true)]
namespace NUnitBench.Test
{
    public class Tests
    {
        private const int COUNT = 1000;

        private Benchmark.Source.ShapeTestData[] shapeData;

        [SetUp]
        public void Setup()
        {
            //var r = new Randomizer();
            shapeData = Benchmark.Source.ShapeTestData.GenerateCollection(COUNT);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void TestShapes1()
        {
            for (int i = 0; i < COUNT; i++)
            {
                Assert.AreEqual(shapeData[(i)].ShapeInstance.GetArea(),
                                ShapeProperties.GetArea(shapeData[(i)].ShapePropInstance));
            }
        }

        [Test]
        public void TestShapes2()
        {
            for (int i = 0; i < COUNT; i++)
            {
                Assert.AreEqual(shapeData[(i)].ShapeInstance.GetArea(),
                                ShapePropertiesClass.GetArea(shapeData[(i)].ShapePropClassInstance));
            }
        }

        [Test]
        public void TestShapes3()
        {
            for (int i = 0; i < COUNT; i++)
            {
                Assert.AreEqual(shapeData[(i)].ShapeInstance.GetArea(),
                                shapeData[(i)].ShapePropClassInstance.GetArea());
            }
        }

    }
}
