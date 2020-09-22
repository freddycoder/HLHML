using HLHML.Exceptions;
using Shouldly;
using Xunit;
using static HLHML.Dictionnaire.TermeBuilder;

namespace HLHML.Test
{
    public class LexerTest
    {
        [Fact]
        public void WhatHappendOnNonClosingDoubleQuote()
        {
            var text = "Afficher \"";

            var lexer = new Lexer(text);

            lexer.ObtenirProchainTerme().ShouldBe(Terme("Afficher", TokenType.Verbe));

            var exception = Should.Throw<NonClosingQuoteException>(() => lexer.ObtenirProchainTerme());

            exception.BeginAt.ShouldBe(text.Length - 1);
            exception.Text.ShouldBe(text);
        }
    }
}
