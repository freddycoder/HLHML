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
                Type = TokenType.Conjonction
            });
            Add(new Terme
            {
                Mots = "trois",
                Type = TokenType.Adjectif,
                ValeurNumérique = 3
            });
            Add(new Terme
            {
                Mots = "troisième",
                Type = TokenType.Adjectif
            });
        }
    }
}
