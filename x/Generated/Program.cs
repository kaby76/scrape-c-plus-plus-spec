// Template generated code from trgen 0.10.0

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

public class Program
{
    private static bool _noisy = false;

    public static Parser Parser { get; set; }
    public static Lexer Lexer { get; set; }
    public static string Input { get; set; }
    public static ITokenStream TokenStream { get; set; }
    public static IParseTree Tree { get; set; }
    public static string StartSymbol { get; set; } = "translation_unit";
    public static IParseTree Parse(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new CPlusPlus14Lexer(str);
        Lexer = lexer;
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new CPlusPlus14Parser(tokens);
        Parser = parser;
        var tree = parser.translation_unit();
        Input = lexer.InputStream.ToString();
        TokenStream = parser.TokenStream;
        Tree = tree;
        return tree;
    }

    static void Main(string[] args)
    {

        //args = new string[] { "-file", "/home/ken/qtbase/src/corelib/global/qnamespace.h" };
        bool noisy = false;
        bool show_tree = false;
        bool show_tokens = false;
        bool old = false;
        bool two_byte = false;
        string file_name = null;
        string input = null;
        System.Text.Encoding encoding = null;
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].Equals("-noisy"))
            {
                noisy = true;
                continue;
            }
            else if (args[i].Equals("-tokens"))
            {
                show_tokens = true;
                continue;
            }
            else if (args[i].Equals("-two-byte"))
            {
                two_byte = true;
                continue;
            }
            else if (args[i].Equals("-old"))
            {
                old = true;
                continue;
            }
            else if (args[i].Equals("-tree"))
            {
                show_tree = true;
                continue;
            }
            else if (args[i].Equals("-input"))
                input = args[++i];
            else if (args[i].Equals("-file"))
                file_name = args[++i];
            else if (args[i].Equals("-encoding"))
            {
                ++i;
                encoding = Encoding.GetEncoding(
                    args[i],
                    new EncoderReplacementFallback("(unknown)"),
                    new DecoderReplacementFallback("(error)"));
                if (encoding == null)
                    throw new Exception(@"Unknown encoding. Must be an Internet Assigned Numbers Authority (IANA) code page name. https://www.iana.org/assignments/character-sets/character-sets.xhtml");
            }
        }
        ICharStream str = null;
        if (input == null && file_name == null)
        {
            str = CharStreams.fromStream(System.Console.OpenStandardInput());
        } else if (input != null)
        {
            str = CharStreams.fromString(input);
        } else if (file_name != null)
        {
            if (two_byte)
                str = new TwoByteCharStream(file_name);
            else if (old)
            {
                FileStream fs = new FileStream(file_name, FileMode.Open);
                str = new Antlr4.Runtime.AntlrInputStream(fs);
            }
            else if (encoding == null)
                str = CharStreams.fromPath(file_name);
            else
                str = CharStreams.fromPath(file_name, encoding);
        }
        var lexer = new CPlusPlus14Lexer(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.NextToken();
                var token = (CommonToken)ro_token;
                token.TokenIndex = i;
                new_s.AppendLine(token.ToString());
                if (token.Type == Antlr4.Runtime.TokenConstants.EOF)
                    break;
            }
            System.Console.Error.WriteLine(new_s.ToString());
            lexer.Reset();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new CPlusPlus14Parser(tokens);
        parser._noisy = noisy;
        var listener_lexer = new ErrorListener<int>(false);
        var listener_parser = new ErrorListener<IToken>(false);
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        DateTime before = DateTime.Now;
        var tree = parser.translation_unit();
        DateTime after = DateTime.Now;
        if (_noisy) System.Console.Error.WriteLine("Time: " + (after - before));
        if (listener_lexer.had_error || listener_parser.had_error)
        {
            System.Console.Error.WriteLine("Parse failed.");
        }
        else
        {
            System.Console.Error.WriteLine("Parse succeeded.");
        }
        if (show_tree)
        {
            System.Console.Error.WriteLine(tree.ToStringTree(parser));
        }
        System.Environment.Exit(listener_lexer.had_error || listener_parser.had_error ? 1 : 0);
    }
}
