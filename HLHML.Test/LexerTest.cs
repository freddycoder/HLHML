using HLHML.Exceptions;
using Shouldly;
using Xunit;
using static HLHML.TermeBuilder;

namespace HLHML.Test
{
    public class LexerTest
    {
        [Fact]
        public void WhatHappendOnNonClosingDoubleQuote()
        {
            var text = "Afficher \"";

            var lexer = new Lexer(text);

            lexer.ObtenirProchainTerme().ShouldBe(Terme("Afficher", TypeTerme.Verbe));

            var exception = Should.Throw<NonClosingQuoteException>(() => lexer.ObtenirProchainTerme());

            exception.BeginAt.ShouldBe(text.Length - 1);
            exception.Text.ShouldBe(text);
        }

        [Fact]
        public void Parenthèses()
        {
            var lexer = new Lexer("()");

            lexer.ObtenirProchainTerme().ShouldBe(Terme("(", TypeTerme.OuvertureParenthèse));
            lexer.ObtenirProchainTerme().ShouldBe(Terme(")", TypeTerme.FermetureParenthèse));
        }

        [Fact]
        public void SymboleComparateur()
        {
            var lexer = new Lexer("> < >= <= == !=");

            lexer.ObtenirProchainTerme().ShouldBe(Terme(">",  TypeTerme.PlusGrandQue));
            lexer.ObtenirProchainTerme().ShouldBe(Terme("<",  TypeTerme.PlusPetitQue));
            lexer.ObtenirProchainTerme().ShouldBe(Terme(">=", TypeTerme.PlusGrandOuEgalÀ));
            lexer.ObtenirProchainTerme().ShouldBe(Terme("<=", TypeTerme.PlusPetitOuEgalÀ));
            lexer.ObtenirProchainTerme().ShouldBe(Terme("==", TypeTerme.EgalÀ));
            lexer.ObtenirProchainTerme().ShouldBe(Terme("!=", TypeTerme.DifferentDe));
        }
    }
}
