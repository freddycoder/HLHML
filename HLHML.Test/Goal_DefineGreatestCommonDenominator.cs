using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.Test
{
    [TestClass]
    public class Goal_DefineGreatestCommonDenominator
    {
        [TestMethod]
        [Timeout(2000)]
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

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("3", sw.ToString());
            }
        }

        [TestMethod]
        public void TestLexerBigSubject()
        {
            var lexer = new Lexer("Le plus grand denominateur commun de deux nombres se définit comme suit :");

            Assert.AreEqual(Token("Le", TokenType.Determinant), lexer.GetNextToken());

            Assert.AreEqual(Token("plus grand denominateur commun", TokenType.Sujet), lexer.GetNextToken());

            Assert.AreEqual(Token("de", TokenType.Determinant), lexer.GetNextToken());

            Assert.AreEqual(Token("deux nombres", TokenType.Sujet), lexer.GetNextToken());

            Assert.AreEqual(Token("se", TokenType.Determinant), lexer.GetNextToken());

            Assert.AreEqual(Token("définit", TokenType.Verbe), lexer.GetNextToken());

            Assert.AreEqual(Token("comme suit", TokenType.Adverbe), lexer.GetNextToken());

            Assert.AreEqual(Token(":", TokenType.Ponctuation), lexer.GetNextToken());
        }

        [TestMethod]
        public void TestParseurWithDefinition()
        {
            var lexer = new Lexer("Le plus grand denominateur commun se définit comme suit :" +
                                  "le plus grand denominateur commun vaut 3." +
                                  "" +
                                  "Afficher le plus grand denominateur commun.");

            var parseur = new Parseur(lexer);

            var root = parseur.Parse();

            Assert.AreEqual(Token("Compound", TokenType.Compound), Token(root.Value, root.Type));

            Assert.AreEqual(2, root.Childs.Count);

            Assert.AreEqual(Token("définit", TokenType.Verbe), Token(root.Childs[0]));

            Assert.AreEqual(Token("Afficher", TokenType.Verbe), Token(root.Childs[1]));
        }

        private Token Token(AST ast)
        {
            return Token(ast.Value, ast.Type);
        }

        private Token Token(string value, TokenType type)
        {
            return new Token(value, type);
        }
    }
}
