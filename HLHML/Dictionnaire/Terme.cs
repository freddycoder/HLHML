using System;

namespace HLHML.Dictionnaire
{
    public class Terme : IComparable<Terme>
    {
        public Terme()
        {

        }

        public Terme(string mots, TokenType type)
        {
            Mots = mots;
            Type = type;
        }

        public string? Mots { get; set; }
        public TokenType Type { get; set; }
        public int? ValeurNumérique { get; set; }

        public int CompareTo(Terme terme)
        {
            return Mots[0].CompareTo(terme.Mots[0]);
        }

        public override string ToString()
        {
            return $"{Type}: {Mots}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Terme t)
            {
                return Type == t.Type && Mots.Equals(t.Mots, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Mots.GetHashCode();
        }
    }

    public static class TermeBuilder
    {
        public static Terme Terme(string mots, TokenType type)
        {
            return new Terme(mots, type);
        }
    }
}
