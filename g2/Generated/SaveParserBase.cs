using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public abstract class SaveParserBase : Parser
{
    private readonly ITokenStream _input;
    public bool SeeOutput { get; set; } = true;

    protected SaveParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput)
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
        var strg = stream.ToString();
        strg = strg.Replace("\\\r\n", " ");
        strg = strg.Replace("\\\n\r", " ");
        strg = strg.Replace("\\\n", " ");
        strg = strg.Replace("\\\r", " ");
        var str = CharStreams.fromString(strg);
        //var sr = new StreamReader(stream);
        //if (SeeOutput) System.Console.Error.WriteLine(strg);
        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var pp = new SaveParser(tokens);
        var tree = pp.preprocessing_file();
        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Test.Preprocessor(tokens, locations);
        visitor._preprocessor_symbols = new Test.PreprocessorSymbols(init_table);
        visitor._current_file_name = fn;
        if (File.Exists(SourceName))
        {
            visitor._probe_locations.Insert(0, Path.GetDirectoryName(SourceName));
        }
        visitor.Visit(tree);
        var real_input = visitor.sb.ToString();
        System.Console.WriteLine("FINISHED PREPROCESSING ENTIRE INPUT.");
        if (SeeOutput)
        {
            System.Console.Error.WriteLine(real_input);
        }
        var new_str = CharStreams.fromString(real_input);
        var new_lexer = new SaveLexer(new_str);
        var new_tokens = new CommonTokenStream(new_lexer);
        this.TokenStream = new_tokens;
        var real_this = this as SaveParser;
        IParseTree result = real_this.translation_unit();
        return result;
    }

    (Test.PreprocessorSymbols, List<string>) InitPreprocessor()
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
        var gpp_locations = new List<string>() {
            "/usr/include/c++/9",
            "/usr/include/x86_64-linux-gnu/c++/9",
            "/usr/include/c++/9/backward",
            "/usr/lib/gcc/x86_64-linux-gnu/9/include",
            "/usr/local/include",
            "/usr/include/x86_64-linux-gnu",
            "/usr/include",
            "/home/ken/qtbase/include",
            "/home/ken/qtbase",
        };
        // Create preprocessor.
        var assembly = Assembly.GetExecutingAssembly();
        // Derive clang++-init.h by clang++ -dM -E -x c++ - < /dev/null
        var nnn = "g++-ubuntu-init.h";
        string strg = null;
        var manifestResourceNames = assembly.GetManifestResourceNames();
        foreach (var resourceName in manifestResourceNames)
        {
            if (!resourceName.Contains(nnn)) continue;
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
        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var pp = new SaveParser(tokens);
        var tree = pp.preprocessing_file();
        var visitor = new Test.Preprocessor(tokens, gpp_locations);
        visitor._current_file_name = nnn;
        visitor.Visit(tree);
        var result = visitor._preprocessor_symbols;
        return (result, gpp_locations);
    }

    static DateTime _last_time = DateTime.Now;
    bool Time()
    {
        var now = DateTime.Now;
        System.TimeSpan diff = now.Subtract(_last_time);
        var one = new System.TimeSpan(0, 0, 1);
        _last_time = now;
        if (System.TimeSpan.Compare(diff, one) > 0)
        {
            var tok = this.TokenStream.LT(1);
            System.Console.WriteLine(tok.Line + " " + tok.Column);
        }
        //System.Console.WriteLine(DateTime.Now.ToString());
        return true;
    }
}
