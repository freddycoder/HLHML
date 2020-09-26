using System;
using System.Diagnostics;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class OperateurMathematique : AST
    {
        private string Operator { get; }

        public OperateurMathematique(Terme terme) : base(terme)
        {
            Debug.Assert(terme.Type == TypeTerme.OperateurMathematique);

            Operator = terme.Mots.ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="firstChild"></param>
        /// <exception cref="ArgumentNullException">Si first child est null</exception>
        public OperateurMathematique(Terme token, AST? firstChild) : base(token, firstChild)
        {
            Debug.Assert(token.Type == TypeTerme.OperateurMathematique && token.Mots == "-");

            Operator = token.Mots.ToLower();
        }

        public OperateurMathematique(AST firstChild, Terme token, AST secondChild) : base(firstChild, token, secondChild)
        {
            Debug.Assert(token.Type == TypeTerme.OperateurMathematique);

            Operator = token.Mots.ToLower();
        }

        public string Eval()
        {
            return Operator switch
            {
                "+" => EvalPlus(),
                "modulo" => EvalModulo(),
                "*" => EvalMultiplication(),
                "-" => EvalSoustraction(),
                "/" => EvalDivision(),

                _ => throw new NotImplementedException($"L'operateur {Operator} n'est pas implémenté"),
            };
        }

        private string EvalDivision()
        {
            if (Childs.Count == 2)
            {
                var x = NodeVisitor.EvalDouble(Childs[0]);
                var y = NodeVisitor.EvalDouble(Childs[1]);

                return (x / y).ToString();
            }
            else if (Childs.Count > 2)
            {
                throw new InvalidNodeNumberException("Modulo operator must have only 2 child");
            }

            return "";
        }

        private string EvalSoustraction()
        {
            if (Childs.Count == 1)
            {
                return (- double.Parse(Childs[0].Value)).ToString();
            }
            if (Childs.Count == 2)
            {
                var x = double.Parse(NodeVisitor.Eval(Childs[0]));
                var y = double.Parse(NodeVisitor.Eval(Childs[1]));

                return (x - y).ToString();
            }
            else if (Childs.Count > 2)
            {
                throw new InvalidNodeNumberException("Modulo operator must have only 2 child");
            }

            return "";
        }

        private string EvalMultiplication()
        {
            if (Childs.Count == 2)
            {
                var x = double.Parse(NodeVisitor.Eval(Childs[0]));
                var y = double.Parse(NodeVisitor.Eval(Childs[1]));

                return (x * y).ToString();
            }
            else if (Childs.Count > 2)
            {
                throw new InvalidNodeNumberException("Modulo operator must have only 2 child");
            }

            return "";
        }

        private string EvalPlus()
        {
            if (Childs.Count == 1)
            {
                return Childs[0].Value;
            }
            else if (Childs.Count == 2)
            {
                var x = double.Parse(NodeVisitor.Eval(Childs[0]));
                var y = double.Parse(NodeVisitor.Eval(Childs[1]));

                return (x + y).ToString();
            }
            else if (Childs.Count > 2)
            {
                throw new InvalidNodeNumberException();
            }

            return "";
        }

        private string EvalModulo()
        {
            if (Childs.Count == 2)
            {
                var x = double.Parse(NodeVisitor.Eval(Childs[0]));
                var y = double.Parse(NodeVisitor.Eval(Childs[1]));

                return (x % y).ToString();
            }
            else if (Childs.Count > 2)
            {
                throw new InvalidNodeNumberException("Modulo operator must have only 2 child");
            }

            return "";
        }
    }
}
