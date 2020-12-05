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

        [Theory]
        [InlineData("1\n1\nnon\n", "2,75")]
        [InlineData("5\n6\noui\n", "18,425")]
        public void TP1_Interpreteur(string input, string total)
        {
            var path = Path.Combine("TP-Algo", "TP-01-calculateurDePrix.fr");

            using var sw = new StringWriter();
            using var sr = new StringReader(input);

            var interpreteur = new Interpreteur(sw, sr, newLineWhenAfficher: true);

            interpreteur.Interprete(File.ReadAllText(path));

            var lines = sw.ToString().Split(Environment.NewLine);

            lines.Length.ShouldBe(6);
            lines[0].ShouldBe("Bienvenu dans le calculateur de prix!");
            lines[1].ShouldBe("Combient d'étiquette blanche ? ");
            lines[2].ShouldBe("Combient d'étiquette grise ? ");
            lines[3].ShouldBe("Est-ce que vous voulez la livraison ? ");
            lines[4].ShouldBe(total);
            lines[5].ShouldBe("Merci d'avoir utiliser le calculateur de prix !");
        }

        [Theory]
        [InlineData("prix de livraison = 5.afficher prix de livraison", "5")]
        [InlineData("le prix de livraison vaut 5.afficher prix de livraison", "5")]
        [InlineData("prix de livraison vaut 5.afficher le prix de livraison", "5")]
        public void PrixDeLivraison(string programme, string resultatAttendue)
        {
            using var sw = new StringWriter();

            var interpreteur = new Interpreteur(sw);

            interpreteur.Interprete(programme);

            sw.ToString().ShouldBe(resultatAttendue);
        }
    }
}
