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
            DernierTerme = new DernierTerme();
        }

        public int Position { get; private set; }

        public DernierTerme DernierTerme { get; private set; }

        public Terme ObtenirProchainTerme()
        {
            Avancer();

            DernierTerme.Position = Position;

            if (char.IsLetter(CurrentChar))
            {
                var value = GetNextWord();

                DernierTerme.Terme = Terme(value, ObtenirTypeTerme(value));

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '"')
            {
                var value = ObtenirChaineDeTexte();

                DernierTerme.Terme = Terme(value, TypeTerme.Text);

                return DernierTerme.Terme;
            }
            else if (char.IsDigit(CurrentChar))
            {
                var nombre = GetNumber();

                DernierTerme.Terme = Terme(nombre, TypeTerme.Nombre);

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '.' || CurrentChar == ',' || CurrentChar == ':')
            {
                var point = CurrentChar.ToString();

                Position++;

                DernierTerme.Terme = Terme(point, TypeTerme.Ponctuation);

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = Terme(CurrentChar.ToString(), TypeTerme.OperateurMathematique);

                Position++;

                DernierTerme.Terme = operateur;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '=')
            {
                if (PeekChar == '=')
                {
                    var egalÀ = Terme("==", TypeTerme.EgalÀ);

                    Position++;
                    Position++;

                    DernierTerme.Terme = egalÀ;

                    return DernierTerme.Terme;
                }
                else
                {
                    var vaut = Terme("vaut", TypeTerme.Verbe);

                    Position++;

                    DernierTerme.Terme = vaut;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '%')
            {
                var modulo = Terme("modulo", TypeTerme.OperateurMathematique);

                Position++;

                DernierTerme.Terme = modulo;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '(')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.OuvertureParenthèse);

                Position++;

                DernierTerme.Terme = terme;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == ')')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.FermetureParenthèse);

                Position++;

                DernierTerme.Terme = terme;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '>')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme(">=", TypeTerme.PlusGrandOuEgalÀ);

                    Position++;
                    Position++;

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusGrandQue);

                    Position++;

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '<')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme("<=", TypeTerme.PlusPetitOuEgalÀ);

                    Position++;
                    Position++;

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusPetitQue);

                    Position++;

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '!' && PeekChar == '=')
            {
                var terme = Terme("!=", TypeTerme.DifferentDe);

                Position++;
                Position++;

                DernierTerme.Terme = terme;

                return DernierTerme.Terme;
            }

            DernierTerme.Terme = Terme("", TypeTerme.None);

            return DernierTerme.Terme;
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
