using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Vaut : AST, Actionnable
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

            Scope[Childs[0].Value] = NodeVisitor.Eval(Childs[1]);
        }
    }
}
