using HLHML.AnalyseurLexical;
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

        private readonly TextReader _textReader;

        public Interpreteur(TextWriter? textWriter, bool newLineWhenAfficher = false)
        {
            _scope = new Scope();
            _textWriter = textWriter ?? Console.Out;
            _newLine = newLineWhenAfficher;
            _textReader = Console.In;

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
        }

        public Interpreteur(TextWriter? textWriter, TextReader? textReader, bool newLineWhenAfficher = false) : this(textWriter, newLineWhenAfficher)
        {
            _textReader = textReader ?? Console.In;
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
            parseur.SetTextReader(_textReader);

            var ast = parseur.Parse(_scope);

            NodeVisitor.Visit(ast);
        }
    }
}