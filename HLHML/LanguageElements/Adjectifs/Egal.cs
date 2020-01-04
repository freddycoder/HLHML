using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements.Adjectifs
{
    public class Egal : AST, Adjectif
    {
        public Egal(Token token) : base(token)
        {
        }

        public bool Valider()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException($"The node 'Egal' must have exactly two node. It only has {Childs.Count} node.");
            }

            var x = NodeVisitor.Eval(Childs[0]);
            var y = NodeVisitor.Eval(Childs[1]);

            return x == y;
        }
    }
}
