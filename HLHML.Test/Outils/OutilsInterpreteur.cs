using Shouldly;
using System.IO;

namespace HLHML.Test.Outils
{
    public static class OutilsInterpreteur
    {
        public static void Interprete(string program, string expectedOuput)
        {
            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete(program);

            sw.ToString().ShouldBe(expectedOuput);
        }
    }
}
