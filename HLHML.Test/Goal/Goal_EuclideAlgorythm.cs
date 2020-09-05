using HLHML.LanguageElements;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace HLHML.Test.Goal
{
    public class Goal_EuclideAlgorythm
    {
        [Fact]
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
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("3");
            }
        }

        [Fact]
        public void Euclide0InFile()
        {
            using var sw = new StringWriter();

            Console.SetOut(sw);

            Program.Main(new string[] { "Euclide.fr" });

            sw.ToString().ShouldBe("3");
        }

        [Fact]
        public void Euclide0InFile2()
        {
            using var sw = new StringWriter();

            Console.SetOut(sw);

            Program.Main(new string[] { "Euclideutf-8.fr" });

            sw.ToString().ShouldBe("3");
        }


        [Fact]
        public void TestLexer()
        {
            var lexer = new Lexer("tant que b n'est pas égal à 0");

            lexer.GetNextToken().ShouldBe(new Token("tant que", TokenType.Conjonction));

            lexer.GetNextToken().ShouldBe(new Token("b", TokenType.Sujet));

            lexer.GetNextToken().ShouldBe(new Token("n'", TokenType.Negation));

            lexer.GetNextToken().ShouldBe(new Token("est", TokenType.Verbe));

            lexer.GetNextToken().ShouldBe(new Token("pas", TokenType.Negation));

            lexer.GetNextToken().ShouldBe(new Token("égal à", TokenType.Adjectif));

            lexer.GetNextToken().ShouldBe(new Token("0", TokenType.Nombre));
        }

        [Fact]
        public void TestLexer2()
        {
            var lexer = new Lexer("b = a modulo b.");

            lexer.GetNextToken().ShouldBe(new Token("b", TokenType.Sujet));

            lexer.GetNextToken().ShouldBe(new Token("vaut", TokenType.Verbe));

            lexer.GetNextToken().ShouldBe(new Token("a", TokenType.Sujet));

            lexer.GetNextToken().ShouldBe(new Token("modulo", TokenType.OperateurMathematique));

            lexer.GetNextToken().ShouldBe(new Token("b", TokenType.Sujet));
        }

        [Fact]
        public void TestModulo()
        {
            var program = "afficher 5 modulo 2";

            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("1");
            }
        }

        [Fact]
        public void TestParser()
        {
            var program = "tant que 5 est égal à 5,\n" +
                          "    a = a + 10";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            root.Childs.Count.ShouldBe(1);

            var tantque = root.Childs.First();

            tantque.Childs.Count.ShouldBe(2);

            var compound = tantque.Childs.Last();

            compound.Value.ShouldBe("Compound");
            compound.Type.ShouldBe(TokenType.Compound);
        }

        [Fact]
        public void NoInfiniteLoop()
        {
            var program = "a vaut 1." +
                          "Tant que a n'est pas égal à 4," +
                          "a = a + 1." +
                          "Ensuite, afficher a";

            using (var sw = new StringWriter()) 
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("4");
            }
        }

        [Fact]
        public void NoInfiniteLoop2Parseur()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            root.Childs.Count.ShouldBe(3);
        }

        [Fact]
        public void NoInfiniteLoop2Parseur1()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            var parseur = new Parseur(new Lexer(program));

            var root = parseur.Parse();

            root.Childs.Count.ShouldBe(3);

            var tantque = root.Childs[1];

            tantque.Type.ShouldBe(TokenType.Conjonction);
            tantque.Childs.Count.ShouldBe(2);

            var compound = tantque.Childs[1];

            compound.Type.ShouldBe(TokenType.Compound);
            compound.Childs.Count.ShouldBe(1);

            var operateurEgal = compound.Childs[0];

            operateurEgal.Childs.Count.ShouldBe(2);
            operateurEgal.Childs[0].Value.ShouldBe("a");
            operateurEgal.Childs[1].Value.ShouldBe("1");
            operateurEgal.ShouldBeOfType<Vaut>();
        }

        [Fact]
        public void NoInfiniteLoop2()
        {
            var program = "a vaut 0." +
                          "Tant que a est égal à 0," +
                          "a = 1." +
                          "Ensuite, afficher a";

            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("1");
            }
        }        

        [Fact]
        public void ScopeTantQue()
        {
            var program = "stop vaut 0.\n" +
                          "Tant que stop n'est pas égal à 4,\n" +
                          "    stop = stop + 1.\n";

            var interpreteur = new Interpreteur(null);

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            scope.ContainsKey("stop").ShouldBeTrue();
            var stopValue = scope["stop"] as string;
            (stopValue == "4").ShouldBeTrue();
        }

        [Fact]
        public void ScopeTantQueModulo()
        {
            var program = "stop vaut 0.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = 15 % 2.\n";

            var interpreteur = new Interpreteur(null);

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            scope.ContainsKey("stop").ShouldBeTrue();
            var stopValue = scope["stop"] as string;
            (stopValue == "1").ShouldBeTrue();
        }

        [Fact]
        public void ScopeTantQueModulo2()
        {
            var program = "stop vaut 0.\n" +
                          "a vaut 15.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = a % b.\n";

            var interpreteur = new Interpreteur(null);

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            scope.ContainsKey("stop").ShouldBeTrue();
            var stopValue = scope["stop"] as string;
            (stopValue == "1").ShouldBeTrue();
        }

        [Fact]
        public void ScopeTantQueModulo3()
        {
            var program = "stop vaut 0.\n" +
                          "a vaut 15.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 1,\n" +
                          "    stop = a modulo b.\n";

            var interpreteur = new Interpreteur(null);

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            scope.ContainsKey("stop").ShouldBeTrue();
            var stopValue = scope["stop"] as string;
            (stopValue == "1").ShouldBeTrue();
        }

        [Fact]
        public void ScopeTantQueModulo4()
        {
            var program = "stop vaut 1.\n" +
                          "a vaut 3.\n" +
                          "b vaut 2.\n" +
                          "Tant que stop n'est pas égal à 0,\n" +
                          "    t = a modulo b." +
                          "    stop = stop modulo t.\n";

            var interpreteur = new Interpreteur(null);

            interpreteur.Interprete(program);

            var scope = interpreteur.Scope;

            scope.ContainsKey("stop").ShouldBeTrue();
            var stopValue = scope["stop"] as string;
            (stopValue == "0").ShouldBeTrue();

            scope.ContainsKey("t").ShouldBeFalse();
        }

        [Fact]
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
            a.ShouldBe(3);
        }
    }
}
