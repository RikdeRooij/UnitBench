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

//[assembly: System.Diagnostics.Debuggable(isJITTrackingEnabled: false, isJITOptimizerDisabled: false)]

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
            Run<ShapesBenchmark>();
            Console.ReadKey();
        }

        private static void Run<T>()
        {
            BenchmarkRunner.Run<T>();
            //return BenchmarkRunner.Run<T>(DefaultConfig.Instance
            //    .AddJob(
            //        Job.Default
            //    //.WithToolchain(new InProcessEmitToolchain(logOutput: false, timeout: TimeSpan.FromSeconds(30)))
            //    //.WithIterationTime(Perfolizer.Horology.TimeInterval.FromMilliseconds(100))
            //    )
            //    .AddLogger(new ConsoleLogger(unicodeSupport: true))//, ConsoleLogger.CreateGrayScheme()))
            //    .WithOptions(ConfigOptions.JoinSummary));
        }
    }

}
