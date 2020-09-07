using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using System;
using System.Collections.Generic;
using System.IO;
using static HLHML.TokenBuilder;

namespace HLHML
{
    public class Parseur
    {
        private readonly Lexer _lexer;
        private Token CurrentToken { get; set; }
        private Scope? _actuelScope;

        private TextWriter _textWriter;
        private bool _newLine;

        public Parseur(Lexer lexer)
        {
            _lexer = lexer;
            _textWriter = Console.Out;

            CurrentToken = GetNextToken();
        }

        private Token GetNextToken()
        {
            CurrentToken = _lexer.GetNextToken();
            return CurrentToken;
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
            return GeneriqueCompound(scope, () => CurrentToken.Type != TokenType.None && CurrentToken.Type != TokenType.Adverbe);
        }

        private AST InitialiserConjonction()
        {
            var conjonction = new Conjonction(CurrentToken);

            var isFirst = true;

            var predicatIsNegated = false;

            GetNextToken();

            if (CurrentToken.Type == TokenType.Determinant)
            {
                GetNextToken();
            }

            var first = Expression();

            while ((CurrentToken.Type != TokenType.None ||
                   CurrentToken.Type == TokenType.Ponctuation) &&
                   CurrentToken.Type != TokenType.Adverbe &&
                   conjonction.Childs.Count < 3)
            {
                if (CurrentToken.Type == TokenType.Ponctuation)
                {
                    GetNextToken();

                    //if (CurrentToken.Equals(Token("alors", TokenType.Adverbe)))
                    //{
                    //    GetNextToken();
                    //}
                }
                if (CurrentToken.Equals(new Token("sinon", TokenType.Conjonction)))
                {
                    GetNextToken();
                }

                if (CurrentToken.Type == TokenType.Negation)
                {
                    predicatIsNegated = true;
                }
                else if (conjonction.Value.Equals("tant que", StringComparison.OrdinalIgnoreCase) &&
                         conjonction.Childs.Count == 1)
                {
                    conjonction.AddChild(Parse(new Scope(_actuelScope)));

                    break;
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    conjonction.AddChild(InitialiserVerbe(first.Value));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    conjonction.AddChild(InitialiserConjonction());
                }
                //else if (CurrentToken.Type == TokenType.Sujet)
                //{
                //    conjonction.AddChild(Expression());
                //}

                if (isFirst && CurrentToken.Type != TokenType.Negation)
                {
                    conjonction.Childs[0].AddChildsAsFirstChild(first);

                    isFirst = false;
                }
                else if (CurrentToken.Type == TokenType.Negation)
                {
                    GetNextToken();
                }
            }

            conjonction.PredicatIsNegated = predicatIsNegated;

            return conjonction;
        }

