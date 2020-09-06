using System;
using System.Collections.Generic;

namespace HLHML
{
    public interface IReadOnlyScope
    {
        dynamic? this[string name] { get; }

        bool ContainsKey(string name);
    }

    public class Scope : IReadOnlyScope
    {
        private readonly IDictionary<string, dynamic?> _variables;
        public Scope? Parent { get; }

        public Scope()
        {
            _variables = new Dictionary<string, dynamic?>(StringComparer.OrdinalIgnoreCase);
        }

        public Scope(Scope parent)
        {
            Parent = parent;
            _variables = new Dictionary<string, dynamic?>(StringComparer.OrdinalIgnoreCase);
        }

        public bool ContainsKey(string name)
        {
            var keyFounded = _variables.ContainsKey(name);

            if (!keyFounded)
            {
                if (Parent != null)
                {
                    keyFounded = Parent.ContainsKey(name);
                }
            }

            return keyFounded;
        }

        public dynamic? this[string name]
        {
            get
            {
                if (ContainsKey(name))
                {
                    if (_variables.ContainsKey(name))
                    {
                        return _variables[name];
                    }
                    else if (Parent != null)
                    {
                        return Parent[name];
                    }
                }

                _variables.Add(name, default);

                return _variables[name];
            }
            set
            {
                if (ContainsKey(name))
                {
                    if (_variables.ContainsKey(name))
                    {
                        _variables[name] = value;
                    }
                    else if (Parent != null)
                    {
                        Parent[name] = value;
                    }
                }
                else
                {
                    _variables.Add(name, value);
                }
            }
        }

        public override string ToString()
        {
            var s = this;

            int child = 0;

            for (; s.Parent != null; child++)
            {
                s = s.Parent;
            };

            return $"Scope {child} : VarCount {_variables.Count}";
        }
    }
}