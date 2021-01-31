using HLHML.Exceptions;
using System.Globalization;
using System.Text;
using static HLHML.TermeBuilder;
using static HLHML.DictionnaireTermeConnue;

namespace HLHML.AnalyseurLexical
{
    public class Lexer : ILexer
    {
        private readonly string _text;

        private readonly int _limit;

        public char CurrentChar { get; set; }

        public char PeekChar => Position > _limit ? '\0' : _text[Position + 1];

        public Lexer(string text)
        {
            _text = text;
            Position = 0;
            if (text.Length > 0)
            {
                CurrentChar = _text[Position];
            }
            _limit = _text.Length - 1;
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

                Incrementer();

                DernierTerme.Terme = Terme(point, TypeTerme.Ponctuation);

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = Terme(CurrentChar.ToString(), TypeTerme.OperateurMathematique);

                Incrementer();

                DernierTerme.Terme = operateur;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '=')
            {
                if (PeekChar == '=')
                {
                    var egalÀ = Terme("==", TypeTerme.EgalÀ);

                    Incrementer();
                    Incrementer();

                    DernierTerme.Terme = egalÀ;

                    return DernierTerme.Terme;
                }
                else
                {
                    var vaut = Terme("vaut", TypeTerme.Verbe);

                    Incrementer();

                    DernierTerme.Terme = vaut;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '%')
            {
                var modulo = Terme("modulo", TypeTerme.OperateurMathematique);

                Incrementer();

                DernierTerme.Terme = modulo;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '(')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.OuvertureParenthèse);

                Incrementer();

                DernierTerme.Terme = terme;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == ')')
            {
                var terme = Terme(CurrentChar.ToString(), TypeTerme.FermetureParenthèse);

                Incrementer();

                DernierTerme.Terme = terme;

                return DernierTerme.Terme;
            }
            else if (CurrentChar == '>')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme(">=", TypeTerme.PlusGrandOuEgalÀ);

                    Incrementer();
                    Incrementer();

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusGrandQue);

                    Incrementer();

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '<')
            {
                if (PeekChar == '=')
                {
                    var terme = Terme("<=", TypeTerme.PlusPetitOuEgalÀ);

                    Incrementer();
                    Incrementer();

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
                else
                {
                    var terme = Terme(CurrentChar.ToString(), TypeTerme.PlusPetitQue);

                    Incrementer();

                    DernierTerme.Terme = terme;

                    return DernierTerme.Terme;
                }
            }
            else if (CurrentChar == '!' && PeekChar == '=')
            {
                var terme = Terme("!=", TypeTerme.DifferentDe);

                Incrementer();
                Incrementer();

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
                Incrementer();

                if (!partieDecimaleCommencé && (CurrentChar == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]) && char.IsDigit(PeekChar))
                {
                    sb.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    Incrementer();
                    partieDecimaleCommencé = true;
                }
            }

            return sb.ToString();
        }

        private string ObtenirChaineDeTexte()
        {
            var sb = new StringBuilder();

            var firstQuoteIndice = Position;

            Incrementer();

            while (CurrentChar != '"' && Position < _text.Length)
            {
                sb.Append(CurrentChar);
                Incrementer();
            }

            if (_text.Length == Position)
            {
                throw new NonClosingQuoteException("No closing quote matching", firstQuoteIndice, _text);
            }

            Incrementer();

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
                Incrementer();
            }

            if (CurrentChar == '\'')
            {
                sb.Append(CurrentChar);
                Incrementer();
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
                Incrementer();
            }
        }

        /// <summary>
        /// Sauter les prochains espaces blanc pour se positionner sur le prochain caractère
        /// </summary>
        public void Incrementer()
        {
            if (_limit > Position)
            {
                Position++;

                CurrentChar = _text[Position];
            }
            else
            {
                CurrentChar = '\0';

                if (Position < _text.Length)
                {
                    Position++;
                }
            }
        }
    }
}
