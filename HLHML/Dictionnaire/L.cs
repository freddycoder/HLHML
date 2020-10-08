using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class L : List<Terme>
    {
        public L() : base()
        {
            Add(new Terme
            {
                Mots = "La",
                Type = TokenType.Déterminant
            });
            Add(new Terme
            {
                Mots = "Le",
                Type = TokenType.Déterminant
            });
            Add(new Terme
            {
                Mots = "Lire",
                Type = TokenType.Verbe
            });
        }
    }
}
