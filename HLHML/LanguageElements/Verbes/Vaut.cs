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

        public void Actionner()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException("An assignation must have only tow child node.");
            }

            Scope[Childs.First().Value] = NodeVisitor.Eval(Childs.Last());
        }
    }
}
