using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HLHML.Test
{
    [TestClass]
    public class Goal_InterpreteDemoProgram
    {
        [TestMethod]
        public void TestLexer()
        {
            var lexer = new Lexer("Afficher \"Bonjour le monde!\"");

            var t1 = lexer.GetNextToken();

            Assert.AreEqual(t1.Type, TokenType.Verbe);
            Assert.AreEqual(t1.Value, "Afficher");

            var t2 = lexer.GetNextToken();

            Assert.AreEqual(t2.Type, TokenType.Text);
            Assert.AreEqual(t2.Value, "Bonjour le monde!");

            var t3 = lexer.GetNextToken();

            Assert.AreEqual(t3.Type, TokenType.None);
            Assert.AreEqual(t3.Value, "");
        }

        [TestMethod]
        public void TestParser()
        {
            var lexer = new Lexer("Afficher \"Bonjour le monde!\"");

            var parser = new Parseur(lexer);

            var ast = parser.Parse();

            Assert.AreEqual(ast.Type, TokenType.Compound);
            Assert.AreEqual(ast.Value, "Compound");

            Assert.AreEqual(ast.Childs.Count, 1);

            ast = ast.Childs.First();

            Assert.AreEqual(ast.Type, TokenType.Verbe);
            Assert.AreEqual(ast.Value, "Afficher");
            Assert.IsInstanceOfType(ast, typeof(Afficher));

            Assert.AreEqual(ast.Childs.Count, 1);
            Assert.AreEqual(ast.Childs[0].Value, "Bonjour le monde!");
        }

        [TestMethod]
        public void VerbeAfficher()
        {
            var afficher = new Afficher(new Token("Afficher", TokenType.Verbe));

            afficher.AddChilds(new AST(new Token("Bonjour le monde!", TokenType.Text)));

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                afficher.Actionner();

                Assert.AreEqual("Bonjour le monde!", sw.ToString());
            }
        }

        [TestMethod]
        public void HelloWorld()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete("Afficher \"Bonjour le monde!\"");

                Assert.AreEqual("Bonjour le monde!", sw.ToString());
            }
        }

        [TestMethod]
        public void TestEqualityIngoreCase()
        {
            var eq = StringComparer.OrdinalIgnoreCase;

            Assert.IsTrue(eq.Equals("Vaut", "vaut"));
        }

        [TestMethod]
        public void TestDictionnaryEqualityComparer()
        {
            var dict = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase);

            dict.Add("Vaut", TokenType.Verbe);

            Assert.IsTrue(dict.ContainsKey("vaut"));
        }

        [TestMethod]
        public void TestLexerVariable1()
        {
            var lexer = new Lexer("a vaut 5. Afficher a.");

            var t1 = lexer.GetNextToken();

            Assert.AreEqual(t1.Type, TokenType.Sujet);
            Assert.AreEqual(t1.Value, "a");

            var t2 = lexer.GetNextToken();

            Assert.AreEqual(t2.Type, TokenType.Verbe);
            Assert.AreEqual(t2.Value, "vaut");

            var t3 = lexer.GetNextToken();

            Assert.AreEqual(t3.Type, TokenType.Nombre);
            Assert.AreEqual(t3.Value, "5");

            var t4 = lexer.GetNextToken();

            Assert.AreEqual(t4.Type, TokenType.Ponctuation);
            Assert.AreEqual(t4.Value, ".");

            var t5 = lexer.GetNextToken();

            Assert.AreEqual(t5.Type, TokenType.Verbe);
            Assert.AreEqual(t5.Value, "Afficher");

            var t6 = lexer.GetNextToken();

            Assert.AreEqual(t6.Type, TokenType.Sujet);
            Assert.AreEqual(t6.Value, "a");

            var t7 = lexer.GetNextToken();

            Assert.AreEqual(t7.Type, TokenType.Ponctuation);
            Assert.AreEqual(t7.Value, ".");

            var t8 = lexer.GetNextToken();

            Assert.AreEqual(t8.Type, TokenType.None);
            Assert.AreEqual(t8.Value, "");
        }

        [TestMethod]
        public void ASTandScope()
        {
            var root = new AST(new Token("Compound", TokenType.Compound), new Scope());

            Assert.IsNotNull(root.Scope);

            root.AddChilds(new AST(new Token("a", TokenType.Sujet))
                .AddParent(new AST(new Token("vaut", TokenType.Verbe))));

            root.Childs.First().AddChilds(new AST(new Token("5", TokenType.Nombre)));

            Assert.AreEqual(1, root.Childs.Count);

            Assert.AreEqual(2, root.Childs.First().Childs.Count);

            Assert.AreEqual("a", root.Childs.First().Childs.First().Value);
            Assert.AreEqual("5", root.Childs.First().Childs[1].Value);
        }

        [TestMethod]
        public void Variable1Parseur()
        {
            var parseur = new Parseur(new Lexer("a vaut 5. Afficher a."));

            var ast = parseur.Parse();

            Assert.AreEqual(TokenType.Compound, ast.Type);

            Assert.AreEqual(2, ast.Childs.Count);

            var vaut = ast.Childs.First();

            Assert.AreEqual(TokenType.Verbe, vaut.Type);
            Assert.IsTrue("vaut".Equals(vaut.Value, StringComparison.OrdinalIgnoreCase));

            Assert.AreEqual("a", vaut.Childs.First().Value);
            Assert.AreEqual("5", vaut.Childs.Last().Value);

            var afficher = ast.Childs.Last();

            Assert.AreEqual(TokenType.Verbe, afficher.Type);
            Assert.IsTrue("Afficher".Equals(afficher.Value));

            Assert.AreEqual("a", afficher.Childs.First().Value);
            Assert.AreEqual(TokenType.Sujet, afficher.Childs.First().Type);
        }

        [TestMethod]
        public void Variable1()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete("a vaut 5. Afficher a.");

                Assert.AreEqual("5", sw.ToString());
            }
        }

        [TestMethod]
        public void Variable2()
        {
            var program = "a vaut 5.\n" +
                          "b vaut 7.\n" +
                          "\n" + 
                          "Afficher a \" + \" b \" = ? \".";

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("5 + 7 = ? ", sw.ToString());
            }
        }

        [TestMethod]
        public void Variable3()
        {
            var program = "a vaut 5.\n" +
                          "b vaut 7.\n" +
                          "\n" +
                          "Afficher a \" + \" b \" = ? \"." +
                          "\n" +
                          "Lire la reponse." +
                          "\n" +
                          "Afficher la reponse.";

            using (var sr = new StringReader("12"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("5 + 7 = ? 12", sw.ToString());
            }
        }

        [TestMethod]
        public void AdditionLexer()
        {
            var lexer = new Lexer("5 + 6");

            Assert.AreEqual(new Token("5", TokenType.Nombre), lexer.GetNextToken());
            Assert.AreEqual(new Token("+", TokenType.OperateurMathematique), lexer.GetNextToken());
            Assert.AreEqual(new Token("6", TokenType.Nombre), lexer.GetNextToken());
        }

        [TestMethod]
        public void AdditionParser()
        {
            var parseur = new Parseur(new Lexer("11 + 111"));

            var ast = parseur.Parse();

            var plusOperator = ast.Childs.First();

            Assert.AreEqual(TokenType.OperateurMathematique, plusOperator.Type);
            Assert.AreEqual("+", plusOperator.Value);
            Assert.AreEqual("122", (plusOperator as MathOperator).Eval());

            Assert.AreEqual(2, plusOperator.Childs.Count);

            Assert.AreEqual("11",  plusOperator.Childs[0].Value);
            Assert.AreEqual("111", plusOperator.Childs[1].Value);
        }

        [TestMethod]
        public void VerbeEtAdditionParseur()
        {
            var parseur = new Parseur(new Lexer("Afficher 2 + 1"));

            var root = parseur.Parse();

            Assert.AreEqual(TokenType.Compound, root.Type);

            var affichier = root.Childs.First();

            Assert.AreEqual(TokenType.Verbe, affichier.Type);

            var plus = affichier.Childs.First();

            Assert.AreEqual(TokenType.OperateurMathematique, plus.Type);

            Assert.AreEqual("2", plus.Childs[0].Value);
            Assert.AreEqual("1", plus.Childs[1].Value);

            Assert.IsInstanceOfType(plus, typeof(MathOperator));

            var @operator = plus as MathOperator;

            Assert.AreEqual("3", @operator.Eval());
        }

        [TestMethod]
        public void Addition()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete("Afficher 5 + 6");

                Assert.AreEqual("11", sw.ToString());
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ConjonctionPremierJeton()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            Assert.AreEqual(TokenType.Conjonction, conjonction.Type);
            Assert.AreEqual("Si", conjonction.Value);
            Assert.IsInstanceOfType(conjonction, typeof(Conjonction));
        }

        [TestMethod]
        [Timeout(2000)]
        public void ConjonctionPredicatParseur()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            Assert.AreEqual(2, conjonction.Childs.Count);
            Assert.AreEqual(TokenType.Conjonction, conjonction.Type);
            Assert.AreEqual("Si", conjonction.Value);
            Assert.IsInstanceOfType(conjonction, typeof(Conjonction));

            var predicat = conjonction.Childs.First();

            Assert.AreEqual(TokenType.Adjectif, predicat.Type);
            Assert.AreEqual("égal à", predicat.Value);
            Assert.IsInstanceOfType(predicat, typeof(Egal));

            var action = conjonction.Childs.Last();

            Assert.AreEqual("afficher", action.Value);
            Assert.AreEqual(TokenType.Verbe, action.Type);
        }

        [TestMethod]
        [Timeout(2000)]
        public void ConjonctionPredicatAdjectfParseur()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            Assert.AreEqual(TokenType.Conjonction, conjonction.Type);
            Assert.AreEqual("Si", conjonction.Value);
            Assert.IsInstanceOfType(conjonction, typeof(Conjonction));

            var égal = conjonction.Childs.First();

            Assert.AreEqual(TokenType.Adjectif, égal.Type);
            Assert.AreEqual("égal à", égal.Value);
            Assert.IsInstanceOfType(égal, typeof(Egal));

            Assert.AreEqual(2, égal.Childs.Count);

            Assert.AreEqual("5", égal.Childs[0].Value);
            Assert.AreEqual("5", égal.Childs[1].Value);
        }

        [TestMethod]
        public void InterpretationAvecLaConsole()
        {
            var interpreteur = new Interpreteur();

            using (var sr = new StringReader("a vaut 5\nafficher a"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                interpreteur.Interprete(sr.ReadLine());

                sw.Flush();

                interpreteur.Interprete(sr.ReadLine());

                Assert.AreEqual("5", sw.ToString());
            }
        }

        [TestMethod]
        public void SiVraiAfficher()
        {
            var program = "Si 5 est égal à 5, afficher \"Oui\"";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("Oui", sw.ToString());
            }
        }

        [TestMethod]
        public void InitialisationAvecDeterminant()
        {
            var program = "La réponse vaut 6.Afficher la réponse.";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("6", sw.ToString());
            }
        }

        [TestMethod]
        public void PredicatPlusComplexe()
        {
            var program = "a vaut 2.\n" +
                          "b vaut 4.\n" +
                          "la réponse vaut 6.\n" +
                          "si la réponse est égal à a + b, afficher \"bonne réponse\"";

            var conjonction = new Parseur(new Lexer(program)).Parse().Childs.Last();

            Assert.IsInstanceOfType(conjonction, typeof(Conjonction));

            Assert.IsInstanceOfType(conjonction.Childs.First(), typeof(Egal));

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("bonne réponse", sw.ToString());
            }
        }

        [TestMethod]
        public void GoalBranchTrue()
        {
            var program = "a vaut 5.\n" +
                          "b vaut 7.\n" +
                          "\n" +
                          "Afficher a \" + \" b \" = ? \"." +
                          "\n" +
                          "Lire la reponse." +
                          "\n" +
                          "Si la reponse est égal à a + b, afficher \"Bonne réponse!\" sinon afficher \"Mauvaise réponse\".";

            using (var sr = new StringReader("12"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("5 + 7 = ? Bonne réponse!", sw.ToString());
            }
        }

        [TestMethod]
        public void TestScript()
        {
            using (var sr = new StringReader("12"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(File.ReadAllText("DemoProgram.fr"));

                Assert.AreEqual("5 + 7 = ? Bonne réponse!", sw.ToString());
            }
        }

        [TestMethod]
        public void TestScriptUTF8BOM()
        {
            using (var sr = new StringReader("12"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                Program.Main(new string[] { "DemoProgram.fr" });

                Assert.AreEqual("5 + 7 = ? Bonne réponse!", sw.ToString());
            }
        }

        [TestMethod]
        public void TestScriptAINSI()
        {
            using (var sr = new StringReader("12"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                Program.Main(new string[] { "AINSI_Encoding.fr" });

                Assert.AreEqual("5 + 7 = ? Bonne réponse!", sw.ToString());
            }
        }

        [TestMethod]
        public void Sinon()
        {
            var program = "Si 5 est égal à 6, afficher \"oui\" sinon afficher \"non\"";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("non", sw.ToString());
            }
        }

        [TestMethod]
        public void GoalBranchFalse()
        {
            var program = "a vaut 5.\n" +
                          "b vaut 7.\n" +
                          "\n" +
                          "Afficher a \" + \" b \" = ? \"." +
                          "\n" +
                          "Lire la reponse." +
                          "\n" +
                          "Si la reponse est égal à a + b, afficher \"Bonne réponse!\" sinon afficher \"Mauvaise réponse\"."; ;

            using (var sr = new StringReader("13"))
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.SetIn(sr);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("5 + 7 = ? Mauvaise réponse", sw.ToString());
            }
        }
    }
}
