using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HLHML.AnalyseurLexical;
using HLHML.Dictionnaire;
using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using Shouldly;
using Xunit;
using static HLHML.Test.Outils.OutilsInterpreteur;

namespace HLHML.Test.Goal
{
    public class Goal_InterpreteDemoProgram
    {
        [Fact]
        public void TestLexer()
        {
            var lexer = new Lexer("Afficher \"Bonjour le monde!\"");

            var t1 = lexer.ObtenirProchainTerme();

            t1.Type.ShouldBe(TypeTerme.Verbe);
            t1.Mots.ShouldBe("Afficher");

            var t2 = lexer.ObtenirProchainTerme();

            t2.Type.ShouldBe(TypeTerme.Text);
            t2.Mots.ShouldBe("Bonjour le monde!");

            var t3 = lexer.ObtenirProchainTerme();

            t3.Type.ShouldBe(TypeTerme.None);
            t3.Mots.ShouldBe("");
        }

        [Fact]
        public void TestParser()
        {
            var lexer = new Lexer("Afficher \"Bonjour le monde!\"");

            var parser = new Parseur(lexer);

            var ast = parser.Parse();

            ast.Type.ShouldBe(TypeTerme.Corps);
            ast.Value.ShouldBe("Corps");

            ast.Childs.Count.ShouldBe(1);

            ast = ast.Childs.First();

            ast.Type.ShouldBe(TypeTerme.Verbe);
            ast.Value.ShouldBe("Afficher");
            ast.ShouldBeOfType<Afficher>();

            ast.Childs.Count.ShouldBe(1);
            ast.Childs[0].Value.ShouldBe("Bonjour le monde!");
        }

        [Fact]
        public void HelloWorld()
        {
            var program = "Afficher \"Bonjour le monde!\"";

            Interprete(program, "Bonjour le monde!");
        }

        [Fact]
        public void TestEqualityIngoreCase()
        {
            var eq = StringComparer.OrdinalIgnoreCase;

            eq.Equals("Vaut", "vaut").ShouldBeTrue();
        }

        [Fact]
        public void TestDictionnaryEqualityComparer()
        {
            var dict = new Dictionary<string, TypeTerme>(StringComparer.OrdinalIgnoreCase);

            dict.Add("Vaut", TypeTerme.Verbe);

            dict.ContainsKey("vaut").ShouldBeTrue();
        }

        [Fact]
        public void TestLexerVariable1()
        {
            var lexer = new Lexer("a vaut 5. Afficher a.");

            var t1 = lexer.ObtenirProchainTerme();

            t1.Type.ShouldBe(TypeTerme.Sujet);
            t1.Mots.ShouldBe("a");

            var t2 = lexer.ObtenirProchainTerme();

            t2.Type.ShouldBe(TypeTerme.Verbe);
            t2.Mots.ShouldBe("vaut");

            var t3 = lexer.ObtenirProchainTerme();

            t3.Type.ShouldBe(TypeTerme.Nombre);
            t3.Mots.ShouldBe("5");

            var t4 = lexer.ObtenirProchainTerme();

            t4.Type.ShouldBe(TypeTerme.Ponctuation);
            t4.Mots.ShouldBe(".");

            var t5 = lexer.ObtenirProchainTerme();

            t5.Type.ShouldBe(TypeTerme.Verbe);
            t5.Mots.ShouldBe("Afficher");

            var t6 = lexer.ObtenirProchainTerme();

            t6.Type.ShouldBe(TypeTerme.Sujet);
            t6.Mots.ShouldBe("a");

            var t7 = lexer.ObtenirProchainTerme();

            t7.Type.ShouldBe(TypeTerme.Ponctuation);
            t7.Mots.ShouldBe(".");

            var t8 = lexer.ObtenirProchainTerme();

            t8.Type.ShouldBe(TypeTerme.None);
            t8.Mots.ShouldBe("");
        }

