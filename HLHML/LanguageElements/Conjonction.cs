using System;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Conjonction : AST, IActionnable
    {
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
            while (PredicatCorps())
            {
                NodeVisitor.Visit(Childs[1]);
            }
        }

        private bool PredicatCorps()
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
            var resultatPredicat = EvalPredicat();

            if (resultatPredicat) // Childs[0]
            {
                NodeVisitor.Visit(Childs[1]);
            }
            else if (resultatPredicat == false && Childs.Count == 3) // Sinon
            {
                NodeVisitor.Visit(Childs[2]);
            }
        }

        private bool EvalPredicat()
        {
            if (Childs[0] is Adjectif adj)
            {
                return adj.Valider();
            }

            throw new InvalidPredicatException($"{Childs[0]} n'est pas un prédicat valide.");
        }
    }
}
