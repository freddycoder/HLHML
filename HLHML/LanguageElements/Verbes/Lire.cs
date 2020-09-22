using System;
using System.Linq;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Lire : AST, IActionnable
    {
        public Lire(Terme terme) : base(terme)
        {
        }

        public void Actionner()
        {
            Scope[_childs.First().Value] = Console.ReadLine();
        }
    }
}
