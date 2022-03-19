using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class LoxEnvironment
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private readonly LoxEnvironment? _enclosing;
        public LoxEnvironment()
        {
        }
        public LoxEnvironment(LoxEnvironment enclosing)
        {
            this._enclosing = enclosing;
        }
        public void Define(string name, object val)
        {
            // allow redefining variables
            if (_values.ContainsKey(name))
            {
                _values[name] = val;
            } else
            {
                _values.Add(name, val);
            }
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            if (_enclosing != null) return _enclosing.Get(name);
            
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object val)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = val;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, val);
                return;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");

        }
    }
}
