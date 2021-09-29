// Template generated code from trgen 0.10.0

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using LanguageServer;

public class Program
{
    public static Parser Parser { get; set; }
    public static Lexer Lexer { get; set; }
    public static ITokenStream TokenStream { get; set; }
    public static IParseTree Tree { get; set; }
    public static string StartSymbol { get; set; } = "preprocessing_file";
    public static IParseTree Parse(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new SaveLexer(str);
        Lexer = lexer;
        lexer.PushMode(SaveLexer.PP);
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new SaveParser(tokens);
        Parser = parser;
        var tree = parser.preprocessing_file();
        Tree = tree;
        return tree;
    }

    static void Main(string[] args)
    {
        bool show_tree = false;
        bool show_tokens = false;
        bool old = false;
        bool two_byte = false;
        string file_name = null;
        string input = null;
        System.Text.Encoding encoding = null;
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].Equals("-tokens"))
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
        var lexer = new SaveLexer(str);
        lexer.PushMode(SaveLexer.PP);
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
	        lexer.PushMode(SaveLexer.PP);
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new SaveParser(tokens);
        var listener_lexer = new ErrorListener<int>();
        var listener_parser = new ErrorListener<IToken>();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        DateTime before = DateTime.Now;
        var tree = parser.preprocessing_file();
        DateTime after = DateTime.Now;
        System.Console.Error.WriteLine("Time: " + (after - before));
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

        // Walk parse tree and collect tokens from preprocessor.
        var visitor = new Preprocessor(tokens);
        //visitor.Visit(tree);
        System.Console.WriteLine(visitor.sb.ToString());
	    System.Environment.Exit(listener_lexer.had_error || listener_parser.had_error ? 1 : 0);
    }
}

class Preprocessor : SaveParserBaseVisitor<IParseTree>
{
    Dictionary<string, Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>> preprocessor_symbols = new Dictionary<string, Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>>();
    Dictionary<IParseTree, object> state = new Dictionary<IParseTree, object>();
    public StringBuilder sb = new StringBuilder();
    BufferedTokenStream _stream;

    public Preprocessor(BufferedTokenStream stream)
    {
        _stream = stream;
    }

    public override IParseTree VisitNew_line([NotNull] SaveParser.New_lineContext context)
    {
        for (IParseTree p = context; p != null; p = p.Parent)
        {
            //if (p is SaveParser.Text_lineContext)
            {
                var p1 = TreeEdits.LeftMostToken(context);
                var pp1 = p1.SourceInterval;
                var pp2 = p1.Payload;
                var index = pp2.TokenIndex;
                if (index >= 0)
                {
                    var p2 = _stream.GetHiddenTokensToLeft(index);
                    var p3 = TreeEdits.GetText(p2);
                    sb.Append(p3);
                }
                sb.AppendLine();
                break;
            }
        }
        return null;
    }
    public override IParseTree VisitPreprocessing_token([NotNull] SaveParser.Preprocessing_tokenContext context)
    {
        for (IParseTree p = context; p != null; p = p.Parent)
        {
            if (p is SaveParser.Text_lineContext)
            {
                var p1 = TreeEdits.LeftMostToken(context);
                var pp1 = p1.SourceInterval;
                var pp2 = p1.Payload;
                var index = pp2.TokenIndex;
                if (index >= 0)
                {
                    var p2 = _stream.GetHiddenTokensToLeft(index);
                    var p3 = TreeEdits.GetText(p2);
                    sb.Append(p3);
                }
                sb.Append(context.GetText());
                break;
            }
        }
        var pp_header = context.PPHeader_name();
        var id = context.Identifier();
        var pp_number = context.Pp_number();
        var char_lit = context.Character_literal();
        var user_def_char_list = context.User_defined_character_literal();
        var user_def_str_lit = context.User_defined_string_literal();
        var str_lit = context.String_literal();
        var pp_or = context.preprocessing_op_or_punc();
        if (pp_header != null)
        {
            state[context] = pp_header.GetText();
        }
        else if (id != null)
        {
            state[context] = id.GetText();
        }
        else if (pp_number != null)
        {
            state[context] = pp_number.GetText();
        }
        else if (char_lit != null)
        {
            state[context] = char_lit.GetText();
        }
        else if (user_def_char_list != null)
        {
            state[context] = user_def_char_list.GetText();
        }
        else if (user_def_str_lit != null)
        {
            state[context] = user_def_str_lit.GetText();
        }
        else if (str_lit != null)
        {
            state[context] = str_lit.GetText();
        }
        else if (pp_or != null)
        {
            state[context] = pp_or.GetText();
        }
        else
        {
            state[context] = context.GetChild(0).GetText();
        }
        return null;
    }

    public override IParseTree VisitPreprocessing_file([NotNull] SaveParser.Preprocessing_fileContext context)
    {
        return base.VisitPreprocessing_file(context);
    }

