using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace Test
{
    public class PreprocessorSymbols
    {
        bool _noisy = false;

        public Dictionary<
            string,  // name of the macro for fast lookup.
            Tuple<
                List<string>, // a list of parameter names to the macro.
                CPlusPlus14Parser.Replacement_listContext, // value of macro.
                CommonTokenStream, // token stream where the macro is defined.
                string>> _map; // file name where the macro is defined.

        public PreprocessorSymbols(PreprocessorSymbols copy)
        {
            _map = new Dictionary<string, Tuple<List<string>, CPlusPlus14Parser.Replacement_listContext, CommonTokenStream, string>>(copy._map);
        }

        public PreprocessorSymbols()
        {
            _map = new Dictionary<string, Tuple<List<string>, CPlusPlus14Parser.Replacement_listContext, CommonTokenStream, string>>();
        }

        public void Add(string name,
            List<string> ids,
            CPlusPlus14Parser.Replacement_listContext repl,
            CommonTokenStream ts,
            string fn)
        {
            if (_noisy) System.Console.Error.WriteLine("Defining " + name);
            _map[name] = new Tuple<List<string>, CPlusPlus14Parser.Replacement_listContext, CommonTokenStream, string>(ids, repl, ts, fn);
        }

        public void Delete(string name)
        {
            if (_noisy) System.Console.Error.WriteLine("Undefining " + name);
            this._map.Remove(name);
        }

        public (List<string>,
            CPlusPlus14Parser.Replacement_listContext,
            CommonTokenStream,
            string)
            Find(string macro_name)
        {
            if (_noisy) System.Console.Error.WriteLine("Find " + macro_name);
            if (_map.TryGetValue(macro_name, out Tuple<List<string>, CPlusPlus14Parser.Replacement_listContext, CommonTokenStream, string> entry))
            {
                if (_noisy) System.Console.Error.WriteLine("Yes!");
                var parameters = entry.Item1;
                var macro_value = entry.Item2;
                var stream = entry.Item3;
                var fn = entry.Item4;
                return (parameters, macro_value, stream, fn);
            }
            else
            {
                if (_noisy) System.Console.Error.WriteLine("Nope!");
                return (null, null, null, null);
            }
        }

        public bool Find(string macro_name,
            out List<string> parameters,
            out CPlusPlus14Parser.Replacement_listContext macro_value,
            out CommonTokenStream stream,
            out string fn)
        {
            if (_noisy) System.Console.Error.WriteLine("Find " + macro_name);
            if (_map.TryGetValue(macro_name, out Tuple<List<string>,
                CPlusPlus14Parser.Replacement_listContext,
                CommonTokenStream,
                string> t))
            {
                if (_noisy) System.Console.Error.WriteLine("Yes!");
                parameters = t.Item1;
                macro_value = t.Item2;
                stream = t.Item3;
                fn = t.Item4;
                return true;
            }
            else
            {
                parameters = null;
                macro_value = null;
                stream = null;
                fn = null;
                if (_noisy) System.Console.Error.WriteLine("Nope!");
                return false;
            }
        }

        public bool IsDefined(string macro_name)
        {
            if (_noisy) System.Console.Error.WriteLine("IsDefined " + macro_name);
            var result = _map.TryGetValue(macro_name, out Tuple<List<string>,
                CPlusPlus14Parser.Replacement_listContext,
                CommonTokenStream,
                string> t);
            if (_noisy) System.Console.Error.WriteLine("returning " + result);
            return result;
        }
    }
}
