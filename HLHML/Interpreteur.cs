using System;

namespace HLHML
{
    public class Interpreteur
    {
        private readonly Scope _scope;

        public Interpreteur()
        {
            _scope = new Scope();
        }

        public void Interprete(string input)
        {
            var parseur = new Parseur(new Lexer(input));

            var ast = parseur.Parse(_scope);

            NodeVisitor.Visit(ast);
        }
    }
}