using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace Test
{
    public class PreprocessorSymbols
    {
        bool debug = false;

        public Dictionary<
            string,  // name of the macro for fast lookup.
            Tuple<
                List<string>, // a list of parameter names to the macro.
                Cpp14Parser.Replacement_listContext, // value of macro.
                CommonTokenStream, // token stream where the macro is defined.
                string>> map; // file name where the macro is defined.
        public CommonTokenStream Tokens { get; private set; }

        public PreprocessorSymbols(PreprocessorSymbols copy)
        {
            map = new Dictionary<string, Tuple<List<string>, Cpp14Parser.Replacement_listContext, CommonTokenStream, string>>(copy.map);
        }

        public PreprocessorSymbols()
        {
            map = new Dictionary<string, Tuple<List<string>, Cpp14Parser.Replacement_listContext, CommonTokenStream, string>>();
        }

        public void Add(string name,
            List<string> ids,
            Cpp14Parser.Replacement_listContext repl,
            CommonTokenStream ts,
            string fn)
        {
            if (debug) System.Console.Error.WriteLine("Defining " + name);
            map[name] = new Tuple<List<string>, Cpp14Parser.Replacement_listContext, CommonTokenStream, string>(ids, repl, ts, fn);
        }

        public void Delete(string name)
        {
            if (debug) System.Console.Error.WriteLine("Undefining " + name);
            this.map.Remove(name);
        }

        public (List<string>,
            Cpp14Parser.Replacement_listContext,
            CommonTokenStream,
            string)
            Find(string macro_name)
        {
            if (debug) System.Console.Error.WriteLine("Find " + macro_name);
            if (map.TryGetValue(macro_name, out Tuple<List<string>, Cpp14Parser.Replacement_listContext, CommonTokenStream, string> entry))
            {
                if (debug) System.Console.Error.WriteLine("Yes!");
                var parameters = entry.Item1;
                var macro_value = entry.Item2;
                var stream = entry.Item3;
                var fn = entry.Item4;
                return (parameters, macro_value, stream, fn);
            }
            else
            {
                if (debug) System.Console.Error.WriteLine("Nope!");
                return (null, null, null, null);
            }
        }

        public bool Find(string macro_name,
            out List<string> parameters,
            out Cpp14Parser.Replacement_listContext macro_value,
            out CommonTokenStream stream,
            out string fn)
        {
            if (debug) System.Console.Error.WriteLine("Find " + macro_name);
            if (map.TryGetValue(macro_name, out Tuple<List<string>,
                Cpp14Parser.Replacement_listContext,
                CommonTokenStream,
                string> t))
            {
                if (debug) System.Console.Error.WriteLine("Yes!");
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
                if (debug) System.Console.Error.WriteLine("Nope!");
                return false;
            }
        }

        public bool IsDefined(string macro_name)
        {
            if (debug) System.Console.Error.WriteLine("IsDefined " + macro_name);
            var result = map.TryGetValue(macro_name, out Tuple<List<string>,
                Cpp14Parser.Replacement_listContext,
                CommonTokenStream,
                string> t);
            if (debug) System.Console.Error.WriteLine("returning " + result);
            return result;
        }
    }

}
