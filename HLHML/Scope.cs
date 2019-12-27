using System;
using System.Collections.Generic;

namespace HLHML
{
    public class Scope
    {
        private readonly IDictionary<string, dynamic> _variables;
        private readonly Scope _parent;

        public Scope()
        {
            _variables = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);
        }

        public bool ContainsKey(string name)
        {
            var keyFounded = _variables.ContainsKey(name);

            if (!keyFounded)
            {
                if (_parent != null)
                {
                    keyFounded = _parent.ContainsKey(name);
                }
            }

            return keyFounded;
        }

        public dynamic this[string name]
        {
            get
            {
                if (ContainsKey(name))
                {
                    if (_variables.ContainsKey(name))
                    {
                        return _variables[name];
                    }
                    else if (_parent != null)
                    {
                        return _parent[name];
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
                    else if (_parent != null)
                    {
                        _parent[name] = value;
                    }
                }
                else
                {
                    _variables.Add(name, value);
                }
            }
        }
    }
}