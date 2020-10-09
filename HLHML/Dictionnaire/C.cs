using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class C : List<Terme>
    {
        public C() : base()
        {
            Add(new Terme
            {
                Mots = "cinq",
                Type = TypeTerme.Adjectif,
                ValeurNumérique = 5
            });
            Add(new Terme
            {
                Mots = "comme suit",
                Type = TypeTerme.Adverbe
            });
        }
    }
}
