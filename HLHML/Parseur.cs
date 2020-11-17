using HLHML.Exceptions;
using HLHML.LanguageElements;
using HLHML.LanguageElements.Adjectifs;
using HLHML.LanguageElements.Syntaxe;
using System;
using System.Collections.Generic;
using System.IO;
using static HLHML.TermeBuilder;

namespace HLHML
{
    public class Parseur
    {
        private readonly Lexer _lexer;
        private Terme TermeActuel { get; set; }
        private Scope? _actuelScope;

        private TextWriter _textWriter;
        private bool _newLine;

        private TextReader _textReader;

        public Parseur(Lexer lexer)
        {
            _lexer = lexer;
            _textWriter = Console.Out;
            _textReader = Console.In;

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

        internal void SetTextReader(TextReader textReader)
        {
            _textReader = textReader;
        }

        private void UpdateScopeReference(Scope? scope)
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
            AST ast = GeneriqueCorps(scope, true, () => TermeActuel.Type != TypeTerme.None && 
                                                        TermeActuel.Type != TypeTerme.Adverbe && 
                                                        TermeActuel.Mots.IsNot("Ensuite"));

            return ast;
        }

        private AST InitialiserConjonction()
        {
            var conjonction = new Conjonction(TermeActuel);

            ObtenirProchainTerme();

            if (TermeActuel.Type == TypeTerme.Déterminant)
            {
                ObtenirProchainTerme();
            }

            conjonction.AddChild(Expression());

            if (TermeActuel.Mots.Equals(","))
            {
                ObtenirProchainTerme();

                if (TermeActuel.Equals(Terme("Alors", TypeTerme.Adverbe)))
                {
                    ObtenirProchainTerme();
                }
            }

            conjonction.AddChild(GeneriqueCorps(new Scope(_actuelScope), 
                                                true, 
                                                GetPredicatFunction(conjonction.Terme)));
            // TODO : Ici la fonction predicat retourné par GetPredicatFunction doit être en mesure 
            // de savoir si un noeud parent est un noeud tant que
            // ceci fera passer le test ScopeIteration qui est en échec.

            if (TermeActuel.Mots.Equals("sinon", StringComparison.OrdinalIgnoreCase))
            {
                ObtenirProchainTerme();

                conjonction.AddChild(GeneriqueCorps(new Scope(_actuelScope), 
                                                    true, 
                                                    () => TermeActuel.Type != TypeTerme.None && 
                                                          TermeActuel.Type != TypeTerme.Adverbe && 
                                                          TermeActuel.Mots.IsNot("sinon")));
            }

            return conjonction;
        }

        private Func<bool> GetPredicatFunction(Terme terme)
        {
            switch (terme.Mots.ToLower())
            {
                case "tant que":
                    return () => TermeActuel.Type != TypeTerme.None &&
                                 TermeActuel.Type != TypeTerme.Adverbe &&
                                 TermeActuel.Mots.IsNot("sinon") &&
                                 TermeActuel.Mots.IsNot("ensuite");
                default:
                    return () => TermeActuel.Type != TypeTerme.None &&
                                 TermeActuel.Type != TypeTerme.Adverbe &&
                                 TermeActuel.Mots.IsNot("sinon");
            }
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
                return new Lire(TermeActuel, _textReader).AddChilds(AfterVerbeAndAdjectifs(subject));
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

            adj.AddChilds(AfterVerbeAndAdjectifs(subject));

            return adj;
        }

