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
                Type = TokenType.Adverbe
            });
            Add(new Terme
            {
                Mots = "Est",
                Type = TokenType.Verbe
            });
            Add(new Terme
            {
                Mots = "égal à",
                Type = TokenType.Adjectif
            });
        }
    }
}
