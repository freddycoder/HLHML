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
                Type = TokenType.Préposition
            });
            Add(new Terme
            {
                Mots = "définit",
                Type = TokenType.Verbe
            });
            Add(new Terme
            {
                Mots = "deux",
                Type = TokenType.Adjectif,
                ValeurNumérique = 2
            });
            Add(new Terme
            {
                Mots = "deuxième",
                Type = TokenType.Adjectif
            });
        }
    }
}
