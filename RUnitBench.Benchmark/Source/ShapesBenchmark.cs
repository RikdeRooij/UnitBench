using System;
using System.Collections.Generic;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.InProcess;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.Parameters;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using NUnitBench.Utils;

//[assembly: System.Diagnostics.Debuggable(isJITTrackingEnabled: false, isJITOptimizerDisabled: false)]

namespace NUnitBench.Benchmark.Source
{
    using NUnitBench;
    using Bogus;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;

    //[MarkdownExporter]
    //[Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [ReturnValueValidator]
    [MemoryDiagnoser]
    //[InliningDiagnoser(true, false)]
    //[TailCallDiagnoser]
    public class ShapesBenchmark
    {
        private const int COUNT = 1000;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
#pragma warning disable CA1051 // Do not declare visible instance fields
#if PER_TYPE
        [Params(0, 1, 2)]
        public int N;
#endif
        [Params(0, 1)]
        public int C;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant

        private ObjCall<ShapeProperties, float>[] data1;
        private ObjCall<ShapePropertiesClass, float>[] data2;
        private ObjCall<Shape, float>[] data3;

        private float[] param1;
        private float[] param2;

        private void CreateData<T, T2>(int j, ObjCall<T, float>[] dest_data,
                                       System.Func<T2, float, float, T> objCtor, T2 arg2,
                                       System.Func<T, float> method, bool ctor)
        { dest_data[j] = ObjDataFaker.CreateData<T, T2>(objCtor, arg2, param1[j], param2[j], method, ctor); }

        private void CreateData<T>(int j, ObjCall<T, float>[] dest_data,
                                   System.Func<float, float, T> objCtor,
                                   System.Func<T, float> method, bool ctor)
        { dest_data[j] = ObjDataFaker.CreateData(objCtor, param1[j], param2[j], method, ctor); }

        [GlobalSetup]
        public void Setup()
        {
            var r = new Randomizer();
            //var faker = new Faker();

            const float testMaxValue = float.MaxValue / 1.845e19f;

            param1 = Array.ConvertAll(new float[COUNT], x => r.Float(0.0f, testMaxValue));
            param2 = Array.ConvertAll(new float[COUNT], x => r.Float(0.0f, testMaxValue));
            param1[0] = 0.0f; param2[0] = 0.0f;
            param1[COUNT - 1] = testMaxValue; param2[COUNT - 1] = testMaxValue;

            data1 = new ObjCall<ShapeProperties, float>[COUNT];
            data2 = new ObjCall<ShapePropertiesClass, float>[COUNT];
            data3 = new ObjCall<Shape, float>[COUNT];

            bool ctor = C == 1;
            for (int i = 0; i < COUNT; i++)
            {
                int j = i;
#if !PER_TYPE
                var N = r.Int(0, 2);
#endif
                switch (N)
                {
                    case 0:
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Rectangle, ShapeProperties.GetArea, ctor);
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Rectangle, x => x.GetArea(ShapeTypes.Rectangle), ctor);
                        CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Rectangle, x => x.GetArea(), ctor);
                        CreateData(j, data2, ShapePropertiesClass.Create, ShapeTypes.Rectangle, x => x.GetArea(), ctor);
                        CreateData(j, data3, Rectangle.Create, x => x.GetArea(), ctor);
                        break;
                    case 1:
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Triangle, ShapeProperties.GetArea, ctor);
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Triangle, x => x.GetArea(ShapeTypes.Triangle), ctor);
                        CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Triangle, x => x.GetArea(), ctor);
                        CreateData(j, data2, ShapePropertiesClass.Create, ShapeTypes.Triangle, x => x.GetArea(), ctor);
                        CreateData(j, data3, Triangle.Create, x => x.GetArea(), ctor);
                        break;
                    case 2:
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Circle, ShapeProperties.GetArea, ctor);
                        //CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Circle, x => x.GetArea(ShapeTypes.Circle), ctor);
                        CreateData(j, data1, ShapeProperties.Create, ShapeTypes.Circle, x => x.GetArea(), ctor);
                        CreateData(j, data2, ShapePropertiesClass.Create, ShapeTypes.Circle, x => x.GetArea(), ctor);
                        CreateData(j, data3, Circle.Create, x => x.GetArea(), ctor);
                        break;
                }
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            param1 = null;
            param2 = null;
            data1 = null;
            data2 = null;
            data3 = null;
        }

        // ----------------

        [Benchmark]
        public double TestProp()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += data1[i].ExecCall();
            }
            return r;
        }

        [Benchmark]
        public double TestPropClass()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += data2[i].ExecCall();
            }
            return r;
        }

        [Benchmark]
        public double TestClass()
        {
            double r = 0;
            for (int i = 0; i < COUNT; i++)
            {
                r += data3[i].ExecCall();
            }
            return r;
        }
    }

    //[MarkdownExporter]
    //[Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [ReturnValueValidator]
    [MemoryDiagnoser]
    //[InliningDiagnoser(true, false)]
    //[TailCallDiagnoser]
    public class SimpleShapesBenchmark
    {
        private int N;
        private ShapeTypes t1;
        private float param1;
        private float param2;
        private ShapeProperties data1;
        private ShapePropertiesClass data2;
        private Shape data3;

        [GlobalSetup]
        public void Setup()
        {
            var r = new Randomizer();
            //var faker = new Faker();

            const float testMaxValue = float.MaxValue / 1.85e19f;

            param1 = r.Float(0.0f, testMaxValue);
            param2 = r.Float(0.0f, testMaxValue);

            N = r.Int(0, 2);
            switch (N)
            {
                case 0:
                    t1 = ShapeTypes.Rectangle;
                   data1 = ShapeProperties.Create(ShapeTypes.Rectangle, param1, param2);
                    data2 = ShapePropertiesClass.Create(ShapeTypes.Rectangle, param1, param2);
                    data3 = Rectangle.Create(param1, param2);
                    break;
                case 1:
                    t1 = ShapeTypes.Triangle;
                    data1 = ShapeProperties.Create(ShapeTypes.Triangle, param1, param2);
                    data2 = ShapePropertiesClass.Create(ShapeTypes.Triangle, param1, param2);
                    data3 = Triangle.Create(param1, param2);
                    break;
                case 2:
                    t1 = ShapeTypes.Circle;
                    data1 = ShapeProperties.Create(ShapeTypes.Circle, param1, param2);
                    data2 = ShapePropertiesClass.Create(ShapeTypes.Circle, param1, param2);
                    data3 = Circle.Create(param1, param2);
                    break;
            }
        }

        // ----------------

        [Benchmark]
        public float TestProp()
        {
            return data1.GetArea();
        }

        [Benchmark]
        public float TestPropClass()
        {
            return data2.GetArea();
        }

        [Benchmark]
        public float TestClass()
        {
            return data3.GetArea();
        }

        [Benchmark]
        public float TestCtorProp()
        {
            return ShapeProperties.Create(t1, param1, param2).GetArea();
        }

        [Benchmark]
        public float TestCtorPropClass()
        {
            return ShapePropertiesClass.Create(t1, param1, param2).GetArea();
        }

        [Benchmark]
        public float TestCtorClass()
        {
            return t1 switch
            {
                ShapeTypes.Rectangle => Rectangle.Create(param1, param2).GetArea(),
                ShapeTypes.Triangle => Triangle.Create(param1, param2).GetArea(),
                ShapeTypes.Circle => Circle.Create(param1, param2).GetArea(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }

}
