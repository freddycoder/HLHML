using HLHML.AnalyseurLexical;
using HLHML.LanguageElements.Syntaxe;
using Shouldly;
using System.IO;
using Xunit;

namespace HLHML.Test.Goal
{
    public class Goal_EvaluationBoolean
    {
        private const string _initialisationVariables = "X vaut 10, Y vaut 5, Z vaut 1";

        [Fact]
        public void ConfigurationInitial()
        {
            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete(_initialisationVariables);

            (interpreteur.Scope["X"] as string).ShouldBe("10");
            (interpreteur.Scope["Y"] as string).ShouldBe("5");
            (interpreteur.Scope["Z"] as string).ShouldBe("1");
        }

        [Theory]
        [InlineData("(X > Y) ET (Z > 1)", "faux")]
        [InlineData("(X / 2 >= Y) ET (Y == 5)", "vrai")]
        [InlineData("NON(X > 10) OU (Z > 1)", "vrai")]
        [InlineData("(3 * Z * Y == X + Y) ET (X * 5 == Y * 10)", "vrai")]
        [InlineData("NON( (10 * X - Y == 95) ET (X - Y == Z * 5) )", "faux")]
        [InlineData("(X % 3 == Y % 2) ET (Z % 2 == 2)", "faux")]
        [InlineData("(X / 3 > 3) OU (Y / 3 > 1)", "vrai")]
        public void EvaluationBoolean(string expression, string resultat)
        {
            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete(_initialisationVariables);

            interpreteur.Interprete($"afficher {expression}");

            sw.ToString().ShouldBe(resultat);
        }

        [Fact]
        public void NoeudParenthèse()
        {
            var parseur = new Parseur(new Lexer("(5)"));

            var tree = parseur.Parse();

            tree.Childs.Count.ShouldBe(1);
            tree.Childs[0].ShouldBeOfType<Parenthese>();

            tree.Childs[0].Childs.Count.ShouldBe(1);
            tree.Childs[0].Childs[0].Type.ShouldBe(TypeTerme.Nombre);
        }

        [Fact]
        public void EvalNoeudParenthèse()
        {
            var parseur = new Parseur(new Lexer("(5)"));

            var tree = parseur.Parse();

            (NodeVisitor.Eval(tree.Childs[0]) as string).ShouldBe("5");
        }
    }
}
