using HLHML.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HLHML.Test
{
    public class LexerTest
    {
        [Fact]
        public void WhatHappendOnNonClosingDoubleQuote()
        {
            var text = "Afficher \"";

            var lexer = new Lexer(text);

            lexer.GetNextToken().ShouldBe(Token("Afficher", TokenType.Verbe));

            var exception = Should.Throw<NonClosingQuoteException>(() => lexer.GetNextToken());

            exception.BeginAt.ShouldBe(text.Length - 1);
            exception.Text.ShouldBe(text);
        }

        private Token Token(string v, TokenType tokenType) => new Token(v, tokenType);
    }
}
