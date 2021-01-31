using HLHML.AnalyseurLexical;
using HLHML.Dictionnaire;
using HLHML.LanguageElements;
using Shouldly;
using System.Linq;
using Xunit;
using static HLHML.Test.Outils.OutilsInterpreteur;

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

            Interprete(program, "3");
        }

        [Fact]
        public void TestLexer()
        {
            var lexer = new Lexer("tant que b n'est pas égal à 0");

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("tant que", TypeTerme.Conjonction));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("b", TypeTerme.Sujet));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("n'", TypeTerme.Negation));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("est", TypeTerme.Verbe));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("pas", TypeTerme.Negation));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("égal à", TypeTerme.Adjectif));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("0", TypeTerme.Nombre));
        }

        [Fact]
        public void TestLexer2()
        {
            var lexer = new Lexer("b = a modulo b.");

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("b", TypeTerme.Sujet));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("vaut", TypeTerme.Verbe));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("a", TypeTerme.Sujet));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("modulo", TypeTerme.OperateurMathematique));

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("b", TypeTerme.Sujet));
        }

        [Fact]
        public void TestModulo()
        {
            var program = "afficher 5 modulo 2";

            Interprete(program, "1");
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

            var corps = tantque.Childs.Last();

            corps.Value.ShouldBe("Corps");
            corps.Type.ShouldBe(TypeTerme.Corps);
        }

        [Fact]
        public void NoInfiniteLoop()
        {
            var program = "a vaut 1." +
                          "Tant que a n'est pas égal à 4," +
                          "a = a + 1." +
                          "Ensuite, afficher a";

            Interprete(program, "4");
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

            tantque.Type.ShouldBe(TypeTerme.Conjonction);
            tantque.Childs.Count.ShouldBe(2);

            var corps = tantque.Childs[1];

            corps.Type.ShouldBe(TypeTerme.Corps);
            corps.Childs.Count.ShouldBe(1);

            var operateurEgal = corps.Childs[0];

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

            Interprete(program, "1");
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
