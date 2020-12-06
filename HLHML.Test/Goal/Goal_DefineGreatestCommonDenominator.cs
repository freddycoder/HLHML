using Xunit;
using Shouldly;
using static HLHML.TermeBuilder;
using static HLHML.Test.Outils.OutilsInterpreteur;

namespace HLHML.Test.Goal
{
    public class Goal_DefineGreatestCommonDenominator
    {
        [Fact]
        public void GreatestCommonDenominator()
        {
            var program = "Le plus grand diviseur commun de deux nombres se définit comme suit :" +
                          "a = premier nombre." +
                          "b = deuxième nombre." +
                          "Tant que b n'est pas égal à 0,\n" +
                          "    t = b.\n" +
                          "    b = a modulo b.\n" +
                          "    a = t.\n" +
                          "Ensuite, Le plus grand diviseur commun vaut a." +
                          "\n" +
                          "\n" +
                          "Afficher le plus grand diviseur commun de 15 et 21.";

            Interprete(program, "3");
        }

        [Fact]
        public void TestLexerBigSubject()
        {
            var lexer = new Lexer("Le plus grand denominateur commun de deux nombres se définit comme suit :");

            lexer.ObtenirProchainTerme().ShouldBe(Terme("Le", TypeTerme.Déterminant));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("plus grand denominateur commun", TypeTerme.Sujet));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("de", TypeTerme.Préposition));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("deux", TypeTerme.Adjectif));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("nombres", TypeTerme.Sujet));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("se", TypeTerme.Déterminant));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("définit", TypeTerme.Verbe));

            lexer.ObtenirProchainTerme().ShouldBe(Terme("comme suit", TypeTerme.Adverbe));

            lexer.ObtenirProchainTerme().ShouldBe(Terme(":", TypeTerme.Ponctuation));
        }

        [Fact]
        public void TestParseurWithDefinition()
        {
            var lexer = new Lexer("Le plus grand denominateur commun se définit comme suit :" +
                                  "le plus grand denominateur commun vaut 3." +
                                  "" +
                                  "Afficher le plus grand denominateur commun.");

            var parseur = new Parseur(lexer);

            var root = parseur.Parse();

            Terme(root.Value, root.Type).ShouldBe(Terme("Corps", TypeTerme.Corps));

            root.Childs.Count.ShouldBe(2);

            root.Childs[0].Terme.ShouldBe(Terme("définit", TypeTerme.Verbe));

            root.Childs[1].Terme.ShouldBe(Terme("Afficher", TypeTerme.Verbe));
        }

        [Fact]
        public void TestScopesWithDefinition()
        {
            var program = "Le plus grand denominateur commun se définit comme suit :" +
                          "le plus grand denominateur commun vaut 3." +
                          "" +
                          "Afficher le plus grand denominateur commun.";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            root.Scope.ShouldNotBeNull();
            root.Childs[0].Childs[0].Scope.ShouldNotBeNull();
            root.Childs[0].Childs[1].Scope.ShouldNotBeNull();
            root.Childs[0].Childs[1].Terme.ShouldBe(Terme("Corps", TypeTerme.Corps));

            (root.Childs[0].Childs[1].Scope != root.Scope).ShouldBeTrue();
            (root.Childs[0].Childs[0].Scope == root.Scope).ShouldBeTrue();
        }

        [Fact]
        public void TestInterpretationWithDefinition()
        {
            var program = "Le plus grand denominateur commun se définit comme suit :" +
                          "le plus grand denominateur commun vaut 3." +
                          "" +
                          "Afficher le plus grand denominateur commun.";

            Interprete(program, "3");
        }

        [Fact]
        public void TestInterpretationWithDefinitionAinsi()
        {
            var program = "Le plus grand denominateur commun se définit ainsi :" +
                          "le plus grand denominateur commun vaut 3." +
                          "" +
                          "Afficher le plus grand denominateur commun.";

            Interprete(program, "3");
        }

        [Fact]
        public void TestDefinitionFonctionMaximum()
        {
            var program = "Le maximum de trois nombres se définit comme suit: " +
                          "x = premier nombre." +
                          "si le deuxième nombre est plus grand que x, alors x = deuxième nombre." +
                          "si le troisième nombre est plus grand que x, alors x = troisième nombre." +
                          "Le maximum vaut x." +
                          "" +
                          "Afficher le maximum de 4, 5, 6.";

            Interprete(program, "6");
        }
    }
}
