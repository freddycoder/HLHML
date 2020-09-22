using HLHML.Dictionnaire;
using HLHML.Exceptions;
using System.Globalization;
using System.Text;
using static HLHML.Dictionnaire.TermeBuilder;
using static HLHML.Dictionnaire.DictionnaireTermeConnue;

namespace HLHML
{
    public class Lexer
    {
        private readonly string _text;
        private int _pos;
        
        private char CurrentChar => _pos >= _text.Length ? '\0' : _text[_pos];

        private char PeekChar => _pos + 1 >= _text.Length ? '\0' : _text[_pos + 1];

        public Lexer(string text)
        {
            _text = text;
            _pos = 0;
        }

        public Terme ObtenirProchainTerme()
        {
            Avancer();

            if (char.IsLetter(CurrentChar))
            {
                var value = GetNextWord();

                return Terme(value, ObtenirTokenType(value));
            }
            else if (CurrentChar == '"')
            {
                var value = ObtenirChaineDeTexte();

                return Terme(value, TokenType.Text);
            }
            else if (char.IsDigit(CurrentChar))
            {
                var nombre = GetNumber();

                return Terme(nombre, TokenType.Nombre);
            }
            else if (CurrentChar == '.' || CurrentChar == ',' || CurrentChar == ':')
            {
                var point = CurrentChar.ToString();

                _pos++;

                return Terme(point, TokenType.Ponctuation);
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = Terme(CurrentChar.ToString(), TokenType.OperateurMathematique);

                _pos++;

                return operateur;
            }
            else if (CurrentChar == '=')
            {
                var vaut = Terme("vaut", TokenType.Verbe);

                _pos++;

                return vaut;
            }
            else if (CurrentChar == '%')
            {
                var modulo = Terme("modulo", TokenType.OperateurMathematique);

                _pos++;

                return modulo;
            }

            return Terme("", TokenType.None);
        }

        private string GetNumber()
        {
            var sb = new StringBuilder();

            var partieDecimaleCommencé = false;

            while (char.IsDigit(CurrentChar))
            {
                sb.Append(CurrentChar);
                _pos++;

                if (!partieDecimaleCommencé && (CurrentChar == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]) && char.IsDigit(PeekChar))
                {
                    sb.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    _pos++;
                    partieDecimaleCommencé = true;
                }
            }

            return sb.ToString();
        }

        private string ObtenirChaineDeTexte()
        {
            var sb = new StringBuilder();

            var firstQuoteIndice = _pos;

            _pos++;

            while (CurrentChar != '"' && _pos < _text.Length)
            {
                sb.Append(CurrentChar);
                _pos++;
            }

            if (_text.Length == _pos)
            {
                throw new NonClosingQuoteException("No closing quote matching", firstQuoteIndice, _text);
            }

            _pos++;

            return sb.ToString();
        }

        private Terme PeekNextToken()
        {
            var sublexer = new Lexer(_text.Substring(_pos));

            try
            {
                return sublexer.ObtenirProchainTerme();
            }
            catch
            {
                return Terme("", TokenType.None);
            }
        }

        private TokenType ObtenirTokenType(string mots)
        {
            //if (mots.Length > 1 && mots.EstPluriel())
            //{
            //    mots = mots.AccorderSingulier();
            //}

            if (TermesConnues.ContainsKey(mots))
            {
                return TermesConnues[mots].Type;
            }

            return TokenType.Sujet;
        }

        private string GetNextWord()
        {
            var sb = new StringBuilder();

            while (char.IsLetter(CurrentChar))
            {
                sb.Append(CurrentChar);
                _pos++;
            }

            if (CurrentChar == '\'')
            {
                sb.Append(CurrentChar);
                _pos++;
            }
            else
            {
                var nextTokenPeeked = PeekNextToken();

                var actuelTokenType = ObtenirTokenType(sb.ToString());

                if (actuelTokenType != TokenType.Adverbe &&
                    actuelTokenType == TokenType.Sujet &&
                     (nextTokenPeeked.Type == TokenType.Sujet ||
                      nextTokenPeeked.Type == TokenType.Complement ||
                      nextTokenPeeked.Type == TokenType.Préposition) &&
                      !nextTokenPeeked.Equals(Terme("de", TokenType.Préposition)))
                {
                    Avancer();

                    sb.Append($" {GetNextWord()}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sauter les prochains espaces blanc pour se positionner sur le prochain caractère
        /// </summary>
        private void Avancer()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                _pos++;
            }
        }
    }
}
