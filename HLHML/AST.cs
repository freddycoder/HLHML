using System;
using System.Collections.Generic;
using HLHML.Dictionnaire;

namespace HLHML
{
    public class AST
    {
        protected readonly List<AST> _childs = new List<AST>();
        private Scope? _scope;

        /// <exception cref="ArgumentNullException"></exception>
        public AST(Terme? token)
        {
            Terme = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(Terme token, AST? firstChild)
        {
            Terme = token ?? throw new ArgumentNullException(nameof(token));

            AddChild(firstChild);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(AST firstChild, Terme token, AST secondChild)
        {
            Terme = token ?? throw new ArgumentNullException(nameof(token));
            AddChild(firstChild);
            AddChild(secondChild);
        }

        public AST(Terme token, Scope? scope)
        {
            Terme = token;
            _scope = scope;
        }

        public AST AddParent(AST ast)
        {
            Parent = ast;

            Parent.AddChildsAsFirstChild(this);

            return Parent;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST AddChildsAsFirstChild(AST? ast)
        {
            if (ast == null) throw new ArgumentNullException(nameof(ast));

            ast.Parent = this;
            ast._scope = this._scope;

            _childs.Insert(0, ast);

            return this;
        }

        /// <summary>
        /// Ajout le noeud à la liste des noeuds enfants. La scope de ce noeud enfant sera initialiser
        /// à la référence du scope du noeud parent, saut si celle-ci à déjà été initialisé.
        /// </summary>
        /// <exception cref="ArgumentNullException">Si le paramètres est nul</exception>
        /// <param name="ast">Le noeud enfant à ajouter.</param>
        /// <returns>Le noeud courrant</returns>
        public AST AddChild(AST? ast)
        {
            if (ast == default) throw new ArgumentNullException(nameof(ast), $"ast value is : {Value}");

            ast.Parent = this;

            if (ast._scope == null)
            {
                ast._scope = this._scope;
            }
            
            _childs.Add(ast);

            return this;
        }

        public AST AddChilds(List<AST> asts)
        {
            foreach (var child in asts)
            {
                AddChild(child);
            }

            return this;
        }

        public string Value => Terme.Mots;

        public TypeTerme Type => Terme.Type;

        public Terme Terme { get; }

        public IReadOnlyList<AST> Childs => _childs;

        public Scope Scope
        {
            get
            {
                if (_scope != null)
                {
                    return _scope;
                }
                else if (Parent != null && Parent.Scope != null)
                {
                    return Parent.Scope;
                }
                else
                {
                    throw new ScopeNotInitializeException();
                }
            }
        }

        public AST? Parent { get; private set; }

        public override string ToString()
        {
            return $"{Terme} Childs Count: {Childs.Count}";
        }
    }
}