        [Fact]
        public void ASTandScope()
        {
            var root = new AST(new Terme("Corps", TypeTerme.Corps), new Scope());

            root.Scope.ShouldNotBeNull();

            root.AddChild(new AST(new Terme("a", TypeTerme.Sujet))
                .AddParent(new AST(new Terme("vaut", TypeTerme.Verbe))));

            root.Childs.First().AddChild(new AST(new Terme("5", TypeTerme.Nombre)));

            root.Childs.Count.ShouldBe(1);

            root.Childs.First().Childs.Count.ShouldBe(2);

            root.Childs.First().Childs.First().Value.ShouldBe("a");
            root.Childs.First().Childs[1].Value.ShouldBe("5");
        }

        [Fact]
        public void Variable1Parseur()
        {
            var parseur = new Parseur(new Lexer("a vaut 5. Afficher a."));

            var ast = parseur.Parse();

            ast.Type.ShouldBe(TypeTerme.Corps);

            ast.Childs.Count.ShouldBe(2);

            var vaut = ast.Childs.First();

            vaut.Type.ShouldBe(TypeTerme.Verbe);
            ("vaut".Equals(vaut.Value, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();

            vaut.Childs.First().Value.ShouldBe("a");
            vaut.Childs.Last().Value.ShouldBe("5");

            var afficher = ast.Childs.Last();

            afficher.Type.ShouldBe(TypeTerme.Verbe);
            "Afficher".Equals(afficher.Value).ShouldBeTrue();

            afficher.Childs.First().Value.ShouldBe("a");
            afficher.Childs.First().Type.ShouldBe(TypeTerme.Sujet);
        }

        [Fact]
        public void Variable1()
        {
            Interprete("a vaut 5. Afficher a.", "5");
        }

        [Fact]
        public void Variable2()
        {
            var program = "a vaut 5.\n" +
                          "b vaut 7.\n" +
                          "\n" + 
                          "Afficher a \" + \" b \" = ? \".";

            Interprete(program, "5 + 7 = ? ");
        }

        [Fact]
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
            {
                Console.SetIn(sr);

                Interprete(program, "5 + 7 = ? 12");
            }
        }

        [Fact]
        public void AdditionLexer()
        {
            var lexer = new Lexer("5 + 6");

            lexer.ObtenirProchainTerme().ShouldBe(new Terme("5", TypeTerme.Nombre));
            lexer.ObtenirProchainTerme().ShouldBe(new Terme("+", TypeTerme.OperateurMathematique));
            lexer.ObtenirProchainTerme().ShouldBe(new Terme("6", TypeTerme.Nombre));
        }

        [Fact]
        public void AdditionParser()
        {
            var parseur = new Parseur(new Lexer("11 + 111"));

            var ast = parseur.Parse();

            var plusOperator = ast.Childs.First();

            plusOperator.Type.ShouldBe(TypeTerme.OperateurMathematique);
            plusOperator.Value.ShouldBe("+");
            ((OperateurMathematique)plusOperator).Eval().ShouldBe("122");

            plusOperator.Childs.Count.ShouldBe(2);

            plusOperator.Childs[0].Value.ShouldBe("11");
            plusOperator.Childs[1].Value.ShouldBe("111");
        }

        [Fact]
        public void VerbeEtAdditionParseur()
        {
            var parseur = new Parseur(new Lexer("Afficher 2 + 1"));

            var root = parseur.Parse();

            root.Type.ShouldBe(TypeTerme.Corps);

            var affichier = root.Childs.First();

            affichier.Type.ShouldBe(TypeTerme.Verbe);

            var plus = affichier.Childs.First();

            plus.Type.ShouldBe(TypeTerme.OperateurMathematique);

            plus.Childs[0].Value.ShouldBe("2");
            plus.Childs[1].Value.ShouldBe("1");

            plus.ShouldBeOfType<OperateurMathematique>();

            var @operator = (OperateurMathematique)plus;

            @operator.Eval().ShouldBe("3");
        }

        [Fact]
        public void Addition()
        {
            Interprete("Afficher 5 + 6", "11");
        }

        [Fact]
        public void ConjonctionPremierJeton()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            conjonction.Type.ShouldBe(TypeTerme.Conjonction);
            conjonction.Value.ShouldBe("Si");
            conjonction.ShouldBeOfType<Conjonction>();
        }

