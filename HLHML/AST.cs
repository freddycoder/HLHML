using System;
using System.Collections.Generic;
using HLHML.Dictionnaire;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace HLHML
{
    public class ASTOp //: IOperation
    {
        public IOperation Parent => throw new NotImplementedException();

        public OperationKind Kind => throw new NotImplementedException();

        public SyntaxNode Syntax => throw new NotImplementedException();

        public ITypeSymbol Type => throw new NotImplementedException();

        public Optional<object> ConstantValue => throw new NotImplementedException();

        public IEnumerable<IOperation> Children => throw new NotImplementedException();

        public string Language => throw new NotImplementedException();

        public bool IsImplicit => throw new NotImplementedException();

        public SemanticModel SemanticModel => throw new NotImplementedException();

        public void Accept(OperationVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            throw new NotImplementedException();
        }
    }

    public class AST
    {
        private AST? _parent;
        protected readonly List<AST> _childs = new List<AST>();
        private readonly Terme _terme;
        private Scope? _scope;

        /// <exception cref="ArgumentNullException"></exception>
        public AST(Terme token)
        {
            _terme = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(Terme token, AST firstChild)
        {
            _terme = token ?? throw new ArgumentNullException(nameof(token));
            AddChild(firstChild);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(AST firstChild, Terme token, AST secondChild)
        {
            _terme = token ?? throw new ArgumentNullException(nameof(token));
            AddChild(firstChild);
            AddChild(secondChild);
        }

        public AST(Terme token, Scope? scope)
        {
            _terme = token;
            _scope = scope;
        }

        public AST AddParent(AST ast)
        {
            _parent = ast;

            _parent.AddChildsAsFirstChild(this);

            return _parent;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST AddChildsAsFirstChild(AST? ast)
        {
            if (ast == null) throw new ArgumentNullException(nameof(ast));

            ast._parent = this;
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

            ast._parent = this;

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

        public string Value => _terme.Mots;

        public TokenType Type => _terme.Type;

        public Terme Token => _terme;

        public IReadOnlyList<AST> Childs => _childs;

        public Scope Scope
        {
            get
            {
                if (_scope != null)
                {
                    return _scope;
                }
                else if (_parent != null && _parent.Scope != null)
                {
                    return _parent.Scope;
                }
                else
                {
                    throw new ScopeNotInitializeException();
                }
            }
        }

        public AST? Parent => _parent;

        public override string ToString()
        {
            return $"{_terme} Childs Count: {Childs.Count}";
        }
    }
}