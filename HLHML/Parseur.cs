using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HLHML
{
    public class Parseur
    {
        private readonly Lexer _lexer;
        private Token CurrentToken { get; set; }
        private Scope _parentScope;

        public Parseur(Lexer lexer)
        {
            _lexer = lexer;
        }

        private Token GetNextToken()
        {
            CurrentToken = _lexer.GetNextToken();
            return CurrentToken;
        }

        private void UpdateScopeReference(Scope scope)
        {
            if (scope == null && _parentScope == null)
            {
                _parentScope = new Scope();
            }
            else if (scope != null && _parentScope == null)
            {
                _parentScope = scope;
            }
            else if (scope == null && _parentScope != null)
            {
                _parentScope = new Scope(_parentScope);
            }
            else if (scope != null && _parentScope != null)
            {
                if (scope.Parent != _parentScope)
                {
                    throw new InvalidScopeException("Scopes does not reference each others properly");
                }

                _parentScope = scope;
            }
        }

        public AST Parse(Scope scope = null)
        {
            return GeneriqueCompound(scope, () => CurrentToken.Type != TokenType.None && CurrentToken.Type != TokenType.Adverbe);
        }

        private AST InitialiserConjonction()
        {
            var conjonction = new Conjonction(CurrentToken);

            var isFirst = true;

            var predicatIsNegated = false;

            var first = GetNextToken();

            if (first.Type == TokenType.Determinant)
            {
                first = GetNextToken();
            }

            while ((CurrentToken.Type != TokenType.None ||
                   CurrentToken.Type == TokenType.Ponctuation) &&
                   CurrentToken.Type != TokenType.Adverbe)
            {
                if (!(conjonction.Value.Equals("tant que", StringComparison.OrdinalIgnoreCase) &&
                         conjonction.Childs.Count == 1))
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
                    conjonction.AddChilds(Parse(new Scope(_parentScope)));

                    break;
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    conjonction.AddChilds(InitialiserVerbe(first.Value));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    conjonction.AddChilds(InitialiserConjonction());
                }

                if (isFirst && CurrentToken.Type != TokenType.Negation)
                {
                    conjonction.Childs[0].AddChildsAsFirstChild(new AST(first));

                    isFirst = false;
                }
            }

            conjonction.PredicatIsNegated = predicatIsNegated;

            return conjonction;
        }

        private AST InitialiserVerbe(string subject)
        {
            if (CurrentToken.Value.Equals("Afficher", StringComparison.OrdinalIgnoreCase))
            {
                return new Afficher(CurrentToken).AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (CurrentToken.Value.Equals("Vaut", StringComparison.OrdinalIgnoreCase) ||
                     CurrentToken.Value.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                return new Vaut(CurrentToken).AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (CurrentToken.Value.Equals("Lire", StringComparison.OrdinalIgnoreCase))
            {
                return new Lire(CurrentToken).AddChilds(AfterVerbeAndAdjectifs(subject));
            }
            else if (CurrentToken.Value.Equals("est", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNextAdjectif(subject);
            }

            throw new VerbeNotFoundException(CurrentToken);
        }

        private AST ParseNextAdjectif(string subject)
        {
            AST adj;

            GetNextToken();

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

            adj.AddChilds(AfterVerbeAndAdjectifs(subject));

            return adj;
        }

        private AST AfterBeginingWithSubjectOrNumber()
        {
            var first = CurrentToken;

            GetNextToken();

            if (CurrentToken.Type == TokenType.Determinant)
            {
                GetNextToken();
            }

            if (CurrentToken.Type == TokenType.OperateurMathematique)
            {
                return InialiserMathOperator();
            }
            else if (CurrentToken.Value.Equals("Vaut", StringComparison.OrdinalIgnoreCase) ||
                     CurrentToken.Value.Equals("définit", StringComparison.OrdinalIgnoreCase))
            {
                return InitialiserVerbe(first.Value);
            }

            throw new InvalidSentenceException($"Cannot begin a sentence with token {first} and {CurrentToken}");
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
                    if (CurrentToken.Type == TokenType.Nombre || CurrentToken.Type == TokenType.Sujet)
                    {
                        asts.Add(AfterSubjectOrNumber());
                    }
                    else if (CurrentToken.Type == TokenType.Text)
                    {
                        asts.Add(new AST(CurrentToken));

                        GetNextToken();
                    }
                    else if (CurrentToken.Type == TokenType.Conjonction)
                    {
                        asts.Add(Parse(new Scope(_parentScope)));
                    }
                    else
                    {
                        GetNextToken();
                    }

                } while (CurrentToken.Type == TokenType.Nombre || CurrentToken.Type == TokenType.Sujet ||
                     CurrentToken.Type == TokenType.Text || CurrentToken.Type == TokenType.Determinant);
            }

            return asts;
        }

        private AST BuildDefinition(string subject)
        {
            var traiteLeSujet = false;

            Func<bool> predicat = () =>
            {
                var @return = CurrentToken.Type != TokenType.None && CurrentToken.Type != TokenType.Adverbe;

                if (traiteLeSujet)
                {
                    return false;
                }

                traiteLeSujet = CurrentToken.Value.Equals(subject, StringComparison.OrdinalIgnoreCase);

                return @return;
            };

            UpdateScopeReference(new Scope(_parentScope));

            var root = new AST(new Token("Compound", TokenType.Compound), _parentScope);

            GetNextToken();

            while (predicat.Invoke())
            {
                if (CurrentToken.Type == TokenType.Sujet || CurrentToken.Type == TokenType.Nombre)
                {
                    var token = CurrentToken;

                    var ast = AfterBeginingWithSubjectOrNumber();

                    root.AddChilds(ast.AddChildsAsFirstChild(new AST(token)));
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    root.AddChilds(InitialiserVerbe(""));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    root.AddChilds(InitialiserConjonction());
                }

                if (predicat.Invoke())
                {
                    GetNextToken();
                }
            }

            _parentScope = _parentScope.Parent;

            return root;
        }

        private AST GeneriqueCompound(Scope scope, Func<bool> predicat)
        {
            UpdateScopeReference(scope);

            var root = new AST(new Token("Compound", TokenType.Compound), _parentScope);

            GetNextToken();

            while (predicat.Invoke())
            {
                if (CurrentToken.Type == TokenType.Sujet || CurrentToken.Type == TokenType.Nombre)
                {
                    var token = CurrentToken;

                    var ast = AfterBeginingWithSubjectOrNumber();

                    root.AddChilds(ast.AddChildsAsFirstChild(new AST(token)));
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    root.AddChilds(InitialiserVerbe(""));
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    root.AddChilds(InitialiserConjonction());
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

            _parentScope = _parentScope.Parent;

            return root;
        }

        private AST AfterSubjectOrNumber()
        {
            var first = CurrentToken;

            var ast = new AST(first);

            GetNextToken();

            while (CurrentToken.Type == TokenType.OperateurMathematique)
            {
                ast = InialiserMathOperator().AddChildsAsFirstChild(ast);

                GetNextToken();
            }

            return ast;
        }

        private AST InialiserMathOperator()
        {
            AST ast = new MathOperator(CurrentToken);

            ast.AddChilds(new AST(GetNextToken()));

            return ast;
        }
    }
}