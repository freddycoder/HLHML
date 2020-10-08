using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class S : List<Terme>
    {
        public S() : base()
        {
            Add(new Terme
            {
                Mots = "Se",
                Type = TokenType.Déterminant
            });
            Add(new Terme
            {
                Mots = "Si",
                Type = TokenType.Déterminant,
                ValeurNumérique = 4
            });
            Add(new Terme
            {
                Mots = "Sinon",
                Type = TokenType.Conjonction
            });
        }
    }
}
