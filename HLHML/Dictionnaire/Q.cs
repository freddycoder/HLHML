using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class Q : List<Terme>
    {
        public Q() : base()
        {
            Add(new Terme
            {
                Mots = "que",
                Type = TypeTerme.Complement
            });
            Add(new Terme
            {
                Mots = "quatre",
                Type = TypeTerme.Adjectif,
                ValeurNumérique = 4
            });
        }
    }
}
