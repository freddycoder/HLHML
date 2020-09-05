using System;

namespace HLHML
{
    public enum TokenType
    {
        Sujet, Verbe, Adjectif, Determinant, Conjonction, None, Nombre, Text, Compound, Ponctuation,
        OperateurMathematique,
        Negation,
        Adverbe,
        Complement,
        Preposition,
        OuvertureParentèse
    }

    public class Token
    {
        public readonly TokenType Type;
        public readonly string Value;

        public Token(string value, TokenType type)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Token t)
            {
                return Type == t.Type && Value.Equals(t.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Value.GetHashCode();
        }
    }
}
