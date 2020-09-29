using System.IO;
using System.Linq;
using HLHML.Dictionnaire;

namespace HLHML.LanguageElements
{
    public class Lire : AST, IActionnable
    {
        private readonly TextReader _textReader;

        public Lire(Terme terme, TextReader textReader) : base(terme)
        {
            _textReader = textReader;
        }

        public void Actionner()
        {
            Scope[_childs.First().Value] = _textReader.ReadLine();
        }
    }
}
