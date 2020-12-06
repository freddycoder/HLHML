using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class N : List<Terme>
    {
        public N() : base()
        {
            Add(new Terme
            {
                Mots = "ne",
                Type = TypeTerme.Negation
            });
            Add(new Terme
            {
                Mots = "n'",
                Type = TypeTerme.Negation
            });
            Add(new Terme
            {
                Mots = "Non",
                Type = TypeTerme.Negation
            });
        }
    }
}
