using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Ou : AST, Adjectif
    {
        public Ou(AST rightNode, Terme terme, AST leftNode) : base(rightNode, terme, leftNode)
        {

        }

        public bool Valider()
        {
            var branche1 = NodeVisitor.Eval(Childs[0]);
            var branche2 = NodeVisitor.Eval(Childs[1]);

            return branche1 || branche2;
        }
    }
}
