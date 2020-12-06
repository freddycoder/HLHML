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
                Type = TypeTerme.Negation
            });
            Add(new Terme
            {
                Mots = "plus petit que",
                Type = TypeTerme.PlusPetitQue
            });
            Add(new Terme
            {
                Mots = "plus grand que",
                Type = TypeTerme.PlusGrandQue
            });
            Add(new Terme
            {
                Mots = "pour",
                Type = TypeTerme.Conjonction
            });
            Add(new Terme
            {
                Mots = "premier",
                Type = TypeTerme.Adjectif
            });
        }
    }
}
