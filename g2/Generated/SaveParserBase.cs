using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

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
        // Create preprocessor.
        var input = this.TokenStream;
        var src = input.TokenSource;
        var stream = src.InputStream;
        var strg = stream.ToString();
        var str = CharStreams.fromString(strg);
        //var sr = new StreamReader(stream);

        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        var pp = new SaveParser(tokens);
        var tree = pp.preprocessing_file();
        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Preprocessor(tokens);
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
}
