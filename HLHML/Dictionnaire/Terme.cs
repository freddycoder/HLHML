using System;
using System.Collections.Generic;
using System.IO;
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
                        words.Add(terme.Mots, terme);
                    }
                }
            }

            return words;
        }
    }
}
