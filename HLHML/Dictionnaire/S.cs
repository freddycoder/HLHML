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
                Type = TypeTerme.Déterminant
            });
            Add(new Terme
            {
                Mots = "Si",
                Type = TypeTerme.Conjonction,
                ValeurNumérique = 4
            });
            Add(new Terme
            {
                Mots = "Sinon",
                Type = TypeTerme.Conjonction
            });
        }
    }
}
