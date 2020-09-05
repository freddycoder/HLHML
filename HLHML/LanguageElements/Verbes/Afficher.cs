using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Afficher : AST, Actionnable
    {
        private static readonly bool NewLines = Program.Settings.GetValue<bool>("newLineWhenAfficher");
        private TextWriter _textWriter;

        public Afficher(Token token) : base(token)
        {
            _textWriter = Console.Out;
        }

        public void SetTextWriter(TextWriter textWriter)
        {
            _textWriter = textWriter;
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

            if (NewLines)
            {
                _textWriter.WriteLine();
            }
        }
    }
}
