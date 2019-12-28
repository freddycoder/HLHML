using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML.LanguageElements
{
    public class Conjonction : AST, Actionnable
    {
        public bool PredicatIsNegated { get; set; }

        public Conjonction(Token token) : base(token)
        {
            
        }

        public void Actionner()
        {
            if (Value.Equals("Si", StringComparison.OrdinalIgnoreCase))
            {
                ConjonctionSi();
            }
            else if (Value.Equals("Tant que", StringComparison.OrdinalIgnoreCase))
            {
                ConjonctionTantQue();
            }
            else
            {
                throw new InvalideConjonctionException($"Cannot interprete conjonction {this}");
            }
        }

        private void ConjonctionTantQue()
        {
            while (EvalPredicat())
            {
                NodeVisitor.Visit(Childs[1]);
            }
        }

        private void ConjonctionSi()
        {
            if (EvalPredicat())
            {
                NodeVisitor.VisitNode(Childs[1]);
            }
            else if (Childs.Count == 3) // Sinon
            {
                NodeVisitor.VisitNode(Childs[2]);
            }
        }

        private bool EvalPredicat()
        {
            if (Childs[0] is Adjectif adj)
            {
                return adj.Valider() != PredicatIsNegated;
            }

            throw new InvalidPredicatException($"{Childs[0]} is not a valid predicat.");
        }
    }
}
