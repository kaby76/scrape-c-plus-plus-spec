using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LanguageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Test
{
    public class Preprocessor : Cpp14ParserBaseVisitor<IParseTree>
    {
        public PreprocessorSymbols _preprocessor_symbols = new PreprocessorSymbols();
        Dictionary<IParseTree, object> state = new Dictionary<IParseTree, object>();
        public StringBuilder sb = new StringBuilder();
        CommonTokenStream _stream;
        public string _current_file_name;
        public List<string> _probe_locations;
        private bool _noisy = false;

        public Preprocessor(CommonTokenStream stream, List<string> probe_locations)
        {
            _stream = stream;
            _probe_locations = probe_locations;
        }

        public override IParseTree VisitConstant_expression([NotNull] Cpp14Parser.Constant_expressionContext context)
        {
            // constant_expression :  conditional_expression ;

            var child = context.conditional_expression();

            // Do macro expansion+evaluation.

            ConstantExpressionMacroExpansion ccc = new ConstantExpressionMacroExpansion(_preprocessor_symbols);
            var new_replacement = ccc.Expand(_stream, context);
            if (new_replacement.Contains("QT_VERSION_CHECK"))
            {

            }

            ConstantExpressionEvaluator ddd = new ConstantExpressionEvaluator(_preprocessor_symbols);
            state[context] = ddd.Evaluate(new_replacement);
            return null;
        }

        public override IParseTree VisitPreprocessing_token([NotNull] Cpp14Parser.Preprocessing_tokenContext context)
        {
            base.VisitPreprocessing_token(context);
            return null;
        }

        public override IParseTree VisitPreprocessing_op_or_punc([NotNull] Cpp14Parser.Preprocessing_op_or_puncContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitBoolean_literal([NotNull] Cpp14Parser.Boolean_literalContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitPointer_literal([NotNull] Cpp14Parser.Pointer_literalContext context)
        {
            throw new NotImplementedException();
        }


        // A.14 Preprocessing directives 	 [gram.cpp]

        public override IParseTree VisitPreprocessing_file([NotNull] Cpp14Parser.Preprocessing_fileContext context)
        {
            return base.VisitPreprocessing_file(context);
        }

        public override IParseTree VisitGroup([NotNull] Cpp14Parser.GroupContext context)
        {
            var check = TreeOutput.Reconstruct(this._stream, context);
            if (check.Contains("QT_VERSION_CHECK"))
            {

            }
            // Collect all adjacent text_line and step through that with macro expansion.
            // For others, use visitor.
            for (int c = 0; c < context.ChildCount;)
            {
                var child = context.GetChild(c).GetChild(0);
                if (child is Cpp14Parser.Text_lineContext)
                {
                    int count = 1;
                    var start = c;
                    var end = c + 1;
                    for (c++; c < context.ChildCount; ++c)
                    {
                        end = c;
                        if (!(context.GetChild(c).GetChild(0) is Cpp14Parser.Text_lineContext))
                        {
                            break;
                        }
                        count++;
                    }

                    // Collect all tokens, including new lines, and expand macros.
                    var toks = context.children
                        .ToList()
                        .GetRange(start, count)
                        .SelectMany(ch =>
                        {
                            var text = ch.GetChild(0) as Cpp14Parser.Text_lineContext;
                            if (text == null) return new Cpp14Parser.Preprocessing_tokenContext[0];
                            else if (text.pp_tokens() == null) return new Cpp14Parser.Preprocessing_tokenContext[0];
                            var list = text.pp_tokens().preprocessing_token().Select(x => x.GetChild(0)).ToList();
                            list.Add(text.new_line());
                            var arr = list.ToArray();
                            return arr;
                        }).ToList();

                    StringBuilder eval = new StringBuilder();
                    if (toks != null)
                    {
                        for (int i = 0; i < toks.Count; ++i)
                        {
                            var tok = toks[i];
                            if (tok is TerminalNodeImpl && (tok as TerminalNodeImpl).Symbol.Text == "\\")
                            {
                                var next_tok = toks[i + 1];
                                if (next_tok.GetText() == "n")
                                {
                                    i += 1;
                                    Add(sb, this._stream, tok, "");
                                    sb.AppendLine();
                                    continue;
                                }
                            }
                            if (tok is TerminalNodeImpl && (tok as TerminalNodeImpl).Symbol.Type == Cpp14Parser.Identifier)
                            {
                                var fun = tok.GetText();
                                if (this._preprocessor_symbols.Find(
                                    fun,
                                    out List<string> ids,
                                    out Cpp14Parser.Replacement_listContext repls,
                                    out CommonTokenStream st,
                                    out string fn))
                                {
                                    // First, get the parameters of the macro.
                                    var lparms = ids;
                                    // If there are no parameters, then just add the macro value to the output.
                                    if (lparms == null || lparms.Count == 0)
                                    {
                                        var foo = repls?.pp_tokens()?.preprocessing_token();
                                        if (foo != null)
                                        {
                                            foreach (var s in repls.pp_tokens().preprocessing_token())
                                            {
                                                Add(sb, st, s);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // There are some parameters of the macro.
                                        // Scan ahead for the argument values of the macro. We'll use this
                                        // to make substitutions.
                                        var (get_args, e) = GetArgs(lparms, toks, i);
                                        i = e;
                                        // Instantiate the macro.
                                        StringBuilder sb2 = new StringBuilder();
                                        Dictionary<string, string> map = new Dictionary<string, string>();
                                        for (int k = 0; k < lparms.Count; ++k)
                                        {
                                            if (lparms[k] == "...")
                                            {
                                                // all args from this point on are matched to this parameter.
                                                map["__VA_ARGS__"] = string.Join("", get_args.Skip(k));
                                                break;
                                            }
                                            else
                                            {
                                                map[lparms[k]] = get_args[k];
                                            }
                                        }
                                        var pp_tokens = repls.pp_tokens();
                                        if (pp_tokens == null)
                                        {
                                            continue;
                                        }
                                        var frontier = TreeEdits.Frontier(
                                            pp_tokens)
                                            .ToList();
                                        var toks2 = frontier.ToList();
                                        if (toks2 == null)
                                        {
                                            continue;
                                        }
                                        bool stringize = false;
                                        foreach (var t in toks2)
                                        {
                                            var value = t.Symbol.Text;
                                            if (value == "#")
                                            {
                                                stringize = true;
                                                continue;
                                            }
                                            if (map.TryGetValue(value, out string xxx))
                                            {
                                                if (stringize)
                                                    Add(sb2, st, t, '"' + xxx + '"');
                                                else
                                                    Add(sb2, st, t, xxx);
                                            }
                                            else
                                            {
                                                Add(sb2, st, t);
                                            }
                                            stringize = false;
                                        }
                                        // Reparse and call recursively until fix-point.
                                        var todo = sb2.ToString();
                                        var regex = new Regex("[ \t]*[#][#][ \t]*");
                                        todo = regex.Replace(todo, "");
                                        do
                                        {
                                            if (_noisy) System.Console.Error.WriteLine("Input reparse and expand " + todo);
                                            var str = new AntlrInputStream(todo);
                                            var lexer = new Cpp14Lexer(str);
                                            lexer.PushMode(Cpp14Lexer.PP);
                                            var tokens = new CommonTokenStream(lexer);
                                            var parser = new Cpp14Parser(tokens);
                                            var listener_lexer = new ErrorListener<int>(true);
                                            var listener_parser = new ErrorListener<IToken>(true);
                                            lexer.RemoveErrorListeners();
                                            parser.RemoveErrorListeners();
                                            lexer.AddErrorListener(listener_lexer);
                                            parser.AddErrorListener(listener_parser);
                                            DateTime before = DateTime.Now;
                                            var tree = parser.group();
                                            DateTime after = DateTime.Now;
                                            if (_noisy) System.Console.Error.WriteLine("Time: " + (after - before));
                                            var visitor = new Preprocessor(tokens, this._probe_locations);
                                            visitor._current_file_name = this._current_file_name;
                                            visitor._preprocessor_symbols = this._preprocessor_symbols;
                                            visitor._probe_locations = this._probe_locations;
                                            visitor.Visit(tree);
                                            this._preprocessor_symbols = visitor._preprocessor_symbols;
                                            this._probe_locations = visitor._probe_locations;
                                            var new_todo = visitor.sb.ToString();
                                            new_todo = regex.Replace(new_todo, "");
                                            if (new_todo.ToLower() == "true" || new_todo.ToLower() == "false")
                                            {
                                                new_todo = new_todo.ToLower();
                                            }
                                            if (_noisy) System.Console.Error.WriteLine("Got back " + new_todo);
                                            if (new_todo == todo)
                                                break;
                                            todo = new_todo;
                                        } while (true);
                                        sb.Append(todo);
                                    }
                                }
                                else
                                {
                                    Add(sb, this._stream, tok);
                                }
                            }
                            else
                            {
                                Add(sb, this._stream, tok);
                            }
                        }
                    }
                }
                else if (context.GetChild(c) != null)
                {
                    Visit(context.GetChild(c));
                    c++;
                }
                else
                {
                    c++;
                }
            }
            return null;
        }

        public override IParseTree VisitGroup_part([NotNull] Cpp14Parser.Group_partContext context)
        {
            return base.VisitGroup_part(context);
        }

        public override IParseTree VisitIf_section([NotNull] Cpp14Parser.If_sectionContext context)
        {
            // if_section: (Pound KWIf constant_expression new_line group ? | Pound KWIfdef Identifier new_line group ? | Pound KWIfndef Identifier new_line group ? ) elif_groups? else_group ? endif_line;
            var type_if = context.KWIf();
            var type_ifdef = context.KWIfdef();
            var type_ifndef = context.KWIfndef();
            var test = context.constant_expression();
            var group = context.group();
            var elif = context.elif_groups();
            var els = context.else_group();
            if (type_if != null)
            {
                var st = test.GetText();
                Visit(test);
                var v = state[test];
                ConvertToBool(v, out bool b);
                state[context] = b;
                if (b)
                {
                    Visit(group);
                }
                if (elif != null && !b)
                {
                    Visit(elif);
                    var v2 = state[elif];
                    ConvertToBool(v2, out bool b2);
                    b = b2;
                    state[context] = b;
                }
                if (els != null && !b)
                {
                    Visit(els);
                    //var v3 = state[els];
                    //state[context] = v3;
                }
            }
            else if (type_ifdef != null)
            {
                var id = context.Identifier();
                bool b = _preprocessor_symbols.IsDefined(id.GetText());
                state[context] = b;
                if (b)
                {
                    Visit(group);
                }
                if (elif != null && !b)
                {
                    Visit(elif);
                    var v2 = state[elif];
                    ConvertToBool(v2, out bool b2);
                    b = b2;
                    state[context] = b;
                }
                if (els != null && !b)
                {
                    Visit(els);
                    //var v3 = state[els];
                    //state[context] = v3;
                }
            }
            else if (type_ifndef != null)
            {
                var id = context.Identifier();
                bool b = !_preprocessor_symbols.IsDefined(id.GetText());
                state[context] = b;
                if (b)
                {
                    Visit(group);
                }
                if (elif != null && !b)
                {
                    Visit(elif);
                    var v2 = state[elif];
                    ConvertToBool(v2, out bool b2);
                    b = b2;
                    state[context] = b;
                }
                if (els != null && !b)
                {
                    Visit(els);
                    //var v3 = state[els];
                    //state[context] = v3;
                }
            }
            return null;
        }

        public override IParseTree VisitIf_group([NotNull] Cpp14Parser.If_groupContext context)
        {
            // Evaluate the context expression.
            if (context.KWIf() != null)
            {
                var c = context.constant_expression();
                Visit(c);
                var v = state[c];
                ConvertToBool(v, out bool b);
                state[context] = b;
                state[context.Parent] = b;
                if (b)
                {
                    Visit(context.group());
                }
            }
            else if (context.KWIfndef() != null)
            {
                var id = context.Identifier().GetText();
                var b = _preprocessor_symbols.IsDefined(id);
                state[context] = !b;
                state[context.Parent] = !b;
                if (!b)
                {
                    Visit(context.group());
                }
            }
            else if (context.KWIfdef() != null)
            {
                var id = context.Identifier().GetText();
                var b = _preprocessor_symbols.IsDefined(id);
                state[context] = b;
                state[context.Parent] = b;
                if (b)
                {
                    Visit(context.group());
                }
            }
            return null;
        }

        public override IParseTree VisitElif_groups([NotNull] Cpp14Parser.Elif_groupsContext context)
        {
            foreach (var g in context.elif_group())
            {
                Visit(g);
                var v = state[g];
                bool b = v != null && v is bool ? (bool)v : false;
                state[context] = state[g];
                if (b) break;
            }
            return null;
        }

        public override IParseTree VisitElif_group([NotNull] Cpp14Parser.Elif_groupContext context)
        {
            // elif_group :  Pound KWElif constant_expression new_line group ? ;
            var exp = context.constant_expression();
            Visit(exp);
            var v = state[exp];
            bool b = v != null && v is bool ? (bool)v : false;
            state[context] = v;
            if (b)
            {
                var group = context.group();
                Visit(group);
            }
            return null;
        }

        public override IParseTree VisitElse_group([NotNull] Cpp14Parser.Else_groupContext context)
        {
            // Get state from ancestor if_section. Do not visit if true.
            var if_section = context.Parent as Cpp14Parser.If_sectionContext;
            var v = state[if_section];
            ConvertToBool(v, out bool b);
            if (!b)
            {
                var group = context.group();
                if (group != null) Visit(group);
            }
            return null;
        }

        public override IParseTree VisitEndif_line([NotNull] Cpp14Parser.Endif_lineContext context)
        {
            return base.VisitEndif_line(context);
        }

        public override IParseTree VisitControl_line([NotNull] Cpp14Parser.Control_lineContext context)
        {
            // control_line :  Pound KWInclude pp_tokens new_line |  Pound KWDefine Identifier replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list ? RightParen replacement_list new_line |  Pound KWDefine Identifier lparen Ellipsis RightParen replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list Comma Ellipsis RightParen replacement_list new_line |  Pound KWUndef Identifier new_line |  Pound KWLine pp_tokens new_line |  Pound (KWError|KWWarning) pp_tokens ? new_line |  Pound KWPragma pp_tokens ? new_line |  Pound new_line ;
            var define = context.KWDefine();
            var id = context.Identifier();
            var lp = context.lparen();
            var pp_tokens = context.pp_tokens();
            var repl = context.replacement_list();
            var idlist = context.identifier_list();
            var ellip = context.Ellipsis();
            if (context.KWDefine() != null)
            {
                Cpp14Parser.Replacement_listContext list = context.replacement_list();
                var id_string = id.GetText();
                if (id_string.Contains("QT_VERSION_CHECK"))
                {

                }
                var parms1 = context.identifier_list();
                var parms2 = context.Ellipsis();
                var parms = new List<string>();
                if (parms1 != null)
                    parms.AddRange(parms1.Identifier().Select(p => p.GetText()));
                if (parms2 != null)
                    parms.Add(parms2.GetText());
                _preprocessor_symbols.Add(id.GetText(),
                    parms, list, _stream, _current_file_name);
                sb.AppendLine(); // Per spec, output blank line.
            }
            else if (context.KWUndef() != null)
            {
                _preprocessor_symbols.Delete(id.GetText());
            }
            else if (context.KWInclude() != null)
            {
                var header = context.pp_tokens()?.preprocessing_token().ToList();
                if (header.Count > 1 || header.Count == 0) throw new Exception();
                var inc_file_name = header.First().GetText();
                // This list obtained from https://stackoverflow.com/questions/344317/where-does-gcc-look-for-c-and-c-header-files
                // echo "#include <bogus.h>" > t.cc; g++ -v t.cc; rm t.cc
                // echo "#include <bogus.h>" > t.c; gcc -v t.c; rm t.c
                // Fix for Windows.
                List<string> new_list = new List<string>();
                //foreach (var s in probe_locations) { new_list.Add("c:/msys64" + s); }
                //probe_locations = new_list;
                var header_string = inc_file_name as string;
                var angle_bracket_include = header_string[0] == '<';
                if (!angle_bracket_include)
                {
                    // Look in standard file location.
                    _probe_locations.Insert(0, "./");
                }
                var stripped = header_string.Substring(1, header_string.Length - 2);
                // Find file.
                bool found = false;
                foreach (var l in _probe_locations)
                {
                    var dir = !l.EndsWith("/") ? l + "/" : l;
                    var p = Path.Combine(dir, stripped);
                    p = Path.GetFullPath(p);
                    //System.Console.Error.WriteLine("Trying " + p);
                    if (File.Exists(p))
                    {
                        found = true;
                        System.Console.Error.WriteLine("Including " + p);
                        var to_add = Path.GetDirectoryName(p);
                        _probe_locations.Insert(0, to_add);
                        // Add file to input.
                        var strg = File.ReadAllText(p);
                        strg = strg.Replace("\\\r\n", " ");
                        strg = strg.Replace("\\\n\r", " ");
                        strg = strg.Replace("\\\n", " ");
                        strg = strg.Replace("\\\r", " ");
                        var str = new AntlrInputStream(strg);
                        var lexer = new Cpp14Lexer(str);
                        lexer.PushMode(Cpp14Lexer.PP);
                        var tokens = new CommonTokenStream(lexer);
                        var parser = new Cpp14Parser(tokens);
                        var listener_lexer = new ErrorListener<int>(true);
                        var listener_parser = new ErrorListener<IToken>(true);
                        lexer.RemoveErrorListeners();
                        parser.RemoveErrorListeners();
                        lexer.AddErrorListener(listener_lexer);
                        parser.AddErrorListener(listener_parser);
                        DateTime before = DateTime.Now;
                        var tree = parser.preprocessing_file();
                        DateTime after = DateTime.Now;
                        if (_noisy) System.Console.Error.WriteLine("Time: " + (after - before));
                        var visitor = new Preprocessor(tokens, this._probe_locations);
                        visitor._current_file_name = p;
                        visitor.state = this.state;
                        visitor._preprocessor_symbols = this._preprocessor_symbols;
                        visitor._probe_locations = this._probe_locations;
                        visitor.Visit(tree);
                        this.state = visitor.state;
                        this._preprocessor_symbols = visitor._preprocessor_symbols;
                        this._probe_locations = visitor._probe_locations;
                        var replacement_text = visitor.sb.ToString();
                        sb.AppendLine(replacement_text);
                        _probe_locations.RemoveAt(0);
                        if (_noisy) System.Console.Error.WriteLine("Back on " + this._current_file_name);
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("Cannot find " + stripped);
                }
            }
            return null;
        }

        public override IParseTree VisitNon_directive([NotNull] Cpp14Parser.Non_directiveContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLparen([NotNull] Cpp14Parser.LparenContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitIdentifier_list([NotNull] Cpp14Parser.Identifier_listContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitReplacement_list([NotNull] Cpp14Parser.Replacement_listContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitPp_tokens([NotNull] Cpp14Parser.Pp_tokensContext context)
        {
            base.VisitPp_tokens(context);
            return null;
        }

        public override IParseTree VisitNew_line([NotNull] Cpp14Parser.New_lineContext context)
        {
            var p1 = TreeEdits.LeftMostToken(context);
            var pp1 = p1.SourceInterval;
            var pp2 = p1.Payload;
            var index = pp2.TokenIndex;
            if (index >= 0)
            {
                var p2 = _stream.GetHiddenTokensToLeft(index);
                var p3 = TreeEdits.GetText(p2);
                if (p3.Contains("\\\n"))
                { }
                sb.Append(p3);
            }
            sb.AppendLine();
            return null;
        }

        private void ParseNumber(string s, out object l)
        {
            s = s.ToLower();
            if (s.EndsWith("ll"))
                s = s.Substring(0, s.Length - 2);
            else if (s.EndsWith("l"))
                s = s.Substring(0, s.Length - 1);
            else if (char.IsDigit(s[s.Length - 1]))
            { }
            else throw new Exception();
            try
            {
                l = int.Parse(s);
                return;
            }
            catch (Exception)
            {
            }
            try
            {
                l = long.Parse(s);
                return;
            }
            catch (Exception)
            {
            }
            try
            {
                l = float.Parse(s);
                return;
            }
            catch (Exception)
            {
            }
            try
            {
                l = double.Parse(s);
                return;
            }
            catch (Exception)
            {
            }
            l = 0;
        }

        public override IParseTree VisitText_line([NotNull] Cpp14Parser.Text_lineContext context)
        {
            throw new Exception("Should not be here");
        }

        private void Add(StringBuilder sb, CommonTokenStream stream, IParseTree tree, string replacement = null)
        {
            var p1 = TreeEdits.LeftMostToken(tree);
            var pp1 = p1.SourceInterval;
            var pp2 = p1.Payload;
            var index = pp2.TokenIndex;
            if (index >= 0)
            {
                var p2 = stream.GetHiddenTokensToLeft(index);
                var p3 = TreeEdits.GetText(p2);
                if (p3.Contains("\\\n"))
                { }
                sb.Append(p3);
            }
            if (replacement == null)
            {
                //sb.Append(tree.GetText());
                var str = TreeOutput.Reconstruct(stream, tree);
                if (str.Contains("\\\n"))
                { }
                sb.Append(str);
            }
            else
            {
                if (replacement.Contains("\\\n"))
                { }
                sb.Append(replacement);
            }
        }

        private (List<string>, int) GetArgs(List<string> lparms, List<IParseTree> toks, int i)
        {
            // "i" points to the name of macro call. We need to skip past this name, and the "(", to
            // get to the first argument.
            List<string> args = new List<string>();
            int last = i + 3;
            int level = 0;
            StringBuilder sb = new StringBuilder();
            for (int j = i + 2; j < toks.Count; ++j)
            {
                if (toks[j].GetText() == ",")
                {
                    if (level == 0)
                    {
                        args.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                    else
                        Add(sb, this._stream, toks[j]);
                }
                else if (toks[j].GetText() == "(")
                {
                    Add(sb, this._stream, toks[j]);
                    level++;
                }
                else if (toks[j].GetText() == ")")
                {
                    if (level == 0)
                    {
                        args.Add(sb.ToString());
                        last = j;
                        break;
                    }
                    else
                        Add(sb, this._stream, toks[j]);
                    level--;
                }
                else
                {
                    Add(sb, this._stream, toks[j]);
                }
            }
            return (args, last);
        }

        object EvalExpr(string fun, Cpp14Parser.Expression_listContext args)
        {
            return null;
        }

        private void ConvertToBool(object v, out bool b)
        {
            if (v == null)
            {
                b = false;
            }
            else if (v is bool)
            {
                b = (bool)v;
            }
            else if (v is int)
            {
                int i = (int)v;
                b = i != 0;
            }
            else if (v is long)
            {
                long i = (long)v;
                b = i != 0;
            }
            else if (v is string)
            {
                if (v.ToString().ToLower() == "true")
                    b = true;
                else if (v.ToString().ToLower() == "false")
                    b = false;
                else
                {
                    ParseNumber(v.ToString(), out object n);
                    if (n is int)
                    {
                        int i = (int)n;
                        b = i != 0;
                    }
                    else if (n is long)
                    {
                        long i = (long)n;
                        b = i != 0;
                    }
                    else throw new Exception();
                }
            }
            else throw new Exception();
        }

    }
}
