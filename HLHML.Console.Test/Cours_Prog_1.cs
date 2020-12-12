using HLHML.LanguageElements;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace HLHML.Console.Test
{
    public class Cours_Prog_1
    {
        [Fact]
        public void AfficherCarre()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.Carre.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("3\n");

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(5);
            lines[0].ShouldBe("Entrer la taille du carré : ");
            lines[1].ShouldBe("***");
            lines[2].ShouldBe("***");
            lines[3].ShouldBe("***");
            lines[4].ShouldBe(string.Empty);
        }

        [Fact]
        public void AfficherCarre1()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.Carre.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("1\n");

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(3);
            lines[0].ShouldBe("Entrer la taille du carré : ");
            lines[1].ShouldBe("*");
            lines[2].ShouldBe(string.Empty);
        }

        [Fact]
        public void TestEvalIntStringMultiplication()
        {
            var parseur = new Parseur(new Lexer("3 * \"*\""));

            AST tree = parseur.Parse();

            string output = (tree.Childs.Single() as OperateurMathematique).Eval();

            output.ShouldBe("***");
        }

        [Fact]
        public void AfficherTriangle()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.Triangle.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("3\n");

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(5);
            lines[0].ShouldBe("Entrer la taille du triangle : ");
            lines[1].ShouldBe("*");
            lines[2].ShouldBe("**");
            lines[3].ShouldBe("***");
            lines[4].ShouldBe(string.Empty);
        }

        [Fact]
        public void AfficherTriangle1()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.Triangle.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("1\n");

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(3);
            lines[0].ShouldBe("Entrer la taille du triangle : ");
            lines[1].ShouldBe("*");
            lines[2].ShouldBe(string.Empty);
        }

        [Fact]
        public void AfficherRectangle()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.Rectangle.fr");

            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(4);
            lines[0].ShouldBe("*******");
            lines[1].ShouldBe("*     *");
            lines[2].ShouldBe("*******");
            lines[3].ShouldBe(string.Empty);
        }
    }
}
