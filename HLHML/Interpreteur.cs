using HLHML.LanguageElements;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace HLHML
{
    public class Interpreteur
    {
        private readonly Scope _scope;

        public IReadOnlyScope Scope => _scope;

        private readonly TextWriter _textWriter;
        private readonly bool _newLine;

        public Interpreteur(TextWriter? textWriter, bool newLineWhenAfficher = false)
        {
            _scope = new Scope();
            _textWriter = textWriter ?? Console.Out;
            _newLine = newLineWhenAfficher;

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
        }

        /// <summary>
        /// Interprete la chaine passé en paramètre. Si la méthode est appelé à répétition,
        /// les variables créer lors des executions précédentes seront toujours là.
        /// </summary>
        /// <param name="input">Le script à executer</param>
        public void Interprete(string? input)
        {
            var parseur = new Parseur(new Lexer(input ?? ""));

            parseur.SetTextWriter(_textWriter, _newLine);

            var ast = parseur.Parse(_scope);

            NodeVisitor.Visit(ast);
        }
    }
}