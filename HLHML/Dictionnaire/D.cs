using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class D : List<Terme>
    {
        public D() : base()
        {
            Add(new Terme
            {
                Mots = "de",
                Type = TypeTerme.Préposition
            });
            Add(new Terme
            {
                Mots = "définit",
                Type = TypeTerme.Verbe
            });
            Add(new Terme
            {
                Mots = "deux",
                Type = TypeTerme.Adjectif,
                ValeurNumérique = 2
            });
            Add(new Terme
            {
                Mots = "deuxième",
                Type = TypeTerme.Adjectif
            });
        }
    }
}
