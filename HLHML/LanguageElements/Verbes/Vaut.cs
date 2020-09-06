using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Vaut : AST, IActionnable
    {
        public Vaut(Token token) : base(token)
        {
        }

        public Vaut(AST firstChild, Token token, AST secondChild) : base(firstChild, token, secondChild)
        {
        }

        public void Actionner()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException("An assignation must have only tow childs nodes.");
            }

            if (Value.Equals("vaut", StringComparison.OrdinalIgnoreCase))
            {
                Scope[Childs[0].Value] = NodeVisitor.Eval(Childs[1]);
            }
            else
            {
                NodeVisitor.Visit(Childs[1]);

                Scope[Childs[0].Value] = Childs[1].Scope[Childs[0].Value];
            }
        }
    }
}
