using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Validators;
using Perfolizer.Horology;

[assembly: System.Diagnostics.Debuggable(isJITTrackingEnabled: false, isJITOptimizerDisabled: false)]

[assembly: CLSCompliant(true)]
namespace NUnitBench.Benchmark
{
    using NUnitBench;
    using NUnitBench.Utils;
    using NUnitBench.Benchmark.Source;
    using Bogus;

    public static class BenchmarkProgram
    {
        static void Main()
        {
            //StartRun<SimpleShapesBenchmark>();
            StartRun<ShapesBenchmark>();
            Console.ReadKey();
        }

        private static void StartRun<T>()
        {
            //BenchmarkRunner.Run<T>();
            BenchmarkRunner.Run<T>(DefaultConfig.Instance
                .AddJob(
                    new Job("MyJob", RunMode.Medium)
                    {
                        Environment = {
                                          Runtime = MonoRuntime.Default,
                                          Jit = Jit.Llvm,
                                          //Runtime = ClrRuntime.Net462,
                                          Platform = Platform.X64
                                      },
                        Run = {
                                  RunStrategy = RunStrategy.Throughput,
                                  /*IterationTime = TimeInterval.Millisecond * 300,
                                  IterationCount = 3,
                                  LaunchCount = 1,
                                  UnrollFactor = 1,
                                  WarmupCount = 1*/
                              }
                    }
                //.WithToolchain(CsProjClassicNetToolchain.Net462)
                //.WithToolchain(new InProcessEmitToolchain(logOutput: false, timeout: TimeSpan.FromSeconds(30)))
                )
                .AddLogger(new ConsoleLogger(unicodeSupport: true))//, ConsoleLogger.CreateGrayScheme()))
                .WithOptions(ConfigOptions.JoinSummary));
        }
    }

    public class ObjCall<T, T2>
    {
        public Func<T> Ctor { get; }
        public Func<T, T2> Call { get; }
        public Func<T2> ExecCall { get; set; }
        public T Inst { get; }

        public ObjCall(System.Func<T> objCtor, System.Func<T, T2> method, bool ctor)
        {
            Ctor = objCtor;
            Call = method;
            Inst = objCtor == null ? default : objCtor();
            ExecCall = ctor ? new Func<T2>(() => Call(Ctor())) : new Func<T2>(() => Call(Inst));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T2 Exec() { return Call(Inst); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T2 ExecCtor() { return Call(Ctor()); }
    }

    public class ObjData<TResult>
    {
        public TResult Instance { get; }
        public Func<TResult> Ctor { get; }
        public ObjData(System.Func<TResult> objCtor)
        { Ctor = objCtor; Instance = objCtor == null ? default : objCtor(); }
        public ObjData(TResult obj)
        { Ctor = () => obj; Instance = obj; }
    }

    public static class ObjDataFaker
    {
        public static ObjCall<T, float> CreateData<T, T2>(System.Func<T2, float, float, T> objCtor,
                                                          T2 arg2, float p1, float p2,
                                                          System.Func<T, float> method, bool ctor)
        { return new ObjCall<T, float>(() => objCtor(arg2, p1, p2), method, ctor); }

        public static ObjCall<T, float> CreateData<T>(System.Func<float, float, T> objCtor,
                                                      float p1, float p2,
                                                      System.Func<T, float> method, bool ctor)
        { return new ObjCall<T, float>(() => objCtor(p1, p2), method, ctor); }

        public static ObjDataFaker<T> CreateFaker<T>(System.Func<T> objCtor) // where T : ObjData<T>
            where T : class
        {
            //var faker = new Faker<ObjData<T>>();
            return new ObjDataFaker<T>()
                .CustomInstantiator2(f => new ObjData<T>(objCtor));
        }

        public static ObjDataFaker<T> CreateObjData<TFaker, T>(System.Func<T> objCtor)
            where T : class where TFaker : ObjDataFaker<T>, new()
        {
            return new TFaker().CustomInstantiator2(f => new ObjData<T>(objCtor));
        }

        // ----

        private static Faker<ObjData<T>> GenerateFaker<T>(System.Func<T> objCtor)
        {
            return new Faker<ObjData<T>>()
                .CustomInstantiator(f => new ObjData<T>(objCtor));
        }

        public static ObjData<T>[] GenerateCollection<T>(int count, System.Func<T> objCtor)
        { return GenerateFaker<T>(objCtor).Generate(count).ToArray(); }

        public static ObjData<T> Generate<T>(System.Func<T> objCtor)
        { return GenerateFaker(objCtor).Generate(); }

    }

#pragma warning disable CS3009 // Base type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant

    public class ObjDataFaker<TResult> : Faker<ObjData<TResult>>
    {
        public ObjDataFaker<TResult> CustomInstantiator2(Func<Faker, ObjData<TResult>> factoryMethod)
        {
            CreateActions[currentRuleSet] = factoryMethod;
            return this;
        }
        public ObjDataFaker<TResult> CustomInstantiator3<T>(Func<Faker, T> factoryMethod) where T : TResult
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            CreateActions[currentRuleSet] = f => new ObjData<TResult>(factoryMethod(f));
            return this;
        }
    }
    // public class Faker : ILocaleAware, IHasRandomizer, IHasContext
    // public class Faker<T> : IFakerTInternal, ILocaleAware, IRuleSet<T> where T : class
    public static class FakerExtensionMethods
    {
        public static T[] GenerateArray<T>(this Faker<T> faker, int count) where T : class
        {
            return faker?.Generate(count).ToArray();
        }
    }

#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3009 // Base type is not CLS-compliant

}
