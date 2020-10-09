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
                Type = TypeTerme.Déterminant
            });
            Add(new Terme
            {
                Mots = "Le",
                Type = TypeTerme.Déterminant
            });
            Add(new Terme
            {
                Mots = "Lire",
                Type = TypeTerme.Verbe
            });
        }
    }
}