        private List<AST> AfterVerbeAndAdjectifs(string subject)
        {
            var asts = new List<AST>();

            ObtenirProchainTerme();

            var skipDoWhile = false;

            if (TermeActuel.Type == TypeTerme.Adverbe)
            {
                ObtenirProchainTerme();

                if (TermeActuel.Type == TypeTerme.Ponctuation)
                {
                    asts.Add(BuildDefinition(subject));

                    skipDoWhile = true;
                }
            }

            if (!skipDoWhile)
            {
                do
                {
                    if (TermeActuel.Type == TypeTerme.Nombre ||
                        TermeActuel.Type == TypeTerme.Sujet ||
                        TermeActuel.Type == TypeTerme.OperateurMathematique ||
                        TermeActuel.Type == TypeTerme.OuvertureParenthèse ||
                        TermeActuel.Type == TypeTerme.Negation)
                    {
                        asts.Add(Expression());
                    }
                    else if (TermeActuel.Type == TypeTerme.Text)
                    {
                        asts.Add(new AST(TermeActuel));

                        ObtenirProchainTerme();
                    }
                    else if (TermeActuel.Type == TypeTerme.Conjonction)
                    {
                        asts.Add(Parse(new Scope(_actuelScope)));
                    }
                    else
                    {
                        ObtenirProchainTerme();
                    }

                } while (TermeActuel.Type == TypeTerme.Nombre || TermeActuel.Type == TypeTerme.Sujet ||
                        TermeActuel.Type == TypeTerme.Text || TermeActuel.Type == TypeTerme.Déterminant ||
                        TermeActuel.Type == TypeTerme.OperateurMathematique);
            }

            return asts;
        }

        private AST BuildDefinition(string subject)
        {
            var traiteLeSujet = false;

            bool sujet_pas_traité_ou_definition_pas_terminer()
            {
                var @return = TermeActuel.Type != TypeTerme.None && TermeActuel.Type != TypeTerme.Adverbe;

                if (traiteLeSujet)
                {
                    return false;
                }

                traiteLeSujet = TermeActuel.Mots.Equals(subject, StringComparison.OrdinalIgnoreCase);

                return @return;
            }

            _actuelScope = new Scope(_actuelScope);

            var root = new AST(new Terme("Corps", TypeTerme.Corps), _actuelScope);

            ObtenirProchainTerme();

            while (sujet_pas_traité_ou_definition_pas_terminer())
            {
                if (TermeActuel.Type == TypeTerme.Sujet || TermeActuel.Type == TypeTerme.Nombre)
                {
                    root.AddChild(Expression());
                }
                else if (TermeActuel.Type == TypeTerme.Verbe)
                {
                    root.AddChild(InitialiserVerbe(subject));
                }
                else if (TermeActuel.Type == TypeTerme.Conjonction)
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

        private AST GeneriqueCorps(Scope? scope, bool skipLastAdverb, Func<bool> predicat)
        {
            UpdateScopeReference(scope);

            var root = new AST(new Terme("Corps", TypeTerme.Corps), _actuelScope);

            while (predicat.Invoke())
            {
                if (TermeActuel.Type == TypeTerme.Sujet ||
                    TermeActuel.Type == TypeTerme.Nombre ||
                    TermeActuel.Type == TypeTerme.OuvertureParenthèse ||
                    TermeActuel.Type == TypeTerme.Negation)
                {
                    root.AddChild(Expression());
                }
                else if (TermeActuel.Type == TypeTerme.Verbe)
                {
                    root.AddChild(InitialiserVerbe(""));
                }
                else if (TermeActuel.Type == TypeTerme.Conjonction)
                {
                    root.AddChild(InitialiserConjonction());
                }

                if (TermeActuel.Type != TypeTerme.Adverbe && TermeActuel.Mots.IsNot("sinon"))
                {
                    ObtenirProchainTerme();
                }
            }

            if (skipLastAdverb && TermeActuel.Type == TypeTerme.Adverbe)
            {
                ObtenirProchainTerme();
            }

            _actuelScope = _actuelScope.Parent;

            return root;
        }

        private bool _expressionEstInversser;
        private Terme? _termeInverssion;

        public AST? Expression()
        {
            var node = Level_16();

            if (_expressionEstInversser)
            {
                node = new Non(_termeInverssion, node);
                _expressionEstInversser = false;
                _termeInverssion = null;
            }

            return node;
        }

        private AST? Level_16()
        {
            var node = Level_14();

            if (TermeActuel.Type == TypeTerme.Déterminant)
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

        private AST? Level_14()
        {
            var node = Level_13();

            if (TermeActuel.Mots.Equals("ou", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Ou(node, t, Level_14());
            }

            return node;
        }

        private AST? Level_13()
        {
            var node = Level_10();

            if (TermeActuel.Mots.Equals("et", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Et(node, t, Level_13());
            }

            return node;
        }

        private AST? Level_10()
        {
            var node = Level_9();

            return node;
        }

        private AST? Level_9()
        {
            var node = Level_8();

            if (TermeActuel.Type == TypeTerme.EgalÀ)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Egal(node, t, Level_9());
            }

            return node;
        }

        private AST? Level_8()
        {
            var node = Level_7();

            if (TermeActuel.Type == TypeTerme.PlusPetitQue)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new PlusPetit(node, t, Level_8());
            }
            else if (TermeActuel.Type == TypeTerme.PlusGrandQue)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new PlusGrand(node, t, Level_8());
            }
            if (TermeActuel.Type == TypeTerme.PlusPetitOuEgalÀ)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new PlusPetitOuEgal(node, t, Level_8());
            }
            else if (TermeActuel.Type == TypeTerme.PlusGrandOuEgalÀ)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new PlusGrandOuEgal(node, t, Level_8());
            }

            return node;
        }

