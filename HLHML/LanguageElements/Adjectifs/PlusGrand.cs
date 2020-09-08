using HLHML.Dictionnaire;

namespace HLHML.LanguageElements.Adjectifs
{
    public class PlusGrand : AST, Adjectif
    {
        public PlusGrand(Terme terme) : base(terme)
        {
        }

        public bool Valider()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException($"L'adjectif plus 'plus petit que' doit avoir deux noeuds enfants. Celui-ci en à {Childs.Count}.");
            }

            return NodeVisitor.EvalDouble(Childs[0]) > NodeVisitor.EvalDouble(Childs[1]);
        }
    }
}
