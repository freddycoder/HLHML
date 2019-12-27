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
            Scope[Childs.First().Value] = NodeVisitor.Eval(Childs.Skip(1));
        }
    }
}