        private AST? Level_7()
        {
            var node = Level_4();

            if (TermeActuel.Mots.Equals("n'", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                _expressionEstInversser = true;
                _termeInverssion = t;
            }

            if (TermeActuel.Mots.Equals("est"))
            {
                ObtenirProchainTerme();

                if (TermeActuel.Mots.Equals("pas", StringComparison.OrdinalIgnoreCase))
                {
                    var t = TermeActuel;
                    ObtenirProchainTerme();

                    if (_expressionEstInversser == false)
                    {
                        _expressionEstInversser = true;
                        _termeInverssion = t;
                    }
                }
            }

            return node;
        }

        private AST? Level_4()
        {
            var node = Level_3();

            while (TermeActuel.Type == TypeTerme.OperateurMathematique &&
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

            if (TermeActuel.Type == TypeTerme.OperateurMathematique &&
                  (TermeActuel.Mots == "/" || TermeActuel.Mots == "modulo" ||
                   TermeActuel.Mots == "%") || TermeActuel.Mots == "*")
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new OperateurMathematique(node, t, Level_3());
            }
            else if (TermeActuel.Mots.Equals("non", StringComparison.OrdinalIgnoreCase))
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Non(t, Level_3());
            }

            return node;
        }

        private AST? Level_2()
        {
            var node = Level_1();

            if (TermeActuel.Equals(Terme("de", TypeTerme.Préposition)))
            {
                ObtenirProchainTerme();

                if (node != null)
                {
                    node.AddChild(Level_1());
                }
                else
                {
                    // Ça ou throw new Exception() ...
                    // avant, il n'y avait pas le if et node.AddChild était directement appeller alors que node pouvait être null.
                    node = Level_1();
                }
            }

            return node;
        }

        private AST? Level_1()
        {
            var node = Level_0();

            while (node == null && TermeActuel.Type == TypeTerme.OperateurMathematique && TermeActuel.Mots == "-")
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

            if (TermeActuel.Type == TypeTerme.Sujet)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }
            else if (TermeActuel.Type == TypeTerme.Nombre)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }
            else if (TermeActuel.Type == TypeTerme.Text)
            {
                node = new AST(TermeActuel);
                ObtenirProchainTerme();
            }
            else if (TermeActuel.Type == TypeTerme.OuvertureParenthèse)
            {
                var t = TermeActuel;
                ObtenirProchainTerme();
                node = new Parenthese(t, Expression());

                if (TermeActuel.Type == TypeTerme.FermetureParenthèse)
                {
                    ObtenirProchainTerme();
                }
                else
                {
                    throw new NonClosingParenthesisException();
                }
            }

            return node;
        }

        public override string ToString()
        {
            return $"Parseur : {{ CurrentToken : {TermeActuel?.ToString() ?? "null" } }} ";
        }
    }
}