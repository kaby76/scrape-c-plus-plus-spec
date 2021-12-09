using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LanguageServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class ConstantExpressionMacroExpansion : CPlusPlus14ParserBaseVisitor<IParseTree>
    {
        PreprocessorSymbols _preprocessor_symbols;
        Antlr4.Runtime.TokenStreamRewriter _rewriter;
        ITokenStream _tokens;
        public bool _noisy = false;

        public ConstantExpressionMacroExpansion(PreprocessorSymbols preprocessor_symbols)
        {
            _preprocessor_symbols = preprocessor_symbols;
        }

        public string Expand(ITokenStream tokens, IParseTree tree)
        {
            var input = TreeOutput.Reconstruct(tokens, tree);
            do
            {
                if (_noisy) System.Console.Error.WriteLine("Input for expand is " + input);
                var str = new AntlrInputStream(input);
                var lexer = new CPlusPlus14Lexer(str);
                lexer.PushMode(CPlusPlus14Lexer.PP);
                var cts = new CommonTokenStream(lexer);
                _tokens = cts;
                _rewriter = new TokenStreamRewriter(_tokens);
                var parser = new CPlusPlus14Parser(_tokens);
                var listener_lexer = new ErrorListener<int>(false);
                var listener_parser = new ErrorListener<IToken>(false);
                lexer.RemoveErrorListeners();
                parser.RemoveErrorListeners();
                lexer.AddErrorListener(listener_lexer);
                parser.AddErrorListener(listener_parser);
                var subtree = parser.constant_expression_eof();
                if (listener_lexer.had_error || listener_parser.had_error)
                {
                    System.Console.Error.WriteLine("Error in parsing " + input);
                    System.Console.Error.WriteLine(Test.TreeOutput.OutputTokens(cts));
                    System.Console.Error.WriteLine(Test.TreeOutput.OutputTree(subtree, lexer, parser, cts).ToString());
                }
                this.Visit(subtree);
                var other = _rewriter.GetText();
                other = other.Replace("##", "");
                if (other == input)
                {
                    if (_noisy) System.Console.Error.WriteLine("Finished");
                    return other;
                }
                input = other;
            } while (true);
        }

        public override IParseTree VisitUnqualified_id([NotNull] CPlusPlus14Parser.Unqualified_idContext context)
        {
            // unqualified_id :  Identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  Minus class_name |  Minus decltype_specifier |  template_id ;
            var id = context.Identifier();
            if (id != null)
            {
                // Get value. Null if undefined, otherwise "".
                if (_preprocessor_symbols.IsDefined(id.GetText()))
                {
                    var b = _preprocessor_symbols.Find(id.GetText(),
                        out List<string> ids,
                        out CPlusPlus14Parser.Replacement_listContext repls,
                        out CommonTokenStream st,
                        out string fn);
                    var new_str = TreeOutput.Reconstruct(st, repls);
                    var payload = id.Payload;
                    var common_token = payload as CommonToken;
                    _rewriter.Replace(common_token.TokenIndex, new_str);
                }
            }
            return null;
        }

        public override IParseTree VisitPostfix_expression([NotNull] CPlusPlus14Parser.Postfix_expressionContext context)
        {
            // postfix_expression :  primary_expression |  postfix_expression LeftBracket expression RightBracket |  postfix_expression LeftBracket braced_init_list RightBracket |  postfix_expression LeftParen expression_list ? RightParen |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  postfix_expression Dot KWTemplate ?  id_expression |  postfix_expression Arrow KWTemplate ? id_expression |  postfix_expression Dot pseudo_destructor_name |  postfix_expression Arrow pseudo_destructor_name |  postfix_expression PlusPlus |  postfix_expression MinusMinus |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ;
            // postfix_expression : (  primary_expression |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ) ( LeftBracket expression RightBracket | LeftBracket braced_init_list RightBracket | LeftParen expression_list ? RightParen | Dot KWTemplate ? id_expression | Arrow KWTemplate ? id_expression | Dot pseudo_destructor_name | Arrow pseudo_destructor_name | PlusPlus | MinusMinus )* ;
            var pri = context.primary_expression();
            var exp = context.expression();
            var exp_list = context.expression_list();
            var simp_type = context.simple_type_specifier();
            var typename = context.typename_specifier();
            if (pri != null)
            {
                Visit(pri);
            }
            else if (typename != null && exp_list != null)
            {
                //Visit(post);
                //Visit(exp_list);
                // Find post in symbol table.
                // Symbol is actually unevaluated.
                var s = typename.GetText();
                //var s = v.ToString();
                var replace = EvalExpr(s, exp_list.First());
                if (replace != null)
                {
                    _rewriter.Replace(context.Start, context.Stop, replace);
                }
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitUnary_operator([NotNull] CPlusPlus14Parser.Unary_operatorContext context)
        {
            return null;
        }

        public override IParseTree VisitUnary_expression([NotNull] CPlusPlus14Parser.Unary_expressionContext context)
        {
            // unary_expression :  postfix_expression |  PlusPlus cast_expression |  MinusMinus cast_expression |  unary_operator cast_expression |  KWSizeof unary_expression |  KWSizeof LeftParen type_id RightParen |  KWSizeof Ellipsis LeftParen Identifier RightParen |  KWAlignof LeftParen type_id RightParen |  noexcept_expression |  new_expression |  delete_expression ;
            var post = context.postfix_expression();
            var cast = context.cast_expression();
            var plus = context.PlusPlus();
            var minus = context.MinusMinus();
            var unary = context.unary_operator();
            var size = context.KWSizeof();
            var align = context.KWAlignof();
            var new_ex = context.new_expression();
            var noexc = context.noexcept_expression();
            var del = context.delete_expression();
            if (post != null)
            {
                Visit(post);
            }
            else if (unary != null && cast != null)
            {
                Visit(unary);
                if (unary.GetText() == "defined")
                {
                }
                else
                {
                    Visit(cast);
                }
            }
            else throw new Exception();
            return null;
        }

        object EvalExpr(string fun, CPlusPlus14Parser.Expression_listContext args)
        {
            if (_preprocessor_symbols.Find(
                fun, out List<string> ids,
                out CPlusPlus14Parser.Replacement_listContext repls,
                out CommonTokenStream st,
                out string fn))
            {
                // evaluate fun(aa,ab,ac,...)
                var lparms = ids;
                var largs = args.initializer_list().initializer_clause()
                    .Select(p => p.GetText())
                    .ToList();
                Dictionary<string, string> map = new Dictionary<string, string>();
                for (int i = 0; i < lparms.Count; ++i)
                {
                    if (lparms[i] == "...")
                    {
                        // all args from this point on are matched to this parameter.
                        map["__VA_ARGS__"] = string.Join("", largs.Skip(i));
                        break;
                    }
                    else
                    {
                        map[lparms[i]] = largs[i];
                    }
                }
                var pp_tokens = repls.pp_tokens();
                if (pp_tokens == null)
                {
                    return null;
                }
                var frontier = TreeEdits.Frontier(
                    pp_tokens)
                    .ToList();
                var toks = frontier.Select(t => t.Symbol).ToList();
                if (toks == null)
                {
                    return null;
                }
                StringBuilder sb = new StringBuilder();
                foreach (var t in toks)
                {
                    var value = t.Text;
                    if (map.TryGetValue(value, out string xxx))
                    {
                        if (xxx.Contains("\n"))
                        { }
                        sb.Append(xxx);
                    }
                    else
                    {
                        if (value.Contains("\n"))
                        { }
                        sb.Append(value);
                    }
                }
                return sb.ToString();
            }
            return null;
        }
    }
}