        [Fact]
        public void ConjonctionPredicatParseur()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            conjonction.Childs.Count.ShouldBe(2);
            conjonction.Type.ShouldBe(TypeTerme.Conjonction);
            conjonction.Value.ShouldBe("Si");
            conjonction.ShouldBeOfType<Conjonction>();

            var predicat = conjonction.Childs.First();

            predicat.Type.ShouldBe(TypeTerme.EgalÀ);
            predicat.Value.ShouldBe("égal à");
            predicat.ShouldBeOfType<Egal>();

            var action = conjonction.Childs.Last();

            action.ShouldBeOfType<AST>();
            action.Type.ShouldBe(TypeTerme.Corps);
        }

        [Fact]
        public void ConjonctionPredicatAdjectfParseur()
        {
            var parseur = new Parseur(new Lexer("Si 5 est égal à 5, afficher \"Oui\""));

            var conjonction = parseur.Parse().Childs.Single();

            conjonction.Type.ShouldBe(TypeTerme.Conjonction);
            conjonction.Value.ShouldBe("Si");
            conjonction.ShouldBeOfType<Conjonction>();

            var égal = conjonction.Childs.First();

            égal.Type.ShouldBe(TypeTerme.EgalÀ);
            égal.Value.ShouldBe("égal à");
            égal.ShouldBeOfType<Egal>();

            égal.Childs.Count.ShouldBe(2);

            égal.Childs[0].Value.ShouldBe("5");
            égal.Childs[1].Value.ShouldBe("5");
        }

        [Fact]
        public void InterpretationAvecLaConsole()
        {
            using (var sr = new StringReader("a vaut 5\nafficher a"))
            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                Console.SetIn(sr);

                interpreteur.Interprete(sr.ReadLine());

                sw.Flush();

                interpreteur.Interprete(sr.ReadLine());

                sw.ToString().ShouldBe("5");
            }
        }

        [Fact]
        public void SiVraiAfficher()
        {
            var program = "Si 5 est égal à 5, afficher \"Oui\"";

            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("Oui");
            }
        }

        [Fact]
        public void InitialisationAvecDeterminant()
        {
            var program = "La réponse vaut 6.Afficher la réponse.";

            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("6");
            }
        }

        [Fact]
        public void PredicatPlusComplexe()
        {
            var program = "a vaut 2.\n" +
                          "b vaut 4.\n" +
                          "la réponse vaut 6.\n" +
                          "si la réponse est égal à a + b, afficher \"bonne réponse\"";

            var conjonction = new Parseur(new Lexer(program)).Parse().Childs.Last();

            conjonction.ShouldBeOfType<Conjonction>();

            conjonction.Childs.First().ShouldBeOfType<Egal>();

            using (var sw = new StringWriter())
            {
                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("bonne réponse");
            }
        }

        [Fact]
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
                Console.SetIn(sr);

                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("5 + 7 = ? Bonne réponse!");
            }
        }

        [Fact]
        public void Sinon()
        {
            var program = "Si 5 est égal à 6, afficher \"oui\" sinon afficher \"non\"";

            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete(program);

            sw.ToString().ShouldBe("non");
        }

        [Fact]
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
                Console.SetIn(sr);

                var interpreteur = new Interpreteur(sw);

                interpreteur.Interprete(program);

                sw.ToString().ShouldBe("5 + 7 = ? Mauvaise réponse");
            }
        }
    }
}
