using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class P : List<Terme>
    {
        public P() : base()
        {
            Add(new Terme
            {
                Mots = "pas",
                Type = TokenType.Negation
            });
            Add(new Terme
            {
                Mots = "plus petit que",
                Type = TokenType.Adjectif
            });
            Add(new Terme
            {
                Mots = "plus grand que",
                Type = TokenType.Adjectif
            });
            Add(new Terme
            {
                Mots = "pour",
                Type = TokenType.Conjonction
            });
            Add(new Terme
            {
                Mots = "premier",
                Type = TokenType.Adjectif
            });
        }
    }
}
