using System;
using System.IO;
using System.Linq;
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
                if (child.Type == TypeTerme.Text || child.Type == TypeTerme.Nombre)
                {
                    _textWriter.Write(child.Value);
                }
                else if (child.Type == TypeTerme.Sujet)
                {
                    var valeurSujet = Scope[child.Value];

                    if (valeurSujet is AST noeudDefinition)
                    {
                        var corps = noeudDefinition.Childs.Last();

                        NodeVisitor.Visit(corps);

                        var toPrint = corps.Scope[child.Value];

                        _textWriter.Write(toPrint);
                    }
                    else
                    {
                        _textWriter.Write(valeurSujet ?? "");
                    }
                }
                else if (child is OperateurMathematique op)
                {
                    _textWriter.Write(op.Eval());
                }
                else if (child.Type == TypeTerme.Conjonction || child.Type == TypeTerme.Negation)
                {
                    var result = NodeVisitor.Eval(child);

                    if (result == true)
                    {
                        _textWriter.Write("vrai");
                    }
                    else if (result == false)
                    {
                        _textWriter.Write("faux");
                    }
                }
            }

            if (_newLine)
            {
                _textWriter.WriteLine();
            }
        }
    }
}
