using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Bogus;
using NUnit.Framework;
using NUnitBench.Benchmark.Source;

namespace NUnitBench.Test
{
    public class UnitTestShapesVariant
    {
        private readonly Shape data1;
        private readonly ShapeProperties data2;
        private readonly ShapePropertiesClass data3;

        public UnitTestShapesVariant(Shape data1, ShapeProperties data2, ShapePropertiesClass data3)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
        }

        public void AssertTest()
        {
            Assert.AreEqual(data1.GetArea(), ShapeProperties.GetArea(data2));
            Assert.AreEqual(data1.GetArea(), ShapePropertiesClass.GetArea(data3));
            Assert.AreEqual(ShapeProperties.GetArea(data2), data3.GetArea());
        }

        public string GetTestStr()
        {
            return (data1.GetArea() + " == " + ShapeProperties.GetArea(data2) + " == " + ShapePropertiesClass.GetArea(data3) + " == " + data3.GetArea());
        }
    }

    [TestFixture]
    public class UnitTestShapes : UnitTestBase
    {
        private const int COUNT = 1000;

        private UnitTestShapesVariant[] data1;
        private UnitTestShapesVariant[] data2;
        private UnitTestShapesVariant[] data3;

        private static void CreateVariants(float p1, float p2,
            out UnitTestShapesVariant data1v, out UnitTestShapesVariant data2v, out UnitTestShapesVariant data3v)
        {
            data1v = new UnitTestShapesVariant(Rectangle.Create(p1, p2),
                                               ShapeProperties.Create(ShapeTypes.Rectangle, p1, p2),
                                               ShapePropertiesClass.Create(ShapeTypes.Rectangle, p1, p2)
                                              );
            data2v = new UnitTestShapesVariant(Triangle.Create(p1, p2),
                                               ShapeProperties.Create(ShapeTypes.Triangle, p1, p2),
                                               ShapePropertiesClass.Create(ShapeTypes.Triangle, p1, p2)
                                              );
            data3v = new UnitTestShapesVariant(Circle.Create(p1, p2),
                                               ShapeProperties.Create(ShapeTypes.Circle, p1, p2),
                                               ShapePropertiesClass.Create(ShapeTypes.Circle, p1, p2)
                                              );
        }

        //[SetUp]
        [OneTimeSetUp]
        public void Setup()
        {
            //var r = new Randomizer();
            //shapeData = Benchmark.Source.ShapeTestData.GenerateCollection(COUNT);
            data1 = new UnitTestShapesVariant[COUNT];
            data2 = new UnitTestShapesVariant[COUNT];
            data3 = new UnitTestShapesVariant[COUNT];

            var r = new Randomizer();

            const float testMaxValue = float.MaxValue / 1.845e19f;

            for (int i = 0; i < COUNT; i++)
            {
                int j = i;
                float p1 = r.Float(0.0f, testMaxValue);
                float p2 = r.Float(0.0f, testMaxValue);
                CreateVariants(p1, p2, out data1[j], out data2[j], out data3[j]);
            }

            CreateVariants(0.0f, 0.0f, out data1[0], out data2[0], out data3[0]);
            CreateVariants(testMaxValue, testMaxValue, out data1[COUNT - 1], out data2[COUNT - 1], out data3[COUNT - 1]);

            TestWriteLine("rect1: " + data1[0].GetTestStr());
            TestWriteLine("tris1: " + data2[0].GetTestStr());
            TestWriteLine("crcl1: " + data3[0].GetTestStr());
            TestWriteLine("rect2: " + data1[COUNT - 1].GetTestStr());
            TestWriteLine("tris2: " + data2[COUNT - 1].GetTestStr());
            TestWriteLine("crcl2: " + data3[COUNT - 1].GetTestStr());

        }

        [Test]
        public void TestShapes1Rectangle()
        {
            for (int i = 0; i < COUNT; i++)
            {
                data1[i].AssertTest();
            }
        }

        [Test]
        public void TestShapes2Triangle()
        {
            for (int i = 0; i < COUNT; i++)
            {
                data2[i].AssertTest();
            }
        }

        [Test]
        public void TestShapes3Circle()
        {
            for (int i = 0; i < COUNT; i++)
            {
                data3[i].AssertTest();
            }
        }

    }

}