    public override IParseTree VisitGroup([NotNull] SaveParser.GroupContext context)
    {
        return base.VisitGroup(context);
    }

    public override IParseTree VisitGroup_part([NotNull] SaveParser.Group_partContext context)
    {
        return base.VisitGroup_part(context);
    }

    public override IParseTree VisitIf_section([NotNull] SaveParser.If_sectionContext context)
    {
        return base.VisitIf_section(context);
    }

    public override IParseTree VisitIf_group([NotNull] SaveParser.If_groupContext context)
    {
        // Evaluate the context expression.
        if (context.KWIf() != null)
        {
            var c = context.constant_expression();
            Visit(c);
            var v = state[c];
            bool b = (v is null) ? false : (bool)v;
            state[context] = b;
            state[context.Parent] = b;
            if (b)
            {
                Visit(context.group());
            }
        }
        else if (context.PPIfndef() != null)
        {
            var id = context.Identifier().GetText();
            var b = preprocessor_symbols.ContainsKey(id);
            state[context] = ! b;
            state[context.Parent] = ! b;
            if (! b)
            {
                Visit(context.group());
            }
        }
        else if (context.PPIfdef() != null)
        {
            var id = context.Identifier().GetText();
            var b = preprocessor_symbols.ContainsKey(id);
            state[context] = b;
            state[context.Parent] = b;
            if (b)
            {
                Visit(context.group());
            }
        }
        return null;
    }

    public override IParseTree VisitElif_groups([NotNull] SaveParser.Elif_groupsContext context)
    {
        return base.VisitElif_groups(context);
    }

    public override IParseTree VisitElif_group([NotNull] SaveParser.Elif_groupContext context)
    {
        return base.VisitElif_group(context);
    }

    public override IParseTree VisitElse_group([NotNull] SaveParser.Else_groupContext context)
    {
        // Get state from ancestor if_section. Do not visit if true.
        var if_section = context.Parent as SaveParser.If_sectionContext;
        var v = state[if_section];
        bool b = (v is null) ? false : (bool)v;
        if (!b)
        {
            var group = context.group();
            if (group != null) Visit(group);
        }
        return null;
    }

    public override IParseTree VisitEndif_line([NotNull] SaveParser.Endif_lineContext context)
    {
        return base.VisitEndif_line(context);
    }

    public override IParseTree VisitControl_line([NotNull] SaveParser.Control_lineContext context)
    {
        if (context.PPDefine() != null)
        {
            var id = context.Identifier().GetText();
            SaveParser.Replacement_listContext list = context.replacement_list();
            SaveParser.Identifier_listContext parms = context.identifier_list();
            preprocessor_symbols[id] = new Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>(parms, list);
            sb.AppendLine(); // Per spec, output blank line.
        }
        else if (context.PPUndef() != null)
        {
            var id = context.Identifier().GetText();
            preprocessor_symbols.Remove(id);
        }
        else if (context.PPInclude() != null)
        {
            var header = context.pp_tokens();
            VisitPp_tokens(header);
            // Get pp_tokens state.
            // This list obtained from https://stackoverflow.com/questions/344317/where-does-gcc-look-for-c-and-c-header-files
            // echo "#include <bogus.h>" > t.cc; g++ -v t.cc; rm t.cc
            // echo "#include <bogus.h>" > t.c; gcc -v t.c; rm t.c
            List<string> probe_locations = new List<string>()
            {
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/x86_64-pc-msys",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include/c++/backward",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/include-fixed",
                "/usr/include",
                "/usr/lib/gcc/x86_64-pc-msys/10.2.0/../../../../lib/../include/w32api",
            };
            // Fix for Windows.
            List<string> new_list = new List<string>();
            foreach (var s in probe_locations) { new_list.Add("c:/msys64" + s); }
            probe_locations = new_list;
            var header_string = state[header] as string;
            var angle_bracket_include = header_string[0] == '<';
            if (!angle_bracket_include)
            {
                // Look in standard file location.
                probe_locations.Insert(0, "./");
            }
            var stripped = header_string.Substring(1, header_string.Length - 2);
            // Find file.
            foreach (var l in probe_locations)
            {
                var dir = !l.EndsWith("/") ? l + "/" : l;
                if (File.Exists(dir + stripped))
                {
                    // Add file to input.
                    var input = File.ReadAllText(dir + stripped);
                    var str = new AntlrInputStream(input);
                    var lexer = new SaveLexer(str);
                    lexer.PushMode(SaveLexer.PP);
                    var tokens = new CommonTokenStream(lexer);
                    var parser = new SaveParser(tokens);
                    var listener_lexer = new ErrorListener<int>();
                    var listener_parser = new ErrorListener<IToken>();
                    lexer.AddErrorListener(listener_lexer);
                    parser.AddErrorListener(listener_parser);
                    DateTime before = DateTime.Now;
                    var tree = parser.preprocessing_file();
                    var visitor = new Preprocessor(tokens);
                    visitor.state = this.state;
                    visitor.preprocessor_symbols = this.preprocessor_symbols;
                    visitor.Visit(tree);
                    this.state = visitor.state;
                    this.preprocessor_symbols = visitor.preprocessor_symbols;
                    sb.AppendLine(visitor.sb.ToString());
                    break;
                }
            }
        }
        return null;
    }

