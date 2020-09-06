using Xunit;
using Shouldly;
using static HLHML.TokenBuilder;
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

            lexer.GetNextToken().ShouldBe(Token("Le", TokenType.Determinant));

            lexer.GetNextToken().ShouldBe(Token("plus grand denominateur commun", TokenType.Sujet));

            lexer.GetNextToken().ShouldBe(Token("de", TokenType.Determinant));

            lexer.GetNextToken().ShouldBe(Token("deux nombres", TokenType.Sujet));

            lexer.GetNextToken().ShouldBe(Token("se", TokenType.Determinant));

            lexer.GetNextToken().ShouldBe(Token("définit", TokenType.Verbe));

            lexer.GetNextToken().ShouldBe(Token("comme suit", TokenType.Adverbe));

            lexer.GetNextToken().ShouldBe(Token(":", TokenType.Ponctuation));
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

            Token(root.Value, root.Type).ShouldBe(Token("Compound", TokenType.Compound));

            root.Childs.Count.ShouldBe(2);

            root.Childs[0].Token.ShouldBe(Token("définit", TokenType.Verbe));

            root.Childs[1].Token.ShouldBe(Token("Afficher", TokenType.Verbe));
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
    }
}
