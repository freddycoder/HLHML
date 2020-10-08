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
                Type = TokenType.Negation
            });
            Add(new Terme
            {
                Mots = "n'",
                Type = TokenType.Negation
            });
            Add(new Terme
            {
                Mots = "nombre",
                Type = TokenType.Nom
            });
            Add(new Terme
            {
                Mots = "nombres",
                Type = TokenType.Nom
            });
        }
    }
}
