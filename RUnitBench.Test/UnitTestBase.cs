using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

[assembly: CLSCompliant(true)]
namespace NUnitBench.Test
{
    public class UnitTestBase : IDisposable
    {
        private MyDebugOutWriter myDebugWriter;
        private bool disposedValue;

        [SetUp]
        public void BaseSetUp() { myDebugWriter?.Dispose(); myDebugWriter = new MyDebugOutWriter(); }

        [TearDown]
        public void BaseTearDown() { myDebugWriter?.Dispose(); myDebugWriter = null; }

        public static void TestWriteLine(string value) { NUnit.Framework.TestContext.Progress.WriteLine(value); }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            if (disposing)
            {
                myDebugWriter?.Dispose(); myDebugWriter = null;
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /*public class UnitTestExample : UnitTestBase
    {
        [SetUp]
        public void Setup()
        {
            //var r = new Randomizer();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }*/


    public class MyDebugOutWriter : TextWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }

        private TextWriter oldConsoleOut;

        public MyDebugOutWriter()
        {
            this.oldConsoleOut = Console.Out;
            Console.SetOut(this);
        }

        protected override void Dispose(bool disposing)
        {
#if false
            if (tmp_sb.Length > 0)
                WriteLine("");
            tmp_sb.Clear();
#endif
            if (oldConsoleOut != null)
            {
                Console.SetOut(oldConsoleOut);
                oldConsoleOut = null;
            }
            Console.ResetColor();
            base.Dispose(disposing);
        }

        public override void Write(char value)
        {
            oldConsoleOut.Write(value);
            NUnit.Framework.TestContext.Progress.Write(value);
            //base.Write(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            oldConsoleOut.Write(buffer, index, count);
            NUnit.Framework.TestContext.Progress.Write(buffer, index, count);
            //base.Write(buffer, index, count);
        }

#if false
        private ConsoleColor consoleColor = Console.ForegroundColor;
        private readonly StringBuilder tmp_sb = new StringBuilder();
#endif

        public override void Write(string value)
        {
#if false
            //if (consoleColor != Console.ForegroundColor)
            consoleColor = Console.ForegroundColor;
            //tmp_sb.Append(ForegroundColorANSI(consoleColor));
            tmp_sb.Append(value);
            //tmp_sb.Append(ResetColorANSI());
#endif

            Debugger.Log(0, "Test", "[debug]" + value);
            Trace.Write("[trace]" + value);
            oldConsoleOut.Write(value);
            NUnit.Framework.TestContext.Progress.Write(value);
            //base.Write(value);
        }
        public override void WriteLine(string value)
        {
#if false
            if (tmp_sb.Length > 0)
            {
                m_output.WriteLine(tmp_sb.ToString());
                tmp_sb.Clear();
            }

            consoleColor = Console.ForegroundColor;
            //m_output.WriteLine(ForegroundColorANSI(consoleColor));
            m_output.WriteLine(value);
            //m_output.WriteLine(ResetColorANSI());
#endif

            Debugger.Log(0, "Test", "[debug]" + value);
            Trace.WriteLine("[trace]" + value);
            oldConsoleOut.WriteLine(value);
            NUnit.Framework.TestContext.Progress.Write(value);
            //base.WriteLine(value);
        }

        public override void Flush() { Trace.Flush(); oldConsoleOut.Flush(); NUnit.Framework.TestContext.Progress.Flush(); base.Flush(); }

        public static string ForegroundColorANSI(ConsoleColor c)
        {
            string colorString = "\x1b[";
            switch (c)
            {
                case ConsoleColor.Black: colorString += "30"; break;
                case ConsoleColor.DarkBlue: colorString += "34"; break;
                case ConsoleColor.DarkGreen: colorString += "32"; break;
                case ConsoleColor.DarkCyan: colorString += "36"; break;
                case ConsoleColor.DarkRed: colorString += "31"; break;
                case ConsoleColor.DarkMagenta: colorString += "35"; break;
                case ConsoleColor.DarkYellow: colorString += "33"; break;
                case ConsoleColor.Gray: colorString += "37"; break;
                case ConsoleColor.DarkGray: colorString += "30;1"; break;
                case ConsoleColor.Blue: colorString += "34;1"; break;
                case ConsoleColor.Green: colorString += "32;1"; break;
                case ConsoleColor.Cyan: colorString += "36;1"; break;
                case ConsoleColor.Red: colorString += "31;1"; break;
                case ConsoleColor.Magenta: colorString += "35;1"; break;
                case ConsoleColor.Yellow: colorString += "33;1"; break;
                case ConsoleColor.White: colorString += "37;1"; break;
                default: colorString = ""; break;
            }
            if (string.IsNullOrEmpty(colorString))
                return null;
            colorString += "m";
            return colorString;
        }

        public static string ResetColorANSI() { return ("\x1b[m"); }
    }


}
