using System;
using System.Collections.Generic;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.Parameters;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using NUnitBench.Utils;

//[assembly: System.Diagnostics.Debuggable(isJITTrackingEnabled: false, isJITOptimizerDisabled: false)]

namespace NUnitBench.Benchmark.Source
{
    using NUnitBench;
    using Bogus;

    //[MarkdownExporter]
    [ReturnValueValidator]
    [MemoryDiagnoser]
    public class ShapesBenchmark
    {
        private const int COUNT = 1;

        //[Params(0, 1, 2)]
        //public int N;

        private ShapeTestData[] shapeData;

        [GlobalSetup]
        public void Setup()
        {
            //var r = new Random();
            //var r = new Randomizer();

            //var faker_ShapeData = new Faker<ShapeTestData>()
            //    .CustomInstantiator(f =>
            //        new ShapeTestData(f.PickRandom<ShapeTypes>(), f.Random.Float(0.0f, 100.0f), f.Random.Float(0.0f, 100.0f)));
            //shapeData = faker_ShapeData.Generate(COUNT);
            //createShape = shapeData.ConvertAll(sd => sd.getShapeCtor()).ToArray();
            //createShapeProp = shapeData.ConvertAll(sd => sd.getShapePropCtor()).ToArray();
            //createShapePropClass = shapeData.ConvertAll(sd => sd.getShapePropClassCtor()).ToArray();
            //shapes = Array.ConvertAll(createShape, x => x());
            //shapeProps = Array.ConvertAll(createShapeProp, x => x());
            //shapePropsClass = Array.ConvertAll(createShapePropClass, x => x());

            shapeData = ShapeTestData.GenerateCollection(COUNT);
        }

        // ----------------

        [Benchmark]
        public double TestClass0()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += (shapeData[i].Shape.Ctor)().GetArea();
            }
            return r;
        }

        [Benchmark]
        public double TestProp0()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += ShapeProperties.GetArea((shapeData[i].ShapeProp.Ctor)());
            }
            return r;
        }

        [Benchmark]
        public double TestPropClass0()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += ShapePropertiesClass.GetArea((shapeData[i].ShapePropClass.Ctor)());
            }
            return r;
        }

        [Benchmark]
        public double TestPropClassM0()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += (shapeData[i].ShapePropClass.Ctor)().GetArea();
            }
            return r;
        }

        // ----

        [Benchmark]
        public double TestClass1()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += (shapeData[i].Shape.Instance).GetArea();
            }
            return r;
        }

        [Benchmark]
        public double TestProp1()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += ShapeProperties.GetArea(shapeData[i].ShapeProp.Instance);
            }
            return r;
        }

        [Benchmark]
        public double TestPropClass1()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += ShapePropertiesClass.GetArea(shapeData[i].ShapePropClass.Instance);
            }
            return r;
        }

        [Benchmark]
        public double TestPropClassM1()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += (shapeData[i].ShapePropClass.Instance).GetArea();
            }
            return r;
        }

        //[Benchmark]
        //public float Test0() { return (shapes[N]).area(); }

        //[Benchmark]
        //public float Test1() { return (shapeProps[N]).GetArea(); }

    }


    public class ShapeTestData
    {
        private enum TestShapeTypes { Rectangle, Triangle, Circle }

        private readonly TestShapeTypes type; private readonly float l; private readonly float h;

        private ShapeTestData(TestShapeTypes type, float l, float h = 0)
        {
            this.type = type; this.l = l; this.h = h;
            shape = new CtorInstance<Shape>(GetShapeCtor());
            shapeProp = new CtorInstance<ShapeProperties>(GetShapePropCtor());
            shapePropClass = new CtorInstance<ShapePropertiesClass>(GetShapePropClassCtor());
        }

        private Func<Shape> GetShapeCtor()
        {
            return this.type switch
            {
                TestShapeTypes.Rectangle => () => new Rectangle(l, h),
                TestShapeTypes.Triangle => () => new Triangle(l, h),
                TestShapeTypes.Circle => () => new Circle(l),
                _ => throw new InvalidOperationException(),
            };
        }

        private static ShapeTypes GetShapeType(TestShapeTypes testTypes) =>
            testTypes switch
            {
                TestShapeTypes.Rectangle => ShapeTypes.Rectangle,
                TestShapeTypes.Triangle => ShapeTypes.Triangle,
                TestShapeTypes.Circle => ShapeTypes.Circle,
                _ => throw new InvalidOperationException(),
            };

        private Func<ShapeProperties> GetShapePropCtor() => () => new ShapeProperties(GetShapeType(type), l, h);
        private Func<ShapePropertiesClass> GetShapePropClassCtor() => () => new ShapePropertiesClass(GetShapeType(type), l, h);

        // ----

        private readonly CtorInstance<Shape> shape;
        private readonly CtorInstance<ShapeProperties> shapeProp;
        private readonly CtorInstance<ShapePropertiesClass> shapePropClass;
        internal CtorInstance<Shape> Shape => shape;
        internal CtorInstance<ShapeProperties> ShapeProp => shapeProp;
        internal CtorInstance<ShapePropertiesClass> ShapePropClass => shapePropClass;

        public Shape ShapeInstance => shape.Instance;
        public ShapeProperties ShapePropInstance => ShapeProp.Instance;
        public ShapePropertiesClass ShapePropClassInstance => ShapePropClass.Instance;

        // ----

        private static Faker<ShapeTestData> GenerateFaker()
        {
            return new Faker<ShapeTestData>()
                .CustomInstantiator(f =>
                                        new ShapeTestData(f.PickRandom<TestShapeTypes>(),
                                                          f.Random.Float(0.0f, 100.0f),
                                                          f.Random.Float(0.0f, 100.0f))
                                    );
        }

        public static ShapeTestData[] GenerateCollection(int count)
        {
            return GenerateFaker().Generate(count).ToArray();
        }

        public static ShapeTestData Generate() { return GenerateFaker().Generate(); }
    }

}
