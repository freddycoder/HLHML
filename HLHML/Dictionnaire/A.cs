using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class A : List<Terme>
    {
        public A() : base()
        {
            Add(new Terme
            {
                Mots = "à",
                Type = TokenType.Préposition
            });
            Add(new Terme
            {
                Mots = "Afficher",
                Type = TokenType.Verbe
            });
            Add(new Terme
            {
                Mots = "Alors",
                Type = TokenType.Adverbe
            });
            Add(new Terme
            {
                Mots = "Ainsi",
                Type = TokenType.Adverbe
            });
        }
    }
}
