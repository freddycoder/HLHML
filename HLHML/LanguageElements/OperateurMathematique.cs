using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class OperateurMathematique : AST
    {
        private string Operator { get; }

        public OperateurMathematique(Token token) : base(token)
        {
            Debug.Assert(token.Type == TokenType.OperateurMathematique);

            Operator = token.Value.ToLower();
        }

        public OperateurMathematique(Token token, AST firstChild) : base(token, firstChild)
        {
            Debug.Assert(token.Type == TokenType.OperateurMathematique && token.Value == "-");

            Operator = token.Value.ToLower();
        }

        public OperateurMathematique(AST firstChild, Token token, AST secondChild) : base(firstChild, token, secondChild)
        {
            Debug.Assert(token.Type == TokenType.OperateurMathematique);

            Operator = token.Value.ToLower();
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
