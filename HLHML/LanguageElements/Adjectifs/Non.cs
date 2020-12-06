using HLHML.Dictionnaire;

namespace HLHML.LanguageElements.Adjectifs
{
    public class Non : AST, Adjectif
    {
        public Non(Terme terme, AST firstChild) : base(terme, firstChild)
        {

        }

        public bool Valider()
        {
            return !NodeVisitor.Eval(Childs[0]);
        }
    }
}
