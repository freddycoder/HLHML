using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class V : List<Terme>
    {
        public V() : base()
        {
            Add(new Terme
            {
                Mots = "Vaut",
                Type = TypeTerme.Verbe
            });
            Add(new Terme
            {
                Mots = "variant",
                Type = TypeTerme.Verbe
            });
        }
    }
}
