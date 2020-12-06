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

        private char CurrentChar => Position >= _text.Length ? '\0' : _text[Position];

        private char PeekChar => Position + 1 >= _text.Length ? '\0' : _text[Position + 1];

        public Lexer(string text)
        {
            _text = text;
            Position = 0;
        }

        public int Position { get; private set; }

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

                Position++;

                return Terme(point, TypeTerme.Ponctuation);
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = Terme(CurrentChar.ToString(), TypeTerme.OperateurMathematique);

                Position++;

                return operateur;
            }
            else if (CurrentChar == '=')
            {
                if (PeekChar == '=')
                {
                    var egalÀ = Terme("==", TypeTerme.EgalÀ);

                    Position++;
                    Position++;

                    return egalÀ;
                }
                else
                {
                    var vaut = Terme("vaut", TypeTerme.Verbe);

                    Position++;

                    return vaut;
                }
            }
            else if (CurrentChar == '%')
            {
                var modulo = Terme("modulo", TypeTerme.OperateurMathematique);

                Position++;

                return modulo;
            }
            else if (CurrentChar == '(')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.OuvertureParenthèse);

                Position++;

                return terme;
            }
            else if (CurrentChar == ')')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.FermetureParenthèse);

                Position++;

                return terme;
            }
            else if (CurrentChar == '>')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme(">=", TypeTerme.PlusGrandOuEgalÀ);

                    Position++;
                    Position++;

                    return terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusGrandQue);

                    Position++;

                    return terme;
                }
            }
            else if (CurrentChar == '<')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme("<=", TypeTerme.PlusPetitOuEgalÀ);

                    Position++;
                    Position++;

                    return terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusPetitQue);

                    Position++;

                    return terme;
                }
            }
            else if (CurrentChar == '!' && PeekChar == '=')
            {
                var terme = Terme("!=", TypeTerme.DifferentDe);

                Position++;
                Position++;

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
                Position++;

                if (!partieDecimaleCommencé && (CurrentChar == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]) && char.IsDigit(PeekChar))
                {
                    sb.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    Position++;
                    partieDecimaleCommencé = true;
                }
            }

            return sb.ToString();
        }

        private string ObtenirChaineDeTexte()
        {
            var sb = new StringBuilder();

            var firstQuoteIndice = Position;

            Position++;

            while (CurrentChar != '"' && Position < _text.Length)
            {
                sb.Append(CurrentChar);
                Position++;
            }

            if (_text.Length == Position)
            {
                throw new NonClosingQuoteException("No closing quote matching", firstQuoteIndice, _text);
            }

            Position++;

            return sb.ToString();
        }

        private Terme PeekNextToken()
        {
            var sublexer = new Lexer(_text.Substring(Position));

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
                Position++;
            }

            if (CurrentChar == '\'')
            {
                sb.Append(CurrentChar);
                Position++;
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
                Position++;
            }
        }
    }
}
