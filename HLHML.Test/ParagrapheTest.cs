using HLHML.AnalyseurLexical;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using static HLHML.TermeBuilder;

namespace HLHML.Test
{
    public class ParagrapheTest
    {
        [Theory]
        [InlineData("ma fonction se définit comme suit : \n    Afficher \"ma fonction\".\n\nma fonction.", "    ")]
        [InlineData("ma fonction se définit comme suit : \n  Afficher \"ma fonction\".\n\nma fonction.", "  ")]
        [InlineData("ma fonction se définit comme suit : \n\tAfficher \"ma fonction\".\n\nma fonction.", "\t")]
        public void LexerDetecteSautDeLigne(string text, string textIndentation)
        {
            var lexer = new Lexer(text).PrendreEnComptesEspacement();

            var termes = ListerLesTerme(lexer);

            termes.Count.ShouldBe(17);

            termes[0].ShouldBe(Terme("ma", TypeTerme.Déterminant));
            termes[1].ShouldBe(Terme("fonction", TypeTerme.Sujet));
            termes[2].ShouldBe(Terme("se", TypeTerme.Déterminant));
            termes[3].ShouldBe(Terme("définit", TypeTerme.Verbe));
            termes[4].ShouldBe(Terme("comme suit", TypeTerme.Adverbe));
            termes[5].ShouldBe(Terme(":", TypeTerme.Ponctuation));
            termes[6].ShouldBe(Terme("\\n", TypeTerme.SautDeLigne));
            termes[7].ShouldBe(Terme(textIndentation, TypeTerme.Indentation));
            termes[8].ShouldBe(Terme("Afficher", TypeTerme.Verbe));
            termes[9].ShouldBe(Terme("ma fonction", TypeTerme.Text));
            termes[10].ShouldBe(Terme(".", TypeTerme.Ponctuation));
            termes[11].ShouldBe(Terme("\\n", TypeTerme.SautDeLigne));
            termes[12].ShouldBe(Terme("\\n", TypeTerme.SautDeLigne));
            termes[13].ShouldBe(Terme("ma", TypeTerme.DifferentDe));
            termes[14].ShouldBe(Terme("fonction", TypeTerme.Sujet));
            termes[15].ShouldBe(Terme(".", TypeTerme.Ponctuation));
            termes[16].ShouldBe(Terme("", TypeTerme.None));
        }

        private List<Terme> ListerLesTerme(ILexer lexer)
        {
            var liste = new List<Terme>(20);

            Terme t;

            do
            {
                t = lexer.ObtenirProchainTerme();

                liste.Add(t);

            } while (t.Type != TypeTerme.None);

            return liste;
        }

        [Theory]
        [InlineData("ma fonction se définit comme suit : \n    Afficher \"ma fonction\".\n\nma fonction.")]
        [InlineData("ma fonction se définit comme suit : \n  Afficher \"ma fonction\".\n\nma fonction.")]
        [InlineData("ma fonction se définit comme suit : \n\tAfficher \"ma fonction\".\n\nma fonction.")]
        public void ParseurDetectePorteDefinitionAvecEspacement(string text)
        {
            var parseur = new Parseur(new Lexer(text).PrendreEnComptesEspacement());

            // TODO : Le lexer avec le decarateur doit avoir des essais pour la propriété DernierTerme

            var tree = parseur.Parse();

            tree.Childs.Count.ShouldBe(2);
        }
    }
}
