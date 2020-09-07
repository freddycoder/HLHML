using Shouldly;
using System.IO;
using Xunit;

namespace HLHML.Test
{
    public class InteractiveTest
    {
        [Fact]
        public void ExempleAlgorithme()
        {
            using var sw = new StringWriter();
            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete("a = 1");
            interpreteur.Interprete("b = 2");
            interpreteur.Interprete("c = 3");
            interpreteur.Interprete("x = a");
            interpreteur.Interprete("si b est plus grand que x, alors x = b");
            interpreteur.Interprete("si c est plus grand que x, alors x = c");

            (interpreteur.Scope["a"] as string).ShouldBe("1");
            (interpreteur.Scope["b"] as string).ShouldBe("2");
            (interpreteur.Scope["c"] as string).ShouldBe("3");
            (interpreteur.Scope["x"] as string).ShouldBe("3");

            interpreteur.Interprete("Afficher x");

            sw.ToString().ShouldBe("3");
        }
    }
}
