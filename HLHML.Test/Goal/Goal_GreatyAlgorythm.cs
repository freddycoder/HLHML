using HLHML.LanguageElements;
using Microsoft.CodeAnalysis;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Xunit;

namespace HLHML.Test.Goal
{
    public class Goal_GreatyAlgorythm
    {
        [Fact]
        public void BaseScript()
        {
            var s = "Entrée :" +
                    "liste ordonnée V des n sommets d'un graphe G." +
                    "liste ordonnée C de couleurs." +
                    "Pour i variant de 1 à n," +
                    "    v = V[i]." +
                    "    couleur = la première couleur de C non utilisée par les voisins de v." +
                    "    colorier(v, couleur)." +
                    "Ensuite." +
                    "";

            var tree = new Parseur(new Lexer(s)).Parse();
        }

        [Fact]
        public void BouclePour()
        {
            var s = "liste ordonnée V = [1,2,3]. Pour i variant de 1 à n, v = V[i]. Afficher v. Ensuite.";

            AST tree = new Parseur(new Lexer(s)).Parse();

            tree.Childs.Count.ShouldBe(2);
        }

        [Fact]
        public void BouclePour2()
        {
            var s = "Pour i variant de 1 à 5, Afficher i. Ensuite.";

            AST tree = new Parseur(new Lexer(s)).Parse();

            tree.Childs.Count.ShouldBe(1);
        }

        [Fact]
        public void VariantTree()
        {
            var s = "i variant de 1 à 5";

            AST tree = new Parseur(new Lexer(s)).Parse();

            tree.Childs.Count.ShouldBe(1);
            
            tree = tree.Childs[0];

            tree.Value.ShouldBe("variant");
            tree.Scope.ContainsKey("i").ShouldBeTrue();
        }

        [Fact]
        public void De1A5ListeTree()
        {
            var s = "de 1 à 5";

            AST tree = new Parseur(new Lexer(s)).Parse();

            tree.Value.ShouldBeOfType<Liste>();
        }

        [Fact]
        public void AfficherDe1A5()
        {
            using var sr = new StringWriter();

            var s = "Affcher de 1 à 5";

            var interpreteur = new Interpreteur(sr);

            interpreteur.Interprete(s);

            sr.ToString().ShouldBe("1 2 3 4 5");
        }

        [Fact]
        public void AfficherDe1A5_2()
        {
            using var sr = new StringWriter();

            var s = "Affcher i variant de 1 à 5";

            var interpreteur = new Interpreteur(sr);

            interpreteur.Interprete(s);

            sr.ToString().ShouldBe("1 2 3 4 5");
        }

        [Fact]
        public void AfficherDe1A5_2Tree()
        {
            var s = "Affcher i variant de 1 à 5";

            var ast = new Parseur(new Lexer(s)).Parse();

            ast.Childs.Count.ShouldBe(1);
            ast.Childs[0].ShouldBeOfType<Afficher>();

            ast = ast.Childs[0];
        }
    }
}
