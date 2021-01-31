using System;
using System.Collections.Generic;
using System.Text;

namespace HLHML.LanguageElements
{
    public class Parametres : AST
    {
        public Parametres(Terme token, AST? firstChild) : base(token, firstChild)
        {
        }
    }
}
