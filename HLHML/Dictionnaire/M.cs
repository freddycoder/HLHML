using System.Collections.Generic;

namespace HLHML.Dictionnaire
{
    public class M : List<Terme>
    {
        public M() : base()
        {
            Add(new Terme
            {
                Mots = "modulo",
                Type = TokenType.OperateurMathematique
            });
        }
    }
}
