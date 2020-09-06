using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Lire : AST, IActionnable
    {
        public Lire(Token token) : base(token)
        {
        }

        public void Actionner()
        {
            Scope[_childs.First().Value] = Console.ReadLine();
        }
    }
}
