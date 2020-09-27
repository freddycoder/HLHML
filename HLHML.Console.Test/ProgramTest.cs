using Shouldly;
using System.IO;
using Xunit;

namespace HLHML.Console.Test
{
    public class ProgramTest
    {
        [Fact]
        public void TestScriptUTF8BOM()
        {
            using var sr = new StringReader("12");
            using var sw = new StringWriter();

            System.Console.SetIn(sr);

            Program.MainProgram(new string[] { "DemoProgram.fr" }, sw, sr);

            sw.ToString().ShouldBe("5 + 7 = ? Bonne réponse!");
        }

        [Fact]
        public void TestScriptAINSI()
        {
            using var sr = new StringReader("12");
            using var sw = new StringWriter();

            System.Console.SetIn(sr);

            Program.MainProgram(new string[] { "AINSI_Encoding.fr" }, sw, sr);

            sw.ToString().ShouldBe("5 + 7 = ? Bonne réponse!");
        }

        [Fact]
        public void Euclide0InFile()
        {
            using var sw = new StringWriter();

            Program.MainProgram(new string[] { "Euclide.fr" }, sw, System.Console.In);

            sw.ToString().ShouldBe("3");
        }

        [Fact]
        public void Euclide0InFile2()
        {
            using var sw = new StringWriter();

            Program.MainProgram(new string[] { "Euclideutf-8.fr" }, sw, System.Console.In);

            sw.ToString().ShouldBe("3");
        }
    }
}
