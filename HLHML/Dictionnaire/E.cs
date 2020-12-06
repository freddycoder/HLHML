using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class E : List<Terme>
    {
        public E() : base()
        {
            Add(new Terme
            {
                Mots = "Ensuite",
                Type = TypeTerme.Adverbe
            });
            Add(new Terme
            {
                Mots = "Est",
                Type = TypeTerme.Verbe
            });
            Add(new Terme
            {
                Mots = "égal à",
                Type = TypeTerme.EgalÀ
            });
            Add(new Terme
            {
                Mots = "Et",
                Type = TypeTerme.Conjonction
            });
        }
    }
}
