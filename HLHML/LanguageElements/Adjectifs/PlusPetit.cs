﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements.Adjectifs
{
    public class PlusPetit : AST, Adjectif
    {
        public PlusPetit(Token token) : base(token)
        {
        }

        public bool Valider()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException($"L'adjectif plus 'plus petit que' doit avoir deux noeuds enfants. Celui-ci en à {Childs.Count}.");
            }

            return NodeVisitor.EvalDouble(Childs[0]) < NodeVisitor.EvalDouble(Childs[1]);
        }
    }
}
