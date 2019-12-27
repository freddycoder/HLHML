using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHML
{
    public class Lexer
    {
        private readonly string _text;
        private int _pos;
        private readonly IDictionary<string, TokenType> _knowedWords = new Dictionary<string, TokenType>
        (StringComparer.OrdinalIgnoreCase)
        {
            { "Afficher", TokenType.Verbe },
            { "Vaut", TokenType.Verbe },
            { "Lire", TokenType.Verbe },
            { "Est", TokenType.Verbe },
            { "La", TokenType.Determinant },
            { "Si", TokenType.Conjonction },
            { "Sinon", TokenType.Conjonction },
            { "Egual", TokenType.Adjectif },
            { "à", TokenType.Determinant }
        };

        private char CurrentChar => _pos >= _text.Length ? '\0' : _text[_pos];

        public Lexer(string text)
        {
            _text = text;
            _pos = 0;
        }

        public Token GetNextToken()
        {
            Advance();

            if (char.IsLetter(CurrentChar))
            {
                var value = GetNextWord();

                return new Token(value, GetTokenType(value));
            }
            else if (CurrentChar == '"')
            {
                var value = GetString();

                return new Token(value, TokenType.Text);
            }
            else if (char.IsDigit(CurrentChar))
            {
                var nombre = GetNumber();

                return new Token(nombre, TokenType.Nombre);
            }
            else if (CurrentChar == '.' || CurrentChar == ',')
            {
                var point = CurrentChar.ToString();

                _pos++;

                return new Token(point, TokenType.Ponctuation);
            }
            else if (CurrentChar == '+')
            {
                var operateur = new Token(CurrentChar.ToString(), TokenType.OperateurMathematique);

                _pos++;

                return operateur;
            }

            return new Token("", TokenType.None);
        }

        private string GetNumber()
        {
            var sb = new StringBuilder();

            while (char.IsDigit(CurrentChar))
            {
                sb.Append(CurrentChar);
                _pos++;
            }

            return sb.ToString();
        }

        private string GetString()
        {
            StringBuilder sb = new StringBuilder();

            _pos++;

            while (CurrentChar != '"')
            {
                sb.Append(CurrentChar);
                _pos++;
            }

            _pos++;

            return sb.ToString();
        }

        private TokenType GetTokenType(string value)
        {
            if (_knowedWords.ContainsKey(value))
            {
                return _knowedWords[value];
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

            return sb.ToString();
        }

        private void Advance()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                _pos++;
            }
        }
    }
}
