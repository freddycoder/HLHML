using HLHML.Dictionnaire;
using System;

namespace HLHML.LanguageElements.Syntaxe
{
    /// <summary>
    /// Insertion, dans le corps d'une phrase, d'un élément qui, à la différence de l'incise, interrompt la construction syntaxique ; cet élément
    /// </summary>
    public class Parenthese : AST
    {
        /// <exception cref="ArgumentException">Si token.Type != TypeTerme.OuvertureParentèse</exception>
        public Parenthese(Terme terme, AST firstChild) : base(terme, firstChild)
        {
            if (terme.Type != TypeTerme.OuvertureParenthèse)
            {
                throw new ArgumentException($"Impossible de construre un noeud de type Parenthese avec le terme {terme}");
            }
        }
    }
}
