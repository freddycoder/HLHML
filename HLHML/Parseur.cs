using HLHML.Dictionnaire;
using HLHML.Exceptions;
using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using System;
using System.Collections.Generic;
using System.IO;
using static HLHML.Dictionnaire.TermeBuilder;

namespace HLHML
{
    public class Parseur
    {
        private readonly Lexer _lexer;
        private Terme TermeActuel { get; set; }
        private Scope? _actuelScope;

        private TextWriter _textWriter;
        private bool _newLine;

        public Parseur(Lexer lexer)
        {
            _lexer = lexer;
            _textWriter = Console.Out;

            TermeActuel = ObtenirProchainTerme();
        }

        private Terme ObtenirProchainTerme()
        {
            TermeActuel = _lexer.ObtenirProchainTerme();
            return TermeActuel;
        }

        internal void SetTextWriter(TextWriter textWriter, bool newLineWhenAfficher = false)
        {
            _textWriter = textWriter;
            _newLine = newLineWhenAfficher;
        }

        private void UpdateScopeReference(Scope scope)
        {
            if (scope == null && _actuelScope == null)
            {
                _actuelScope = new Scope();
            }
            else if (scope != null && _actuelScope == null)
            {
                _actuelScope = scope;
            }
            else if (scope == null && _actuelScope != null)
            {
                _actuelScope = new Scope(_actuelScope);
            }
            else if (scope != null && _actuelScope != null)
            {
                if (scope.Parent != _actuelScope)
                {
                    throw new InvalidScopeException("Scopes does not reference each others properly");
                }

                _actuelScope = scope;
            }
        }

        /// <summary>
        /// Parser le programme et le transformer sous la représentation d'un arbre de syntaxe abstrait
        /// </summary>
        /// <param name="scope">La scope des variables. Si null, une nouvelle scope sera initialisé</param>
        /// <returns>L'arbre de syntaxe abstrait représentant le programme</returns>
        public AST Parse(Scope? scope = null)
        {
            return GeneriqueCompound(scope, () => TermeActuel.Type != TokenType.None && TermeActuel.Type != TokenType.Adverbe);
        }

        private AST InitialiserConjonction()
        {
            var conjonction = new Conjonction(TermeActuel);

            var isFirst = true;

            var predicatIsNegated = false;

            ObtenirProchainTerme();

            if (TermeActuel.Type == TokenType.Déterminant)
            {
                ObtenirProchainTerme();
            }

            var first = Expression();

            while (TermeActuel.Type != TokenType.None &&
                   (TermeActuel.Type == TokenType.Ponctuation ||
                   TermeActuel.Type != TokenType.Adverbe) &&
                   TermeActuel.Mots != "." &&
                   conjonction.Childs.Count < 3)
            {
                if (TermeActuel.Type == TokenType.Ponctuation)
                {
                    ObtenirProchainTerme();

                    if (TermeActuel.Equals(Terme("alors", TokenType.Adverbe)))
                    {
                        ObtenirProchainTerme();
                    }
                }
                if (TermeActuel.Equals(new Terme("sinon", TokenType.Conjonction)))
                {
                    ObtenirProchainTerme();
                }

                if (TermeActuel.Type == TokenType.Negation)
                {
                    predicatIsNegated = true;
                }
                else if (conjonction.Value.Equals("tant que", StringComparison.OrdinalIgnoreCase) &&
                         conjonction.Childs.Count == 1)
                {
                    conjonction.AddChild(Parse(new Scope(_actuelScope)));

                    break;
                }
                else if (TermeActuel.Type == TokenType.Verbe)
                {
                    conjonction.AddChild(InitialiserVerbe(first.Value));
                }
                else if (TermeActuel.Type == TokenType.Conjonction)
                {
                    conjonction.AddChild(InitialiserConjonction());
                }
                else if (TermeActuel.Type == TokenType.Sujet)
                {
                    conjonction.AddChild(Expression());
                }

                if (isFirst && TermeActuel.Type != TokenType.Negation)
                {
                    conjonction.Childs[0].AddChildsAsFirstChild(first);

                    isFirst = false;
                }
                else if (TermeActuel.Type == TokenType.Negation)
                {
                    ObtenirProchainTerme();
                }
            }

            conjonction.PredicatIsNegated = predicatIsNegated;

            return conjonction;
        }

