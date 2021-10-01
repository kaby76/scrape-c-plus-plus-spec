using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LanguageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

    // A.1 Keywords 	 [gram.key] 
    // typedef_name :  identifier ;
    // namespace_name :  original_namespace_name |  namespace_alias ;

    public override IParseTree VisitOriginal_namespace_name([NotNull] SaveParser.Original_namespace_nameContext context)
    {
        throw new NotImplementedException();
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
        var pp_header = context.header_name();
        var id = context.Identifier();
        var pp_number = context.pp_number();
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

    // § A.2 	 1210  c ISO/IEC 	 N4296

    public override IParseTree VisitToken([NotNull] SaveParser.TokenContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitPreprocessing_op_or_punc([NotNull] SaveParser.Preprocessing_op_or_puncContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitLiteral([NotNull] SaveParser.LiteralContext context)
    {
        // literal :  Integer_literal |  Character_literal |  Floating_literal |  String_literal |  boolean_literal |  pointer_literal |  User_defined_literal ;
        var int_lit = context.Integer_literal();
        var float_lit = context.Floating_literal();
        if (float_lit == null) throw new Exception();
        string s = float_lit.GetText();
        int l = int.Parse(s);
        state[context] = l;
        return null;
    }

    public override IParseTree VisitBoolean_literal([NotNull] SaveParser.Boolean_literalContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitPointer_literal([NotNull] SaveParser.Pointer_literalContext context)
    {
        throw new NotImplementedException();
    }

    // § A.2 	 1214  c ISO/IEC 	 N4296
    // A.3 Basic concepts 	 [gram.basic] 

    public override IParseTree VisitTranslation_unit([NotNull] SaveParser.Translation_unitContext context)
    {
        throw new NotImplementedException();
    }

    // A.4 Expressions 	 [gram.expr] 

    public override IParseTree VisitPrimary_expression([NotNull] SaveParser.Primary_expressionContext context)
    {
        // primary_expression :  literal |  KWThis |  LeftParen expression RightParen |  id_expression |  lambda_expression |  fold_expression ;
        var literal = context.literal();
        var id_expr = context.id_expression();
        if (literal != null)
        {
            Visit(literal);
            state[context] = state[literal];
        }
        else if (id_expr != null)
        {
            Visit(id_expr);
            state[context] = state[id_expr];
        }
        else throw new Exception();
        return null;
    }

    public override IParseTree VisitId_expression([NotNull] SaveParser.Id_expressionContext context)
    {
        // id_expression :  unqualified_id |  qualified_id ;
        var unqual = context.unqualified_id();
        var qual = context.qualified_id();
        if (unqual != null)
        {
            Visit(unqual);
            state[context] = state[unqual];
        }
        else if (qual != null)
        {
            Visit(qual);
            state[context] = state[qual];
        }
        else throw new NotImplementedException();
        return null;
    }

    public override IParseTree VisitUnqualified_id([NotNull] SaveParser.Unqualified_idContext context)
    {
        // unqualified_id :  Identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  Minus class_name |  Minus decltype_specifier |  template_id ;
        var id = context.Identifier();
        if (id != null)
        {
            // Get value. Null if undefined, otherwise "".
            if (preprocessor_symbols.ContainsKey(id.GetText()))
            {
                var val = preprocessor_symbols[id.GetText()];
                var v = val.Item2.GetText();
                state[context] = v;
            }
            else
            {
                state[context] = null;
            }
        }
        else
            throw new NotImplementedException();
        return null;
    }

    public override IParseTree VisitQualified_id([NotNull] SaveParser.Qualified_idContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitNested_name_specifier([NotNull] SaveParser.Nested_name_specifierContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitLambda_expression([NotNull] SaveParser.Lambda_expressionContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitLambda_introducer([NotNull] SaveParser.Lambda_introducerContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitLambda_capture([NotNull] SaveParser.Lambda_captureContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitCapture_default([NotNull] SaveParser.Capture_defaultContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitCapture_list([NotNull] SaveParser.Capture_listContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitCapture([NotNull] SaveParser.CaptureContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitSimple_capture([NotNull] SaveParser.Simple_captureContext context)
    {
        throw new NotImplementedException();
    }

    // § A.4 	 1215  c ISO/IEC 	 N4296

    public override IParseTree VisitInit_capture([NotNull] SaveParser.Init_captureContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitLambda_declarator([NotNull] SaveParser.Lambda_declaratorContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitFold_expression([NotNull] SaveParser.Fold_expressionContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitFold_operator([NotNull] SaveParser.Fold_operatorContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitPostfix_expression([NotNull] SaveParser.Postfix_expressionContext context)
    {
        // postfix_expression :  primary_expression |  postfix_expression LeftBracket expression RightBracket |  postfix_expression LeftBracket braced_init_list RightBracket |  postfix_expression LeftParen expression_list ? RightParen |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  postfix_expression Dot KWTemplate ?  id_expression |  postfix_expression Arrow KWTemplate ? id_expression |  postfix_expression Dot pseudo_destructor_name |  postfix_expression Arrow pseudo_destructor_name |  postfix_expression PlusPlus |  postfix_expression MinusMinus |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ;
        var pri = context.primary_expression();
        if (pri == null) throw new Exception();
        Visit(pri);
        state[context] = state[pri];
        return null;
    }

    public override IParseTree VisitExpression_list([NotNull] SaveParser.Expression_listContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitPseudo_destructor_name([NotNull] SaveParser.Pseudo_destructor_nameContext context)
    {
        throw new NotImplementedException();
    }

    public override IParseTree VisitUnary_expression([NotNull] SaveParser.Unary_expressionContext context)
    {
        // unary_expression :  postfix_expression |  PlusPlus cast_expression |  MinusMinus cast_expression |  unary_operator cast_expression |  KWSizeof unary_expression |  KWSizeof LeftParen type_id RightParen |  KWSizeof Ellipsis LeftParen Identifier RightParen |  KWAlignof LeftParen type_id RightParen |  noexcept_expression |  new_expression |  delete_expression ;
        var post = context.postfix_expression();
        var cast = context.cast_expression();
        if (post == null) throw new Exception();
        Visit(post);
        state[context] = state[post];
        return null;
    }

    // § A.4 	 1216  c ISO/IEC 	 N4296

    public override IParseTree VisitUnary_operator([NotNull] SaveParser.Unary_operatorContext context)
    {
        throw new NotImplementedException();
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
            Visit(test);
            var v = state[test];
            if (v is int)
            {
                bool b = 0 != (int)v;
                state[context] = b;
                if (b)
                {
                    Visit(group);
                }
                else if (elif != null)
                {
                    Visit(elif);
                    var v2 = state[elif];
                    if (v2 is int)
                    {
                        bool b2 = 0 != (int)v2;
                        state[context] = b2;
                        if (b2)
                        {
                        }
                        else if (els != null)
                        {
                            Visit(els);
                            var v3 = state[els];
                            if (v3 is int)
                            {
                                var b3 = 0 != (int)v3;
                                state[context] = b3;
                            }
                        }
                    }
                }
            }
            else if (v is bool)
            {
                bool b = (bool)v;
                state[context] = b;
                if (b)
                {
                    Visit(group);
                }
                else if (elif != null)
                {
                    Visit(elif);
                    var v2 = state[elif];
                    if (v2 is int)
                    {
                        bool b2 = 0 != (int)v2;
                        state[context] = b2;
                        if (b2)
                        {
                        }
                        else if (els != null)
                        {
                            Visit(els);
                            var v3 = state[els];
                            if (v3 is int)
                            {
                                var b3 = 0 != (int)v3;
                                state[context] = b3;
                            }
                        }
                    }
                }
            }
        }
        else if (type_ifdef != null)
        {
            var id = context.Identifier();
            // Get value. Null if undefined, otherwise "".
            if (preprocessor_symbols.ContainsKey(id.GetText()))
            {
                state[context] = 1;
                Visit(group);
            }
            else
            {
                state[context] = null;
            }
        }
        else if (type_ifndef != null)
        {
            var id = context.Identifier();
            // Get value. Null if undefined, otherwise "".
            if (preprocessor_symbols.ContainsKey(id.GetText()))
            {
                state[context] = null;
            }
            else
            {
                state[context] = 1;
                Visit(group);
            }

        }

        return null;
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
        else if (context.KWIfndef() != null)
        {
            var id = context.Identifier().GetText();
            var b = preprocessor_symbols.ContainsKey(id);
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
        if (context.KWDefine() != null)
        {
            var id = context.Identifier().GetText();
            SaveParser.Replacement_listContext list = context.replacement_list();
            var parms = context.identifier_list();
            preprocessor_symbols[id] = new Tuple<SaveParser.Identifier_listContext, SaveParser.Replacement_listContext>(parms, list);
            sb.AppendLine(); // Per spec, output blank line.
        }
        else if (context.KWUndef() != null)
        {
            var id = context.Identifier().GetText();
            preprocessor_symbols.Remove(id);
        }
        else if (context.KWInclude() != null)
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
        var or = context.logical_or_expression();
        var and = context.logical_and_expression();
        if (or != null)
        {
            Visit(or);
            var v = state[or];
            bool b = v == null ? false : (bool)v;
            if (b)
            {
                state[context] = b;
                return null;
            }
        }
        Visit(and);
        var v2 = state[and];
        bool b2 = v2 == null ? false : (bool)v2;
        state[context] = b2;
        return null;
    }

    public override IParseTree VisitAdditive_expression([NotNull] SaveParser.Additive_expressionContext context)
    {
        // additive_expression :  multiplicative_expression |  additive_expression Plus multiplicative_expression |  additive_expression Minus multiplicative_expression ;
        var mul = context.multiplicative_expression();
        var add = context.additive_expression();
        if (add != null)
        {
            Visit(add);
            var v = state[add];
            var l = (int)v;
            Visit(mul);
            var v2 = (int)state[mul];
            if (context.Plus() != null)
            {
                state[context] = l + v2;
            }
            else if (context.Minus() != null)
            {
                state[context] = l - v2;
            }
        }
        else
        {
            Visit(mul);
            state[context] = state[mul];
        }
        return null;
    }

    public override IParseTree VisitAnd_expression([NotNull] SaveParser.And_expressionContext context)
    {
        // and_expression :  equality_expression |  and_expression ( And | KWBitAnd ) equality_expression ;
        var eq = context.equality_expression();
        var and = context.and_expression();
        int b = -1;
        if (and != null)
        {
            Visit(and);
            var v = state[and];
            b = v == null ? 0 : (int)v;
        }
        Visit(eq);
        var v2 = state[eq];
        if (v2 is string)
        {
            v2 = int.Parse(v2 as string);
        }
        else if (v2 is int)
        {
        }
        var b2 = v2 == null ? 0 : (int)v2;
        state[context] = b & b2;
        return null;
    }

    public override IParseTree VisitCast_expression([NotNull] SaveParser.Cast_expressionContext context)
    {
        // cast_expression :  unary_expression |  LeftParen type_id RightParen cast_expression ;
        if (context.unary_expression() != null)
        {
            var unary = context.unary_expression();
            Visit(unary);
            state[context] = state[unary];
        }
        else throw new Exception();
        return null;
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
        // equality_expression :  relational_expression |  equality_expression Equal relational_expression |  equality_expression NotEqual relational_expression ;
        var rel = context.relational_expression();
        var eq = context.equality_expression();
        object l = null;
        if (eq != null)
        {
            Visit(eq);
            var v = state[eq];
            l = v == null ? null : v;
            Visit(rel);
            var v2 = state[rel];
            state[context] = v == v2;
        }
        else
        {
            Visit(rel);
            state[context] = state[rel];
        }
        return null;
    }

    public override IParseTree VisitExclusive_or_expression([NotNull] SaveParser.Exclusive_or_expressionContext context)
    {
        // exclusive_or_expression :  and_expression |  exclusive_or_expression ( Caret | KWXor ) and_expression ;
        var and = context.and_expression();
        var xor = context.exclusive_or_expression();
        int b = 0;
        if (xor != null)
        {
            Visit(xor);
            var v = state[xor];
            b = v == null ? 0 : (int)v;
        }
        Visit(and);
        var v2 = state[and];
        int b2 = v2 == null ? 0 : (int)v2;
        state[context] = b ^ b2;
        return null;
    }

    public override IParseTree VisitInclusive_or_expression([NotNull] SaveParser.Inclusive_or_expressionContext context)
    {
        // inclusive_or_expression :  exclusive_or_expression |  inclusive_or_expression ( Or | KWBitOr ) exclusive_or_expression ;
        var ior = context.inclusive_or_expression();
        var xor = context.exclusive_or_expression();
        int b = 0;
        if (ior != null)
        {
            Visit(ior);
            var v = state[ior];
            b = v == null ? 0 : (int)v;
        }
        Visit(xor);
        var v2 = state[xor];
        int b2 = v2 == null ? 0 : (int)v2;
        state[context] = b | b2;
        return null;
    }

    public override IParseTree VisitLogical_and_expression([NotNull] SaveParser.Logical_and_expressionContext context)
    {
        var ior = context.inclusive_or_expression();
        var and = context.logical_and_expression();
        if (and != null)
        {
            Visit(and);
            var v = state[and];
            bool b = v == null ? false : (bool)v;
            if (!b)
            {
                state[context] = b;
                return null;
            }
        }
        Visit(ior);
        var v2 = state[ior];
        bool b2;
        if (v2 == null) b2 = false;
        if (v2 is bool) b2 = (bool)v2;
        else if (v2 is int) b2 = 0 != (int)v2;
        else throw new Exception();
        state[context] = b2;
        return null;
    }

    public override IParseTree VisitMultiplicative_expression([NotNull] SaveParser.Multiplicative_expressionContext context)
    {
        // multiplicative_expression :  pm_expression |  multiplicative_expression Star pm_expression |  multiplicative_expression Div pm_expression |  multiplicative_expression Mod pm_expression ;
        var pm = context.pm_expression();
        var mul = context.multiplicative_expression();
        if (mul != null)
        {
            Visit(mul);
            var v = state[mul];
            var l = (int)v;
            Visit(pm);
            var v2 = (int)state[pm];
            if (context.Star() != null)
            {
                state[context] = l * v2;
            }
            else if (context.Div() != null)
            {
                state[context] = l / v2;
            }
            else if (context.Mod() != null)
            {
                state[context] = l % v2;
            }
        }
        else
        {
            Visit(pm);
            state[context] = state[pm];
        }
        return null;
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
        // pm_expression :  cast_expression |  pm_expression DotStar cast_expression |  pm_expression ArrowStar cast_expression ;
        var cast = context.cast_expression();
        var pm = context.pm_expression();
        if (pm != null)
        {
            Visit(pm);
            var v = state[pm];
            var l = (int)v;
            Visit(cast);
            var v2 = (int)state[cast];
            if (context.DotStar() != null)
            {
                state[context] = l * v2;
            }
            else if (context.ArrowStar() != null)
            {
                state[context] = l / v2;
            }
        }
        else
        {
            Visit(cast);
            state[context] = state[cast];
        }
        return null;
    }

    public override IParseTree VisitRelational_expression([NotNull] SaveParser.Relational_expressionContext context)
    {
        // relational_expression :  shift_expression |  relational_expression Less shift_expression |  relational_expression Greater shift_expression |  relational_expression LessEqual shift_expression |  relational_expression GreaterEqual shift_expression ;
        var shift = context.shift_expression();
        var rel = context.relational_expression();
        IComparable l = null;
        if (rel != null)
        {
            Visit(rel);
            var v = state[rel];
            l = (v == null ? null : v) as IComparable;
            Visit(shift);
            var v2 = state[shift] as IComparable;
            if (context.Less() != null)
            {
                state[context] = l.CompareTo(v2) < 0;
            }
            else if (context.LessEqual() != null)
            {
                state[context] = l.CompareTo(v2) <= 0;
            }
            else if (context.Greater() != null)
            {
                state[context] = l.CompareTo(v2) > 0;
            }
            else if (context.GreaterEqual() != null)
            {
                state[context] = l.CompareTo(v2) >= 0;
            }
        }
        else
        {
            Visit(shift);
            state[context] = state[shift];
        }
        return null;
    }

    public override IParseTree VisitShift_expression([NotNull] SaveParser.Shift_expressionContext context)
    {
        // shift_expression :  additive_expression |  shift_expression LeftShift additive_expression |  shift_expression RightShift additive_expression ;
        var add = context.additive_expression();
        var shift = context.shift_expression();
        if (shift != null)
        {
            Visit(shift);
            var v = state[shift];
            var l = (int)v;
            Visit(add);
            var v2 = (int)state[add];
            if (context.LeftShift() != null)
            {
                state[context] = l << v2;
            }
            else if (context.RightShift() != null)
            {
                state[context] = l >> v2;
            }
        }
        else
        {
            Visit(add);
            state[context] = state[add];
        }
        return null;
    }

    public override IParseTree VisitThrow_expression([NotNull] SaveParser.Throw_expressionContext context)
    {
        throw new Exception();
    }

    public override IParseTree VisitExpression_statement([NotNull] SaveParser.Expression_statementContext context)
    {
        throw new Exception();
    }

}
