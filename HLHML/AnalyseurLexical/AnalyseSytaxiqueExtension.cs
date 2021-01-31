using System.Collections.Generic;
using static HLHML.TermeBuilder;

namespace HLHML.AnalyseurLexical
{
    public static class AnalyseSytaxiqueExtension
    {
        public static ILexer PrendreEnComptesEspacement(this ILexer lexer) => new EspacementDecorateur(lexer);
    }
}
