using Shouldly;
using System;
using System.IO;
using Xunit;

namespace HLHML.Console.Test
{
    public class Cours_Prog_1
    {
        [Fact]
        public void AfficherCarre()
        {
            var path = Path.Combine("Cours_Prog_1", "ExerciceProg1.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("3\n");

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(3);
            lines[0].ShouldBe("***");
            lines[1].ShouldBe("***");
            lines[2].ShouldBe("***");
        }
    }
}
