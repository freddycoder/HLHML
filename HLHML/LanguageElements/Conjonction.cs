using System;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Conjonction : AST, IActionnable
    {
        public bool PredicatIsNegated { get; set; }

        public Conjonction(Terme terme) : base(terme)
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
            else if (Value.Equals("pour", StringComparison.OrdinalIgnoreCase))
            {
                ConjonctionPour();
            }
            else
            {
                throw new InvalideConjonctionException($"Cannot interprete conjonction {this}");
            }
        }

        private void ConjonctionPour()
        {
            while (PredicatCompound())
            {
                NodeVisitor.Visit(Childs[1]);
            }
        }

        private bool PredicatCompound()
        {
            throw new NotImplementedException();
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