    public override IParseTree VisitText_line([NotNull] SaveParser.Text_lineContext context)
    {
        var pp_tokens = context.pp_tokens();
        var new_line = context.new_line();
        if (pp_tokens != null)
        {
            Visit(pp_tokens);
        }
        if (new_line != null)
        {
            Visit(new_line);
        }
        state[context] = null;
        return null;
    }

    public override IParseTree VisitNon_directive([NotNull] SaveParser.Non_directiveContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitPp_tokens([NotNull] SaveParser.Pp_tokensContext context)
    {
        var preprocessing_tokens = context.preprocessing_token();
        foreach (var pp in preprocessing_tokens)
        {
            VisitPreprocessing_token(pp);
            if (state.TryGetValue(pp, out object v))
            {
                state[context] = v;
            }
            else
            {
                state[context] = null;
            }
        }
        return null;
    }

    private bool Eval(IParseTree node)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitConstant_expression([NotNull] SaveParser.Constant_expressionContext context)
    {
        var child = context.conditional_expression();
        Visit(child);
        state[context] = state[child];
        return null;
    }

    public override IParseTree VisitConditional_expression([NotNull] SaveParser.Conditional_expressionContext context)
    {
        if (context.Question() == null)
        {
            var child = context.logical_or_expression();
            Visit(child);
            state[context] = state[child];
        }
        else
        {
            var c = context.logical_or_expression();
            Visit(c);
            var v = state[c];
            bool b = (v is null) ? false : (bool)v;
            if (b)
            {
                var e = context.expression();
                Visit(e);
                state[context] = state[e];
            }
            else
            {
                var e = context.assignment_expression();
                Visit(e);
                state[context] = state[e];
            }
        }
        return null;
    }

    public override IParseTree VisitExpression([NotNull] SaveParser.ExpressionContext context)
    {
        var child = context.assignment_expression();
        Visit(child);
        state[context] = state[child];
        return null;
    }

    public override IParseTree VisitAssignment_expression([NotNull] SaveParser.Assignment_expressionContext context)
    {
        var first = context.conditional_expression();
        var thrw = context.throw_expression();
        if (first != null)
        {
            Visit(first);
            state[context] = state[first];
        }
        else if (thrw != null)
        {
            Visit(thrw);
            state[context] = state[thrw];
        }
        else
        {
            //logical_or_expression
            //assignment_operator
            var clause = context.initializer_clause();
            Visit(clause);
            state[context] = state[clause];
        }
        return null;
    }

    public override IParseTree VisitLogical_or_expression([NotNull] SaveParser.Logical_or_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitAdditive_expression([NotNull] SaveParser.Additive_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitAnd_expression([NotNull] SaveParser.And_expressionContext context)
    {
        throw new Exception();
    }


    public override IParseTree VisitCast_expression([NotNull] SaveParser.Cast_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitDelete_expression([NotNull] SaveParser.Delete_expressionContext context)
    {
        throw new Exception();
    }
    public override IParseTree VisitInitializer_clause([NotNull] SaveParser.Initializer_clauseContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitEquality_expression([NotNull] SaveParser.Equality_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitExclusive_or_expression([NotNull] SaveParser.Exclusive_or_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitFold_expression([NotNull] SaveParser.Fold_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitId_expression([NotNull] SaveParser.Id_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitInclusive_or_expression([NotNull] SaveParser.Inclusive_or_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitLambda_expression([NotNull] SaveParser.Lambda_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitLogical_and_expression([NotNull] SaveParser.Logical_and_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitMultiplicative_expression([NotNull] SaveParser.Multiplicative_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitNew_expression([NotNull] SaveParser.New_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitNoexcept_expression([NotNull] SaveParser.Noexcept_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitPm_expression([NotNull] SaveParser.Pm_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitPostfix_expression([NotNull] SaveParser.Postfix_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitPrimary_expression([NotNull] SaveParser.Primary_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitRelational_expression([NotNull] SaveParser.Relational_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitShift_expression([NotNull] SaveParser.Shift_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitThrow_expression([NotNull] SaveParser.Throw_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitUnary_expression([NotNull] SaveParser.Unary_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitExpression_list([NotNull] SaveParser.Expression_listContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitExpression_statement([NotNull] SaveParser.Expression_statementContext context)
    {
        throw new Exception();
    }
}

