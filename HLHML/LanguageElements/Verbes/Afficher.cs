using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Afficher : AST, Actionnable
    {
        public Afficher(Token token) : base(token)
        {
        }

        public void Actionner()
        {
            foreach (var child in Childs)
            {
                if (child.Type == TokenType.Text)
                {
                    Console.Write(child.Value);
                }
                else if (child.Type == TokenType.Sujet)
                {
                    Console.Write(Scope[child.Value] ?? "");
                }
                else if (child is MathOperator op)
                {
                    Console.Write(op.Eval());
                }
            }
        }
    }
}
