using System;
using System.IO;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Afficher : AST, IActionnable
    {
        private bool _newLine = false;
        private TextWriter _textWriter;

        public Afficher(Terme terme) : base(terme)
        {
            _textWriter = Console.Out;
        }

        public void SetTextWriter(TextWriter textWriter, bool newLineWhenAfficher = false)
        {
            _textWriter = textWriter;
            _newLine = newLineWhenAfficher;
        }

        public void Actionner()
        {
            foreach (var child in Childs)
            {
                if (child.Type == TokenType.Text || child.Type == TokenType.Nombre)
                {
                    _textWriter.Write(child.Value);
                }
                else if (child.Type == TokenType.Sujet)
                {
                    _textWriter.Write(Scope[child.Value] ?? "");
                }
                else if (child is OperateurMathematique op)
                {
                    _textWriter.Write(op.Eval());
                }
            }

            if (_newLine)
            {
                _textWriter.WriteLine();
            }
        }
    }
}
