using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class MathOperator : AST
    {
        public string Operator { get; }

        public MathOperator(Token token) : base(token)
        {
            Debug.Assert(token.Type == TokenType.OperateurMathematique);

            Operator = token.Value.ToLower();
        }

        public override string Value 
        {
            get
            {
                switch(Operator)
                {
                    case "+":
                        return EvalPlus();
                    case "modulo":
                        return EvalModulo();
                }

                throw new NotImplementedException($"L'operateur {Operator} n'est pas implémenté");
            }
        }

        private string EvalPlus()
        {
            if (Childs.Count == 1)
            {
                return Childs[0].Value;
            }
            else if (Childs.Count == 2)
            {
                var x = int.Parse(NodeVisitor.Eval(Childs[0]));
                var y = int.Parse(NodeVisitor.Eval(Childs[1]));

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
                var x = int.Parse(NodeVisitor.Eval(Childs[0]));
                var y = int.Parse(NodeVisitor.Eval(Childs[1]));

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
