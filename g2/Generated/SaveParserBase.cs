using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

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
        var init_table = InitPreprocessor();
        // Create preprocessor.
        var input = this.TokenStream;
        var src = input.TokenSource;
        var stream = src.InputStream;
        var strg = stream.ToString();
        strg = strg.Replace("\\\r\n", " ");
        strg = strg.Replace("\\\n", " ");
        strg = strg.Replace("\\\r", " ");
        var str = CharStreams.fromString(strg);
        //var sr = new StreamReader(stream);
        if (SeeOutput) System.Console.Error.WriteLine(strg);
        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var pp = new SaveParser(tokens);
        var tree = pp.preprocessing_file();
        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Preprocessor(tokens);
        visitor.preprocessor_symbols = init_table;
        if (File.Exists(SourceName))
        {
            visitor.probe_locations.Insert(0, Path.GetDirectoryName(SourceName));
        }
        visitor.Visit(tree);
        var real_input = visitor.sb.ToString();
        if (SeeOutput) System.Console.Error.WriteLine(real_input);
        var new_str = CharStreams.fromString(real_input);
        var new_lexer = new SaveLexer(new_str);
        var new_tokens = new CommonTokenStream(new_lexer);
        this.TokenStream = new_tokens;
        var real_this = this as SaveParser;
        IParseTree result = real_this.translation_unit();
        return result;
    }

    System.Collections.Generic.Dictionary<string, Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>> InitPreprocessor()
    {
        // Create preprocessor.
        var assembly = Assembly.GetExecutingAssembly();
        var nnn = "clang++-init.h";
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
        var str = CharStreams.fromString(strg);
        if (SeeOutput) System.Console.Error.WriteLine(strg);
        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var pp = new SaveParser(tokens);
        var tree = pp.preprocessing_file();
        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Preprocessor(tokens);
        visitor.Visit(tree);
        System.Collections.Generic.Dictionary<string, Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>> result = visitor.preprocessor_symbols;
        return result;
    }
}
