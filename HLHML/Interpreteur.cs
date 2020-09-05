using HLHML.LanguageElements;
using System;
using System.IO;

namespace HLHML
{
    public class Interpreteur
    {
        private readonly Scope _scope;

        public IReadOnlyScope Scope => _scope;

        private readonly TextWriter _textWriter;

        public Interpreteur(TextWriter textWriter)
        {
            _scope = new Scope();
            _textWriter = textWriter;
        }

        public void Interprete(string? input)
        {
            var parseur = new Parseur(new Lexer(input ?? ""));

            parseur.SetTextWriter(_textWriter);

            var ast = parseur.Parse(_scope);

            NodeVisitor.Visit(ast);
        }
    }
}