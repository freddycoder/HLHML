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
            return NodeVisitor.Eval(Childs[0]) || NodeVisitor.Eval(Childs[1]);
        }
    }
}
