using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class U : List<Terme>
    {
        public U() : base()
        {
            Add(new Terme
            {
                Mots = "Un",
                Type = TypeTerme.Adjectif,
                ValeurNumérique = 1
            });
        }
    }
}
