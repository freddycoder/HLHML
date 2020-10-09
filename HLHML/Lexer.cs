using HLHML.Exceptions;
using System.Globalization;
using System.Text;
using static HLHML.TermeBuilder;
using static HLHML.DictionnaireTermeConnue;

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

                return Terme(value, ObtenirTypeTerme(value));
            }
            else if (CurrentChar == '"')
            {
                var value = ObtenirChaineDeTexte();

                return Terme(value, TypeTerme.Text);
            }
            else if (char.IsDigit(CurrentChar))
            {
                var nombre = GetNumber();

                return Terme(nombre, TypeTerme.Nombre);
            }
            else if (CurrentChar == '.' || CurrentChar == ',' || CurrentChar == ':')
            {
                var point = CurrentChar.ToString();

                _pos++;

                return Terme(point, TypeTerme.Ponctuation);
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = Terme(CurrentChar.ToString(), TypeTerme.OperateurMathematique);

                _pos++;

                return operateur;
            }
            else if (CurrentChar == '=')
            {
                if (PeekChar == '=')
                {
                    var egalÀ = Terme("==", TypeTerme.EgalÀ);

                    _pos++;
                    _pos++;

                    return egalÀ;
                }
                else
                {
                    var vaut = Terme("vaut", TypeTerme.Verbe);

                    _pos++;

                    return vaut;
                }
            }
            else if (CurrentChar == '%')
            {
                var modulo = Terme("modulo", TypeTerme.OperateurMathematique);

                _pos++;

                return modulo;
            }
            else if (CurrentChar == '(')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.OuvertureParenthèse);

                _pos++;

                return terme;
            }
            else if (CurrentChar == ')')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.FermetureParenthèse);

                _pos++;

                return terme;
            }
            else if (CurrentChar == '>')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme(">=", TypeTerme.PlusGrandOuEgalÀ);

                    _pos++;
                    _pos++;

                    return terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusGrandQue);

                    _pos++;

                    return terme;
                }
            }
            else if (CurrentChar == '<')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme("<=", TypeTerme.PlusPetitOuEgalÀ);

                    _pos++;
                    _pos++;

                    return terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusPetitQue);

                    _pos++;

                    return terme;
                }
            }
            else if (CurrentChar == '!' && PeekChar == '=')
            {
                var terme = Terme("!=", TypeTerme.DifferentDe);

                _pos++;
                _pos++;

                return terme;
            }

            return Terme("", TypeTerme.None);
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
                return Terme("", TypeTerme.None);
            }
        }

        private TypeTerme ObtenirTypeTerme(string mots)
        {
            //if (mots.Length > 1 && mots.EstPluriel())
            //{
            //    mots = mots.AccorderSingulier();
            //}

            if (TermesConnues.ContainsKey(mots))
            {
                return TermesConnues[mots].Type;
            }

            return TypeTerme.Sujet;
        }

        private string GetNextWord()
        {
            var sb = new StringBuilder();

            while (char.IsLetterOrDigit(CurrentChar))
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

                var actuelTypeTerme = ObtenirTypeTerme(sb.ToString());

                if (actuelTypeTerme != TypeTerme.Adverbe &&
                    actuelTypeTerme == TypeTerme.Sujet &&
                     (nextTokenPeeked.Type == TypeTerme.Sujet ||
                      nextTokenPeeked.Type == TypeTerme.Complement ||
                      nextTokenPeeked.Type == TypeTerme.Préposition) &&
                      !nextTokenPeeked.Equals(Terme("de", TypeTerme.Préposition)))
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
