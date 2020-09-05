using System;
using System.Collections.Generic;
using System.Text;

namespace HLHML.LanguageElements
{
    public class Liste : AST
    {
        public Liste(AST firstChild, Token token, AST secondChild) : base(firstChild, token, secondChild)
        {
        }
    }
}
