using System;
using System.Collections.Generic;

namespace HLHML
{
    public class AST
    {
        private AST _parent;
        protected readonly List<AST> _childs = new List<AST>();
        private readonly Token _token;
        private Scope _scope;

        public AST(Token token)
        {
            _token = token;
        }

        public AST(Token token, Scope scope)
        {
            _token = token;
            _scope = scope;
        }

        public AST AddParent(AST ast)
        {
            _parent = ast;

            _parent.AddChildsAsFirstChild(this);

            return _parent;
        }

        public AST AddChildsAsFirstChild(AST ast)
        {
            ast._parent = this;
            ast._scope = this._scope;

            _childs.Insert(0, ast);

            return this;
        }

        public AST AddChilds(AST ast)
        {
            ast._parent = this;
            ast._scope = this._scope;

            _childs.Add(ast);

            return this;
        }

        public AST AddChilds(List<AST> asts)
        {
            foreach (var child in asts)
            {
                AddChilds(child);
            }

            return this;
        }

        public virtual string Value => _token.Value;

        public TokenType Type => _token.Type;

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

        public AST Parent => _parent;

        public override string ToString()
        {
            return $"{_token} Childs Count: {Childs.Count}";
        }
    }
}