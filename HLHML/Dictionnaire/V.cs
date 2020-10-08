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
                Type = TokenType.Verbe
            });
            Add(new Terme
            {
                Mots = "variant",
                Type = TokenType.Verbe
            });
        }
    }
}
