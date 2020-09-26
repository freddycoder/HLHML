using HLHML.Dictionnaire;
using static HLHML.Dictionnaire.DictionnaireTermeConnue;

namespace HLHML
{
    public static class Extensions
    {
        public static bool EstPluriel(this Terme terme)
        {
            if (terme.Type == TypeTerme.Nom && terme.Mots.EstPluriel()) 
            {
                return true;
            }

            return false;
        }

        public static bool EstPluriel(this string terme)
        {
            if (TermesConnues.ContainsKey(terme) == false && terme.EndsWith('s') || terme.EndsWith('x'))
            {
                return true;
            }

            return false;
        }

        public static string AccorderSingulier(this string terme)
        {
            if (terme.EstPluriel())
            {
                return terme[0..^1];
            }

            return terme;
        }
    }
}
