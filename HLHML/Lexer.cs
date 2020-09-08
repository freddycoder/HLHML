using HLHML.Dictionnaire;
using HLHML.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using static HLHML.Dictionnaire.TermeBuilder;

namespace HLHML
{
    public class Lexer
    {
        private static readonly IDictionary<string, TokenType> _termesConnues = ObtenirLesTermesConnues();

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

                return new Terme(value, GetTokenType(value));
            }
            else if (CurrentChar == '"')
            {
                var value = GetString();

                return new Terme(value, TokenType.Text);
            }
            else if (char.IsDigit(CurrentChar))
            {
                var nombre = GetNumber();

                return new Terme(nombre, TokenType.Nombre);
            }
            else if (CurrentChar == '.' || CurrentChar == ',' || CurrentChar == ':')
            {
                var point = CurrentChar.ToString();

                _pos++;

                return new Terme(point, TokenType.Ponctuation);
            }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                var operateur = new Terme(CurrentChar.ToString(), TokenType.OperateurMathematique);

                _pos++;

                return operateur;
            }
            else if (CurrentChar == '=')
            {
                var vaut = new Terme("vaut", TokenType.Verbe);

                _pos++;

                return vaut;
            }
            else if (CurrentChar == '%')
            {
                var modulo = new Terme("modulo", TokenType.OperateurMathematique);

                _pos++;

                return modulo;
            }

            return new Terme("", TokenType.None);
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

        private string GetString()
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
                return new Terme("", TokenType.None);
            }
        }

        private TokenType GetTokenType(string value)
        {
            if (_termesConnues.ContainsKey(value))
            {
                return _termesConnues[value];
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

                if (GetTokenType(sb.ToString()) == TokenType.Sujet &&
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

        /// <summary>
        /// Crée un nouveau dictionnaire contenant les terme connue à partire des fichier XML dans le dossier Dictionnare.
        /// </summary>
        /// <returns>Un dictionnaire avec les termes</returns>
        private static IDictionary<string, TokenType> ObtenirLesTermesConnues()
        {
            var words = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase);

            var serialiser = new XmlSerializer(typeof(Terme[]));

            for (char i = 'A'; i <= 'Z'; i++)
            {
                var file = Path.Combine(AppContext.BaseDirectory, "Dictionnaire", $"{i}.xml");

                if (File.Exists(file))
                {
                    using var stream = new StreamReader(file);

                    var termes = serialiser.Deserialize(stream) as Terme[];

                    foreach (Terme terme in termes ?? new Terme[0])
                    {
                        words.Add(terme.Mots, terme.Type);
                    }
                }
            }

            return words;
        }
    }
}
