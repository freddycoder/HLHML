using System.Text;
using static HLHML.TermeBuilder;

namespace HLHML.AnalyseurLexical
{
    public class EspacementDecorateur : ILexer
    {
        private ILexer _lexer;

        public EspacementDecorateur(ILexer lexer)
        {
            _lexer = lexer;
        }

        public int Position => _lexer.Position;

        public DernierTerme DernierTerme => _lexer.DernierTerme;

        public char CurrentChar
        {
            get => _lexer.CurrentChar;
            set
            {
                _lexer.CurrentChar = value;
            }
        }

        public char PeekChar => _lexer.PeekChar;

        public void Incrementer() => _lexer.Incrementer();

        public Terme ObtenirProchainTerme()
        {
            if (char.IsWhiteSpace(_lexer.CurrentChar))
            {
                switch (_lexer.CurrentChar)
                {
                    case '\n':
                        Incrementer();
                        return new Terme("\\n", TypeTerme.SautDeLigne);
                    case ' ':
                    case '\t':
                        var terme = ObtenirIndentation();

                        if (terme != null)
                        {
                            return terme;
                        }

                        break;
                }
            }

            return _lexer.ObtenirProchainTerme();
        }

        private Terme? ObtenirIndentation()
        {
            if (_lexer.CurrentChar == '\t')
            {
                var sb = new StringBuilder();

                while (_lexer.CurrentChar == '\t')
                {
                    sb.Append('\t');
                    Incrementer();
                }

                return Terme(sb.ToString(), TypeTerme.Indentation);
            }
            else if (_lexer.PeekChar == ' ')
            {
                var sb = new StringBuilder();

                while (_lexer.CurrentChar == ' ')
                {
                    sb.Append(' ');
                    Incrementer();
                }

                return Terme(sb.ToString(), TypeTerme.Indentation);
            }
            else if (_lexer.PeekChar == '\n')
            {
                Incrementer();
                Incrementer();
                return new Terme("\\n", TypeTerme.SautDeLigne);
            }

            return default;
        }
    }
}
