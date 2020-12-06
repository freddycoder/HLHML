using HLHML.LanguageElements;
using Shouldly;
using Xunit;
using static HLHML.Test.Outils.OutilsInterpreteur;

namespace HLHML.Test
{
    public class AntiAnnomalie
    {
        [Fact]
        public void DivisionParZero()
        {
            Interprete("r = 10. a = 0, b = 0, r = a / b. afficher r.", "NaN");
        }

        [Fact]
        public void PeuCreerVariableNb1Nb2Etc()
        {
            Interprete("nb1 = 1, nb2 = 2, afficher nb1 + nb2.", "3");
        }

        [Fact]
        public void ModuloFonctionneBien()
        {
            Interprete("nombre1 = 5, nombre2 = 3, resultat = nombre1 % nombre2. afficher resultat.", "2");
        }

        [Fact]
        public void PeuCreerVariableNb1Nb2Etc2()
        {
            Interprete("x = 1, y = 2, afficher x + y.", "3");
        }

        [Fact]
        public void ModuloFonctionneBien2()
        {
            Interprete("x = 5, y = 3, z = x % y. afficher z.", "2");
        }

        [Fact]
        public void ArbreSi()
        {
            var programme = "Si prix etiquette branche est plus grand que 0 et prix etiquette grise est plus grand que 0";

            var parseur = new Parseur(new Lexer(programme));

            var tree = parseur.Parse();

            tree.Childs[0].Childs[0].ShouldBeOfType<Et>();
        }

        [Fact]
        public void Expression()
        {
            var programme = "prix etiquette branche est plus grand que 0 et prix etiquette grise est plus grand que 0";

            var parseur = new Parseur(new Lexer(programme));

            var tree = parseur.Expression();

            tree.ShouldBeOfType<Et>();
        }
    }
}
