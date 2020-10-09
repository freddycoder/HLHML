using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class T : List<Terme>
    {
        public T() : base()
        {
            Add(new Terme
            {
                Mots = "tant que",
                Type = TypeTerme.Conjonction
            });
            Add(new Terme
            {
                Mots = "trois",
                Type = TypeTerme.Adjectif,
                ValeurNumérique = 3
            });
            Add(new Terme
            {
                Mots = "troisième",
                Type = TypeTerme.Adjectif
            });
        }
    }
}
