using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Liste : AST
    {
        public Liste(AST firstChild, Terme terme, AST secondChild) : base(firstChild, terme, secondChild)
        {
        }
    }
}
