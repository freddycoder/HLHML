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

        public Parseur(Lexer lexer)
        {
            _lexer = lexer;
        }

        public Token GetNextToken()
        {
            CurrentToken = _lexer.GetNextToken();
            return CurrentToken;
        }

        public AST Parse(Scope scope = null)
        {
            var root = new AST(new Token("Compound", TokenType.Compound), scope ?? new Scope());

            GetNextToken();

            while (CurrentToken.Type != TokenType.None)
            {
                if (CurrentToken.Type == TokenType.Sujet || CurrentToken.Type == TokenType.Nombre)
                {
                    var token = CurrentToken;

                    var ast = AfterBeginingWithSubjectOrNumber();

                    root.AddChilds(ast.AddChildsAsFirstChild(new AST(token)));
                }
                else if (CurrentToken.Type == TokenType.Verbe)
                {
                    root.AddChilds(InitialiserVerbe());
                }
                else if (CurrentToken.Type == TokenType.Conjonction)
                {
                    root.AddChilds(InitialiserConjnction());
                }

                GetNextToken();
            }

            return root;
        }

        private AST InitialiserConjnction()
        {
            var conjonction = new Conjonction(CurrentToken);

            var isFirst = true;

            var first = GetNextToken();

            if (first.Type == TokenType.Determinant)
            {
                first = GetNextToken();
            }

            while (CurrentToken.Type != TokenType.None || CurrentToken.Type == TokenType.Ponctuation)
            {
                GetNextToken();

                if (CurrentToken.Type == TokenType.Verbe)
                {
                    conjonction.AddChilds(InitialiserVerbe());
                }

                if (isFirst)
                {
                    conjonction.Childs[0].AddChildsAsFirstChild(new AST(first));

                    isFirst = false;
                }
            }

            return conjonction;
        }

        private AST InitialiserVerbe()
        {
            if (CurrentToken.Value.Equals("Afficher", StringComparison.OrdinalIgnoreCase))
            {
                return new Afficher(CurrentToken).AddChilds(AfterVerbeAndAdjectifs());
            }
            else if (CurrentToken.Value.Equals("Vaut", StringComparison.OrdinalIgnoreCase))
            {
                return new Vaut(CurrentToken).AddChilds(AfterVerbeAndAdjectifs());
            }
            else if (CurrentToken.Value.Equals("Lire", StringComparison.OrdinalIgnoreCase))
            {
                return new Lire(CurrentToken).AddChilds(AfterVerbeAndAdjectifs());
            }
            else if (CurrentToken.Value.Equals("est", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNextAdjectif();
            }

            throw new VerbeNotFoundException(CurrentToken);
        }

        private AST ParseNextAdjectif()
        {
            AST adj = new Egual(GetNextToken());

            adj.AddChilds(AfterVerbeAndAdjectifs());

            return adj;
        }

        private AST AfterBeginingWithSubjectOrNumber()
        {
            var first = CurrentToken;

            GetNextToken();

            if (CurrentToken.Type == TokenType.OperateurMathematique)
            {
                return InialiserMathOperator();
            }
            else if (CurrentToken.Value.Equals("Vaut", StringComparison.OrdinalIgnoreCase))
            {
                return InitialiserVerbe();
            }

            throw new InvalidSentenceException($"Connot begin a sentence with token {first}");
        }

        private List<AST> AfterVerbeAndAdjectifs()
        {
            var asts = new List<AST>();

            GetNextToken();

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
                else
                {
                    GetNextToken();
                }

                if (asts.Count > 0 && asts.Last().Type == TokenType.OperateurMathematique)
                {
                    GetNextToken();
                }

            } while (CurrentToken.Type == TokenType.Nombre || CurrentToken.Type == TokenType.Sujet ||
                     CurrentToken.Type == TokenType.Text || CurrentToken.Type == TokenType.Determinant);

            return asts;
        }

        private AST AfterSubjectOrNumber()
        {
            var first = CurrentToken;

            GetNextToken();

            if (CurrentToken.Type == TokenType.OperateurMathematique)
            {
                return InialiserMathOperator().AddChildsAsFirstChild(new AST(first));
            }

            return new AST(first);
        }

        private AST InialiserMathOperator()
        {
            var ast = new MathOperator(CurrentToken);

            ast.AddChilds(new AST(GetNextToken()));

            return ast;
        }
    }
}