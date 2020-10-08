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
                Type = TokenType.Complement
            });
            Add(new Terme
            {
                Mots = "quatre",
                Type = TokenType.Adjectif,
                ValeurNumérique = 4
            });
        }
    }
}
