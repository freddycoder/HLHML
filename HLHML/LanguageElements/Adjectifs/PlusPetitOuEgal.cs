using HLHML.Dictionnaire;

namespace HLHML.LanguageElements.Adjectifs
{
    public class PlusPetitOuEgal : AST, Adjectif
    {
        public PlusPetitOuEgal(AST rightChild, Terme terme, AST leftChild) : base(rightChild, terme, leftChild)
        {

        }

        public bool Valider()
        {
            if (Childs.Count != 2)
            {
                throw new InvalidNodeNumberException($"L'adjectif plus 'plus petit que' doit avoir deux noeuds enfants. Celui-ci en à {Childs.Count}.");
            }

            return NodeVisitor.EvalDouble(Childs[0]) <= NodeVisitor.EvalDouble(Childs[1]);
        }
    }
}
