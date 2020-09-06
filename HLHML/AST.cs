using System;
using System.Collections.Generic;
using System.IO;

namespace HLHML
{
    public class AST
    {
        private AST? _parent;
        protected readonly List<AST> _childs = new List<AST>();
        private readonly Token _token;
        private Scope? _scope;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="token"></param>
        public AST(Token token)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(Token token, AST firstChild)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
            AddChilds(firstChild);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public AST(AST firstChild, Token token, AST secondChild)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
            AddChilds(firstChild);
            AddChilds(secondChild);
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

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="ast"></param>
        /// <returns></returns>
        public AST AddChilds(AST ast)
        {
            if (ast == default) throw new ArgumentNullException(nameof(ast), $"ast value is : {Value}");

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

        public string Value => _token.Value;

        public TokenType Type => _token.Type;

        public Token Token => _token;

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
            return $"{_token} Childs Count: {Childs.Count}";
        }
    }
}