using HLHML.LanguageElements;
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
    public class Goal_EuclideAlgorythm
    {
        [TestMethod]
        [Timeout(2000)]
        public void Euclide0()
        {
            var program = "a vaut 15.\n" +
                          "b vaut 21.\n" +
                          "Tant que b n'est pas égal à 0,\n" +
                          "    t = b.\n" +
                          "    b = a modulo b.\n" +
                          "    a = t.\n" +
                          "Ensuite, afficher a.\n";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("3", sw.ToString());
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Euclide0InFile()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Program.Main(new string[] { "Euclide.fr" });

                Assert.AreEqual("3", sw.ToString());
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Euclide0InFile2()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Program.Main(new string[] { "Euclideutf-8.fr" });

                Assert.AreEqual("3", sw.ToString());
            }
        }


        [TestMethod]
        public void TestLexer()
        {
            var lexer = new Lexer("tant que b n'est pas égal à 0");

            Assert.AreEqual(new Token("tant que", TokenType.Conjonction), lexer.GetNextToken());

            Assert.AreEqual(new Token("b", TokenType.Sujet), lexer.GetNextToken());

            Assert.AreEqual(new Token("n'", TokenType.Negation), lexer.GetNextToken());

            Assert.AreEqual(new Token("est", TokenType.Verbe), lexer.GetNextToken());

            Assert.AreEqual(new Token("pas", TokenType.Negation), lexer.GetNextToken());

            Assert.AreEqual(new Token("égal à", TokenType.Adjectif), lexer.GetNextToken());

            Assert.AreEqual(new Token("0", TokenType.Nombre), lexer.GetNextToken());
        }

        [TestMethod]
        public void TestLexer2()
        {
            var lexer = new Lexer("b = a modulo b.");

            Assert.AreEqual(new Token("b", TokenType.Sujet), lexer.GetNextToken());

            Assert.AreEqual(new Token("vaut", TokenType.Verbe), lexer.GetNextToken());

            Assert.AreEqual(new Token("a", TokenType.Sujet), lexer.GetNextToken());

            Assert.AreEqual(new Token("modulo", TokenType.OperateurMathematique), lexer.GetNextToken());

            Assert.AreEqual(new Token("b", TokenType.Sujet), lexer.GetNextToken());
        }

        [TestMethod]
        public void TestModulo()
        {
            var program = "afficher 5 modulo 2";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("1", sw.ToString());
            }
        }

        [TestMethod]
        public void TestParser()
        {
            var program = "tant que 5 est égal à 5,\n" +
                          "    a = a + 10";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            Assert.AreEqual(1, root.Childs.Count);

            var tantque = root.Childs.First();

            Assert.AreEqual(2, tantque.Childs.Count);

            var compound = tantque.Childs.Last();

            Assert.AreEqual("Compound", compound.Value);
            Assert.AreEqual(TokenType.Compound, compound.Type);
        }

        [TestMethod]
        //[Timeout(2000)]
        public void NoInfiniteLoop()
        {
            var program = "a vaut 1." +
                          "Tant que a n'est pas égal à 4," +
                          "a = a + 1." +
                          "Ensuite, afficher a";

            using (var sw = new StringWriter()) 
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("4", sw.ToString());
            }
        }

        [TestMethod]
        public void NoInfiniteLoop2Parseur()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            Assert.AreEqual(3, root.Childs.Count);
        }

        [TestMethod]
        public void NoInfiniteLoop2Parseur1()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            Assert.AreEqual(3, root.Childs.Count);

            var tantque = root.Childs[1];

            Assert.AreEqual(TokenType.Conjonction, tantque.Type);
            Assert.AreEqual(2, tantque.Childs.Count);

            var compound = tantque.Childs[1];

            Assert.AreEqual(TokenType.Compound, compound.Type);
            Assert.AreEqual(1, compound.Childs.Count);

            var operateurEgal = compound.Childs[0];

            Assert.AreEqual(2, operateurEgal.Childs.Count);
            Assert.AreEqual("a", operateurEgal.Childs[0].Value);
            Assert.AreEqual("1", operateurEgal.Childs[1].Value);
            Assert.IsInstanceOfType(operateurEgal, typeof(Vaut));
        }

        [TestMethod]
        [Timeout(1000)]
        public void NoInfiniteLoop2()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var interpreteur = new Interpreteur();

                interpreteur.Interprete(program);

                Assert.AreEqual("1", sw.ToString());
            }
        }        

        [TestMethod]
        [Timeout(1000)]
        public void ScopeTantQue()
        {
            var program = "stop vaut 0.\n" +
                          "Tant que stop n'est pas égal à 4,\n" +
                          "    stop = stop + 1.\n";

            var interpreteur = new Interpreteur();

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            Assert.IsTrue(scope.ContainsKey("stop"));
            var stopValue = scope["stop"] as string;
            Assert.IsTrue(stopValue == "4");
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScopeTantQueModulo()
        {
            var program = "stop vaut 0.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = 15 % 2.\n";

            var interpreteur = new Interpreteur();

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            Assert.IsTrue(scope.ContainsKey("stop"));
            var stopValue = scope["stop"] as string;
            Assert.IsTrue(stopValue == "1");
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScopeTantQueModulo2()
        {
            var program = "stop vaut 0.\n" +
                          "a vaut 15.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = a % b.\n";

            var interpreteur = new Interpreteur();

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            Assert.IsTrue(scope.ContainsKey("stop"));
            var stopValue = scope["stop"] as string;
            Assert.IsTrue(stopValue == "1");
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScopeTantQueModulo3()
        {
            var program = "stop vaut 0.\n" +
                          "a vaut 15.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = a modulo b.\n";

            var interpreteur = new Interpreteur();

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            Assert.IsTrue(scope.ContainsKey("stop"));
            var stopValue = scope["stop"] as string;
            Assert.IsTrue(stopValue == "1");
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScopeTantQueModulo4()
        {
            var program = "stop vaut 1.\n" +
                          "a vaut 3.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 0,\n" +
                          "    t = a modulo b." +
                          "    stop = stop modulo t.\n";

            var interpreteur = new Interpreteur();

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            Assert.IsTrue(scope.ContainsKey("stop"));
            var stopValue = scope["stop"] as string;
            Assert.IsTrue(stopValue == "0");

            Assert.IsFalse(scope.ContainsKey("t"));
        }

        [TestMethod]
        [Timeout(1000)]
        public void EuclideCSharp()
        {
            var a = 15;
            var b = 21;
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }
            Assert.AreEqual(3, a);
        }
    }
}