        private AST InitialiserVerbe(string subject)
        {
            if (CurrentToken.Value.Equals("Afficher", StringComparison.OrdinalIgnoreCase))
            {
                var ast = new Afficher(CurrentToken);

                ast.SetTextWriter(_textWriter, _newLine);

                return ast.AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (CurrentToken.Value.Equals("Lire", StringComparison.OrdinalIgnoreCase))
            {
                return new Lire(CurrentToken).AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (CurrentToken.Value.Equals("est", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNextAdjectif(subject);
            }
            else if (CurrentToken.Value.Equals("variant", StringComparison.OrdinalIgnoreCase))
            {
                return ParseVariant(subject);
            }
            else if (CurrentToken.Value.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                return BuildDefinition(subject);
            }

            throw new VerbeNotFoundException(CurrentToken);
        }

        private AST ParseVariant(string subject)
        {
            return new AST(CurrentToken).AddChild(Expression());
        }

        private AST ParseNextAdjectif(string subject)
        {
            GetNextToken();

            AST? adj;
            if (CurrentToken.Value.Equals("plus petit que", StringComparison.OrdinalIgnoreCase))
            {
                adj = new PlusPetit(CurrentToken);
            }
            else if (CurrentToken.Value.Equals("plus grand que", StringComparison.OrdinalIgnoreCase))
            {
                adj = new PlusGrand(CurrentToken);
            }
            else
            {
                adj = new Egal(CurrentToken);
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

            GetNextToken();

            var skipDoWhile = false;

            if (CurrentToken.Type == TokenType.Adverbe)
            {
                GetNextToken();

                if (CurrentToken.Type == TokenType.Ponctuation)
                {
                    asts.Add(BuildDefinition(subject));

                    skipDoWhile = true;
                }
            }

            if (!skipDoWhile)
            {
                do
                {
                    if (CurrentToken.Type == TokenType.Nombre || 
                        CurrentToken.Type == TokenType.Sujet || 
                        CurrentToken.Type == TokenType.OperateurMathematique)
                    {
                        asts.Add(Expression());
                    }
                    else if (CurrentToken.Type == TokenType.Text)
                    {
                        asts.Add(new AST(CurrentToken));

                        GetNextToken();
                    }
                    else if (CurrentToken.Type == TokenType.Conjonction)
                    {
                        asts.Add(Parse(new Scope(_actuelScope)));
                    }
                    else
                    {
                        GetNextToken();
                    }

                } while (CurrentToken.Type == TokenType.Nombre || CurrentToken.Type == TokenType.Sujet ||
                        CurrentToken.Type == TokenType.Text || CurrentToken.Type == TokenType.Determinant ||
                        CurrentToken.Type == TokenType.OperateurMathematique);
            }

            return asts;
        }

        private AST BuildDefinition(string subject)
        {
            var traiteLeSujet = false;

            bool sujet_pas_traité_ou_definition_pas_terminer()
            {
                var @return = CurrentToken.Type != TokenType.None && CurrentToken.Type != TokenType.Adverbe;

                if (traiteLeSujet)
                {
                    return false;
                }

                traiteLeSujet = CurrentToken.Value.Equals(subject, StringComparison.OrdinalIgnoreCase);

                return @return;
            }

            _actuelScope = new Scope(_actuelScope);

            var root = new AST(new Token("Compound", TokenType.Compound), _actuelScope);

            GetNextToken();

            while (sujet_pas_traité_ou_definition_pas_terminer())
            {
                if (CurrentToken.Type == TokenType.Sujet || CurrentToken.Type == TokenType.Nombre)
                {
                    root.AddChild(Expression());
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    root.AddChild(InitialiserVerbe(subject));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    root.AddChild(InitialiserConjonction());
                }

                if (sujet_pas_traité_ou_definition_pas_terminer())
                {
                    GetNextToken();
                }
            }

            _actuelScope = _actuelScope.Parent;

            return root;
        }

        private AST GeneriqueCompound(Scope? scope, Func<bool> predicat)
        {
            UpdateScopeReference(scope);

            var root = new AST(new Token("Compound", TokenType.Compound), _actuelScope);

            while (predicat.Invoke())
            {
                if (CurrentToken.Type == TokenType.Sujet || CurrentToken.Type == TokenType.Nombre)
                {
                    root.AddChild(Expression());
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    root.AddChild(InitialiserVerbe(""));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    root.AddChild(InitialiserConjonction());
                }

                if (CurrentToken.Type != TokenType.Adverbe)
                {
                    GetNextToken();
                }
            }

            if (CurrentToken.Type == TokenType.Adverbe)
            {
                GetNextToken();
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

            if (CurrentToken.Type == TokenType.Determinant)
            {
                GetNextToken();
            }

            if (CurrentToken.Value.Equals("Vaut", StringComparison.OrdinalIgnoreCase))
            {
                var t = CurrentToken;
                GetNextToken();
                node = new Vaut(node, t, Expression());
            }
            else if (CurrentToken.Value.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                var t = CurrentToken;
                GetNextToken();
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

            while (CurrentToken.Type == TokenType.OperateurMathematique && 
                  (CurrentToken.Value == "+" || CurrentToken.Value == "-"))
            {
                var t = CurrentToken;
                GetNextToken();
                node = new OperateurMathematique(node, t, Level_3());
            }

            return node;
        }

        private AST? Level_3()
        {
            var node = Level_2();

            while (CurrentToken.Type == TokenType.OperateurMathematique &&
                  (CurrentToken.Value == "/" || CurrentToken.Value == "modulo" || 
                   CurrentToken.Value == "%") || CurrentToken.Value == "*")
            {
                var t = CurrentToken;
                GetNextToken();
                node = new OperateurMathematique(node, t, Level_2());
            }

            return node;
        }

        private AST? Level_2()
        {
            var node = Level_1();

            //if (CurrentToken.Equals(Token("de", TokenType.Preposition)))
            //{
            //    GetNextToken();

            //    node.AddChild(Level_1());
            //}

            return node;
        }

        private AST? Level_1()
        {
            var node = Level_0();

            while (node == null && CurrentToken.Type == TokenType.OperateurMathematique && CurrentToken.Value == "-")
            {
                var t = CurrentToken;
                GetNextToken();
                node = new OperateurMathematique(t, Level_1());
            }

            return node;
        }

        private AST? Level_0()
        {
            AST? node = default;

            if (CurrentToken.Type == TokenType.Sujet)
            {
                node = new AST(CurrentToken);
                GetNextToken();
            }
            else if (CurrentToken.Type == TokenType.Nombre)
            {
                node = new AST(CurrentToken);
                GetNextToken();
            }
            else if (CurrentToken.Type == TokenType.Text)
            {
                node = new AST(CurrentToken);
                GetNextToken();
            }

            return node;
        }

        public override string ToString()
        {
            return $"Parseur : {{ CurrentToken : {CurrentToken?.ToString() ?? "null" } }} ";
        }
    }
}