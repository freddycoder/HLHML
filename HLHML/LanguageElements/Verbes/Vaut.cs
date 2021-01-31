using System;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Vaut : AST, IActionnable
    {
        public Vaut(Terme terme) : base(terme)
        {
        }

        public Vaut(AST firstChild, Terme terme, AST secondChild) : base(firstChild, terme, secondChild)
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
            else if (Value.Equals("définit", StringComparison.InvariantCultureIgnoreCase))
            {
                Scope[Childs[0].Value] = this;
            }
            else
            {
                NodeVisitor.Visit(Childs[1]);

                Scope[Childs[0].Value] = Childs[1].Scope[Childs[0].Value];
            }
        }
    }
}
