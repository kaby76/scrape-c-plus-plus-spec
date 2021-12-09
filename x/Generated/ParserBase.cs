using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

public abstract class ParserBase : Parser
{
    public bool _noisy { get; set; } = false;
    static DateTime _last_time = DateTime.Now;
    ITokenStream _input;
    Lexer lexer;
    public string PreprocessedText { get; private set; }
    public ITokenStream PreprocessedStream { get; private set; }

    protected ParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput)
        : base(input, output, errorOutput)
    {
        _input = input;
    }

    public IParseTree start()
    {
        // Initialize preprocessor with predefines.
        var (init_table, locations) = InitPreprocessor();
        // Create preprocessor.
        var input = this.TokenStream;
        var src = input.TokenSource;
        ICharStream stream = src.InputStream;
        var fn = stream.SourceName;
        // Implement backslash-newlines as per Section 2.2, paragraph #2.
        var strg = stream.ToString();
        strg = strg.Replace("\\\r\n", "");
        strg = strg.Replace("\\\n\r", "");
        strg = strg.Replace("\\\n", "");
        strg = strg.Replace("\\\r", "");
        var str = CharStreams.fromString(strg);
        //var sr = new StreamReader(stream);
        if (_noisy)
        {
            System.Console.Error.WriteLine("Input:");
            System.Console.Error.WriteLine(strg);
        }
        var tokens = _input as CommonTokenStream;
        // tokens.TokenSource;
        lexer = tokens.TokenSource as Lexer;
        lexer.PushMode(CPlusPlus14Lexer.PP);
        var parser = new CPlusPlus14Parser(tokens);
        var listener_lexer = new ErrorListener<int>(false);
        var listener_parser = new ErrorListener<IToken>(false);
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        var tree = parser.preprocessing_file();
        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Test.Preprocessor(tokens, locations);
        visitor._noisy = this._noisy;
        visitor._preprocessor_symbols = new Test.PreprocessorSymbols(init_table);
        visitor._current_file_name = fn;
        if (File.Exists(SourceName))
        {
            visitor._probe_locations.Insert(0, Path.GetDirectoryName(SourceName));
        }
        if (_noisy)
        {
            var sb = Test.TreeOutput.OutputTree(tree, lexer, parser, tokens);
            System.Console.Error.WriteLine(sb.ToString());
        }
        visitor.Visit(tree);
        var real_input = visitor._sb.ToString();
        if (_noisy)
        {
            System.Console.Error.WriteLine("FINISHED PREPROCESSING ENTIRE INPUT.");
            System.Console.Error.WriteLine(real_input);
        }
        lexer.PopMode();
        lexer.Reset();
        var new_str = CharStreams.fromString(real_input);
        lexer.SetInputStream(new_str);
        this.TokenStream = new CommonTokenStream(lexer);
        IParseTree result = (this as CPlusPlus14Parser).translation_unit();
        return result;
    }

    (Test.PreprocessorSymbols, List<string>) InitPreprocessor()
    {
        List<string> locations;
        string init_header;
        if (false)
        {
            var clang_locations = new List<string>()
            {
                "/usr/include/c++/9",
                "/usr/include/x86_64-linux-gnu/c++/9",
                "/usr/include/c++/9/backward",
                "/usr/local/include",
                "/usr/lib/llvm-10/lib/clang/10.0.0/include",
                "/usr/include/x86_64-linux-gnu",
                "/usr/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/x86_64-pc-msys",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/backward",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include-fixed",
                "/usr/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/../../../../lib/../include/w32api",
                "/home/ken/qtbase/include",
                "/home/ken/qtbase",
            };
            var clang_init_header = "clang++-init.h";
            locations = clang_locations;
            init_header = clang_init_header;
        }
        else
        {
            var gcc_locations = new List<string>() {
                "/usr/include/c++/9",
                "/usr/include/x86_64-linux-gnu/c++/9",
                "/usr/include/c++/9/backward",
                "/usr/lib/gcc/x86_64-linux-gnu/9/include",
                "/usr/local/include",
                "/usr/include/x86_64-linux-gnu",
                "/usr/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/x86_64-pc-msys",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/backward",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include-fixed",
                "/home/ken/qtbase/include",
                "/home/ken/qtbase",
            };
            var gcc_init_header = "g++-ubuntu-init.h";
            locations = gcc_locations;
            init_header = gcc_init_header;
        }

        // Add in Windows full paths just in case this program is executed
        // in Windows with Msys2 installed (pacman -S base-devel gcc vim cmake).
        var to_add_locations = locations.Select(l => "/msys64" + l).ToList();
        locations.AddRange(to_add_locations);

        // Create preprocessor.
        var assembly = Assembly.GetExecutingAssembly();
        // Derive clang++-init.h by clang++ -dM -E -x c++ - < /dev/null
        string strg = null;
        var manifestResourceNames = assembly.GetManifestResourceNames();
        foreach (var resourceName in manifestResourceNames)
        {
            if (!resourceName.Contains(init_header)) continue;
            using (var manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null) continue;
                using (var streamReader = new StreamReader(manifestResourceStream))
                {
                    strg = streamReader.ReadToEnd();
                    break;
                }
            }
        }
        if (strg == null) throw new Exception("Resource not added.");
        var str = CharStreams.fromString(strg);
        var lexer = new CPlusPlus14Lexer(str);
        lexer.PushMode(CPlusPlus14Lexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var parser = new CPlusPlus14Parser(tokens);
        var listener_lexer = new ErrorListener<int>(false);
        var listener_parser = new ErrorListener<IToken>(false);
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        var tree = parser.preprocessing_file();
        var visitor = new Test.Preprocessor(tokens, locations);
        visitor._noisy = this._noisy;
        visitor._current_file_name = init_header;
        visitor.Visit(tree);
        var result = visitor._preprocessor_symbols;
        return (result, locations);
    }

    bool Time()
    {
        var now = DateTime.Now;
        System.TimeSpan diff = now.Subtract(_last_time);
        var one = new System.TimeSpan(0, 0, 1);
        _last_time = now;
        if (System.TimeSpan.Compare(diff, one) > 0)
        {
            var tok = this.TokenStream.LT(1);
            System.Console.Error.WriteLine(tok.Line + " " + tok.Column);
        }
        //System.Console.WriteLine(DateTime.Now.ToString());
        return true;
    }
}
