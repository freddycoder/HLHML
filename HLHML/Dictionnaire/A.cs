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
                Type = TypeTerme.Préposition
            });
            Add(new Terme
            {
                Mots = "Afficher",
                Type = TypeTerme.Verbe
            });
            Add(new Terme
            {
                Mots = "Alors",
                Type = TypeTerme.Adverbe
            });
            Add(new Terme
            {
                Mots = "Ainsi",
                Type = TypeTerme.Adverbe
            });
        }
    }
}
