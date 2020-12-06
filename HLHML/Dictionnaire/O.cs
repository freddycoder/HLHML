using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class O : List<Terme>
    {
        public O() : base()
        {
            Add(new Terme
            {
                Mots = "Ou",
                Type = TypeTerme.Conjonction
            });
        }
    }
}