        private AST InitialiserVerbe(string subject)
        {
            if (TermeActuel.Mots.Equals("Afficher", StringComparison.OrdinalIgnoreCase))
            {
                var ast = new Afficher(TermeActuel);

                ast.SetTextWriter(_textWriter, _newLine);

                return ast.AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (TermeActuel.Mots.Equals("Lire", StringComparison.OrdinalIgnoreCase))
            {
                return new Lire(TermeActuel).AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (TermeActuel.Mots.Equals("est", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNextAdjectif(subject);
            }
            else if (TermeActuel.Mots.Equals("variant", StringComparison.OrdinalIgnoreCase))
            {
                return ParseVariant(subject);
            }
            else if (TermeActuel.Mots.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                return BuildDefinition(subject);
            }

            throw new VerbeNotFoundException(TermeActuel);
        }

        private AST ParseVariant(string subject)
        {
            return new AST(TermeActuel).AddChild(Expression());
        }

        private AST ParseNextAdjectif(string subject)
        {
            ObtenirProchainTerme();

            AST? adj;
            if (TermeActuel.Mots.Equals("plus petit que", StringComparison.OrdinalIgnoreCase))
            {
                adj = new PlusPetit(TermeActuel);
            }
            else if (TermeActuel.Mots.Equals("plus grand que", StringComparison.OrdinalIgnoreCase))
            {
                adj = new PlusGrand(TermeActuel);
            }
            else
            {
                adj = new Egal(TermeActuel);
            }
            //else if (CurrentToken.Value.Equals("égal à", StringComparison.OrdinalIgnoreCase))
            //{
            //    adj = new Egal(CurrentToken);
            //}

            //if (adj == null)
            //{
            //    throw new InvalidSentenceException($"There is no adjectif know as {CurrentToken}.");
            //}

            adj.AddChilds(AfterVerbeAndAdjectifs(subject));

            return adj;
        }

        private List<AST> AfterVerbeAndAdjectifs(string subject)
        {
            var asts = new List<AST>();

            ObtenirProchainTerme();

            var skipDoWhile = false;

            if (TermeActuel.Type == TokenType.Adverbe)
            {
                ObtenirProchainTerme();

                if (TermeActuel.Type == TokenType.Ponctuation)
                {
                    asts.Add(BuildDefinition(subject));

                    skipDoWhile = true;
                }
            }

            if (!skipDoWhile)
            {
                do
                {
                    if (TermeActuel.Type == TokenType.Nombre || 
                        TermeActuel.Type == TokenType.Sujet || 
                        TermeActuel.Type == TokenType.OperateurMathematique)
                    {
                        asts.Add(Expression());
                    }
                    else if (TermeActuel.Type == TokenType.Text)
                    {
                        asts.Add(new AST(TermeActuel));

                        ObtenirProchainTerme();
                    }
                    else if (TermeActuel.Type == TokenType.Conjonction)
                    {
                        asts.Add(Parse(new Scope(_actuelScope)));
                    }
                    else
                    {
                        ObtenirProchainTerme();
                    }

                } while (TermeActuel.Type == TokenType.Nombre || TermeActuel.Type == TokenType.Sujet ||
                        TermeActuel.Type == TokenType.Text || TermeActuel.Type == TokenType.Déterminant ||
                        TermeActuel.Type == TokenType.OperateurMathematique);
            }

            return asts;
        }

        private AST BuildDefinition(string subject)
        {
            var traiteLeSujet = false;

            bool sujet_pas_traité_ou_definition_pas_terminer()
            {
                var @return = TermeActuel.Type != TokenType.None && TermeActuel.Type != TokenType.Adverbe;

                if (traiteLeSujet)
                {
                    return false;
                }

                traiteLeSujet = TermeActuel.Mots.Equals(subject, StringComparison.OrdinalIgnoreCase);

                return @return;
            }

            _actuelScope = new Scope(_actuelScope);

            var root = new AST(new Terme("Compound", TokenType.Compound), _actuelScope);

            ObtenirProchainTerme();

            while (sujet_pas_traité_ou_definition_pas_terminer())
            {
                if (TermeActuel.Type == TokenType.Sujet || TermeActuel.Type == TokenType.Nombre)
                {
                    root.AddChild(Expression());
                }
                else if (TermeActuel.Type == TokenType.Verbe)
                {
                    root.AddChild(InitialiserVerbe(subject));
                }
                else if (TermeActuel.Type == TokenType.Conjonction)
                {
                    root.AddChild(InitialiserConjonction());
                }

                if (sujet_pas_traité_ou_definition_pas_terminer())
                {
                    ObtenirProchainTerme();
                }
            }

            _actuelScope = _actuelScope.Parent;

            return root;
        }

        private AST GeneriqueCompound(Scope? scope, Func<bool> predicat)
        {
            UpdateScopeReference(scope);

            var root = new AST(new Terme("Compound", TokenType.Compound), _actuelScope);

            while (predicat.Invoke())
            {
                if (TermeActuel.Type == TokenType.Sujet || TermeActuel.Type == TokenType.Nombre)
                {
                    root.AddChild(Expression());
                }
                else if (TermeActuel.Type == TokenType.Verbe)
                {
                    root.AddChild(InitialiserVerbe(""));
                }
                else if (TermeActuel.Type == TokenType.Conjonction)
                {
                    root.AddChild(InitialiserConjonction());
                }

                if (TermeActuel.Type != TokenType.Adverbe)
                {
                    ObtenirProchainTerme();
                }
            }

            if (TermeActuel.Type == TokenType.Adverbe)
            {
                ObtenirProchainTerme();
            }

            _actuelScope = _actuelScope.Parent;

            return root;
        }

        private AST? Expression()
        {
            var node = Level_16();

            return node;
        }

        private AST? Level_16()
        {
            var node = Level_9();

            if (TermeActuel.Type == TokenType.Déterminant)
            {
                ObtenirProchainTerme();
            }

            if (TermeActuel.Mots.Equals("Vaut", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Vaut(node, t, Expression());
            }
            else if (TermeActuel.Mots.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Vaut(node, t, BuildDefinition(node.Value));
            }

            return node;
        }

        private AST? Level_9()
        {
            var node = Level_7();

            return node;
        }

        private AST? Level_7()
        {
            AST? node = Level_6();

            return node;
        }

        private AST? Level_6()
        {
            var node = Level_5();

            return node;
        }
        private AST? Level_5()
        {
            var node = Level_4();

            return node;
        }

        private AST? Level_4()
        {
            var node = Level_3();

            while (TermeActuel.Type == TokenType.OperateurMathematique && 
                  (TermeActuel.Mots == "+" || TermeActuel.Mots == "-"))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new OperateurMathematique(node, t, Level_3());
            }

            return node;
        }

        private AST? Level_3()
        {
            var node = Level_2();

            while (TermeActuel.Type == TokenType.OperateurMathematique &&
                  (TermeActuel.Mots == "/" || TermeActuel.Mots == "modulo" || 
                   TermeActuel.Mots == "%") || TermeActuel.Mots == "*")
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new OperateurMathematique(node, t, Level_2());
            }

            return node;
        }

        private AST? Level_2()
        {
            var node = Level_1();

            if (TermeActuel.Equals(Terme("de", TokenType.Préposition)))
            {
                ObtenirProchainTerme();

                node.AddChild(Level_1());
            }

            return node;
        }

        private AST? Level_1()
        {
            var node = Level_0();

            while (node == null && TermeActuel.Type == TokenType.OperateurMathematique && TermeActuel.Mots == "-")
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new OperateurMathematique(t, Level_1());
            }

            return node;
        }

        private AST? Level_0()
        {
            AST? node = default;

            if (TermeActuel.Type == TokenType.Sujet)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }
            else if (TermeActuel.Type == TokenType.Nombre)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }
            else if (TermeActuel.Type == TokenType.Text)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }

            return node;
        }

        public override string ToString()
        {
            return $"Parseur : {{ CurrentToken : {TermeActuel?.ToString() ?? "null" } }} ";
        }
    }
}