using Shouldly;
using System;
using System.IO;
using Xunit;

namespace HLHML.Console.Test
{
    public class TP_Algo_TP1
    {
        [Fact]
        public void TP1_CalculateurDePrix_ScenarioNominal()
        {
            var path = Path.Combine("TP-Algo", "TP-01-calculateurDePrix.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader("5\n6\noui\n");

            System.Console.SetIn(sr);

            Program.MainProgram(new string[] { path }, sw, sr, newLineWhenAfficher: true);

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(6);
            lines[0].ShouldBe("Bienvenu dans le calculateur de prix!");
            lines[1].ShouldBe("Combient d'étiquette blanche ? ");
            lines[2].ShouldBe("Combient d'étiquette grise ? ");
            lines[3].ShouldBe("Est-ce que vous voulez la livraison ? ");
            lines[4].ShouldBe("18,425");
            lines[5].ShouldBe("Merci d'avoir utiliser le calculateur de prix !");
        }
    }
}
