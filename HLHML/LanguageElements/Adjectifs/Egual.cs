using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements.Adjectifs
{
    public class Egual : AST, Adjectif
    {
        public Egual(Token token) : base(token)
        {
        }

        public bool Valider()
        {
            var x = NodeVisitor.Eval(Childs[0]);
            var y = NodeVisitor.Eval(Childs[1]);

            return x == y;
        }
    }
}
