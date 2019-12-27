using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Conjonction : AST, Actionnable
    {
        public Conjonction(Token token) : base(token)
        {
        }

        public void Actionner()
        {
            if (EvalPredicat())
            {
                NodeVisitor.VisitNode(Childs[1]);
            }
            else if (Childs.Count == 3)
            {
                NodeVisitor.VisitNode(Childs[2]);
            }
        }

        private bool EvalPredicat()
        {
            if (Childs[0] is Adjectif adj)
            {
                return adj.Valider();
            }

            throw new InvalidPredicatException($"{Childs[0]} is not a valid predicat.");
        }
    }
}
