using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace HLHML.Dictionnaire
{
    public class Terme : IComparable<Terme>
    {
        public Terme() 
        { 
            Mots = string.Empty; 
        }

        public Terme(string mots, TokenType type)
        {
            Mots = mots;
            Type = type;
        }

        public string Mots { get; set; }
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

    public class DictionnaireTermeConnue
    {
        public static readonly IReadOnlyDictionary<string, Terme> TermesConnues = ObtenirLesTermesConnues();

        /// <summary>
        /// Crée un nouveau dictionnaire contenant les terme connue à partire des fichier XML dans le dossier Dictionnare.
        /// </summary>
        /// <returns>Un dictionnaire avec les termes</returns>
        private static IReadOnlyDictionary<string, Terme> ObtenirLesTermesConnues()
        {
            var words = new Dictionary<string, Terme>(StringComparer.OrdinalIgnoreCase);

            var assembly = Assembly.GetAssembly(typeof(A));

            for (char i = 'A'; i <= 'Z'; i++)
            {
                var t = assembly.GetTypes().FirstOrDefault(t => t.Name == i.ToString());

                if (t != default)
                {
                    var termes = Activator.CreateInstance(t) as List<Terme> ?? throw new ApplicationException("Instance of t should be castable to List<Terme>");

                    foreach (var terme in termes)
                    {
                        words.Add(terme.Mots, terme);
                    }
                }
            }

            return words;
        }
    }
}
