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

    public class ConstantExpressionEvaluator : SaveParserBaseVisitor<IParseTree>
    {
        Dictionary<IParseTree, object> state = new Dictionary<IParseTree, object>();
        public PreprocessorSymbols _preprocessor_symbols;

        public ConstantExpressionEvaluator(PreprocessorSymbols preprocessor_symbols)
        {
            _preprocessor_symbols = preprocessor_symbols;
        }

        public object Evaluate(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new SaveLexer(str);
            lexer.PushMode(SaveLexer.PP);
            var _tokens = new CommonTokenStream(lexer);
            var parser = new SaveParser(_tokens);
            var listener_lexer = new ErrorListener<int>();
            var listener_parser = new ErrorListener<IToken>();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);
            var subtree = parser.constant_expression_eof();
            this.Visit(subtree);
            return state[subtree];
        }

        public override IParseTree VisitLiteral([NotNull] SaveParser.LiteralContext context)
        {
            // literal :  Integer_literal |  Character_literal |  Floating_literal |  String_literal |  boolean_literal |  pointer_literal |  User_defined_literal ;
            var int_lit = context.Integer_literal();
            var float_lit = context.Floating_literal();
            var char_lit = context.Character_literal();
            var str_lit = context.String_literal();
            var bool_lit = context.boolean_literal();
            if (int_lit != null)
            {
                string s = int_lit.GetText();
                ParseNumber(s, out object l);
                state[context] = l;
            }
            else if (float_lit != null)
            {
                string s = float_lit.GetText();
                ParseNumber(s, out object l);
                state[context] = l;
            }
            else if (bool_lit != null)
            {
                string s = bool_lit.GetText();
                try
                {
                    var b = bool.Parse(s);
                    state[context] = b;
                }
                catch
                {
                    state[context] = false;
                }
            }
            else if (str_lit != null)
            {
                state[context] = str_lit.GetText();
            }
            else if (char_lit != null)
            {
                state[context] = char_lit.GetText();
            }
            else throw new Exception();
            return null;
        }


        // A.4 Expressions   [gram.expr] 

        public override IParseTree VisitPrimary_expression([NotNull] SaveParser.Primary_expressionContext context)
        {
            // primary_expression :  literal |  KWThis |  LeftParen expression RightParen |  id_expression |  lambda_expression |  fold_expression ;
            var literal = context.literal();
            var id_expr = context.id_expression();
            var expr = context.expression();
            var lp = context.LeftParen();
            var kwthis = context.KWThis();
            var lambda = context.lambda_expression();
            var fold = context.fold_expression();
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
            else if (lp != null)
            {
                var exp = context.expression();
                Visit(exp);
                state[context] = state[exp];
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
                var (ids, repl, str, fn) = _preprocessor_symbols.Find(id.GetText());
                if (ids != null && repl != null)
                {
                    var v = repl.GetText();
                    state[context] = v;
                }
                else
                {
                    state[context] = 1;
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
            var post = context.postfix_expression();
            var exp = context.expression();
            var exp_list = context.expression_list();
            var simp_type = context.simple_type_specifier();
            var typename = context.typename_specifier();
            if (pri != null)
            {
                Visit(pri);
                state[context] = state[pri];
            }
            else if (post != null && exp_list != null)
            {
                Visit(post);
                Visit(exp_list);
                // Find post in symbol table.
                var v = state[post];
                // Symbol is actually unevaluated.
                var s = post.GetText();
                //var s = v.ToString();
                state[context] = EvalExpr(s, exp_list);
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitExpression_list([NotNull] SaveParser.Expression_listContext context)
        {
            // expression_list :  initializer_list ;
            var init_list = context.initializer_list();
            Visit(init_list);
            state[context] = state[init_list];
            return null;
        }

        public override IParseTree VisitPseudo_destructor_name([NotNull] SaveParser.Pseudo_destructor_nameContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitUnary_operator([NotNull] SaveParser.Unary_operatorContext context)
        {
            return null;
        }

        public override IParseTree VisitUnary_expression([NotNull] SaveParser.Unary_expressionContext context)
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
                state[context] = state[post];
            }
            else if (unary != null && cast != null)
            {
                if (unary.GetText() == "defined")
                {
                    if (cast?.unary_expression()?.postfix_expression()?.primary_expression()?.LeftParen() != null)
                    {
                        var variable = cast?.unary_expression()?.postfix_expression()?.primary_expression()?.expression()?.GetText();
                        state[context] = _preprocessor_symbols.IsDefined(variable);
                    }
                    else
                    {
                        var variable = cast.GetText();
                        state[context] = _preprocessor_symbols.IsDefined(variable);
                    }
                }
                else if (unary.GetText() == "!")
                {
                    Visit(unary);
                    Visit(cast);
                    var v = state[cast];
                    ConvertToBool(v, out bool b);
                    state[context] = !b;
                }
                else throw new Exception();
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitNew_expression([NotNull] SaveParser.New_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_placement([NotNull] SaveParser.New_placementContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_type_id([NotNull] SaveParser.New_type_idContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_declarator([NotNull] SaveParser.New_declaratorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNoptr_abstract_declarator([NotNull] SaveParser.Noptr_abstract_declaratorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_initializer([NotNull] SaveParser.New_initializerContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitDelete_expression([NotNull] SaveParser.Delete_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNoexcept_expression([NotNull] SaveParser.Noexcept_expressionContext context)
        {
            throw new NotImplementedException();
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

        public override IParseTree VisitAdditive_expression([NotNull] SaveParser.Additive_expressionContext context)
        {
            // additive_expression :  multiplicative_expression |  additive_expression Plus multiplicative_expression |  additive_expression Minus multiplicative_expression ;
            var mul = context.multiplicative_expression();
            var add = context.additive_expression();
            var plus = context.Plus();
            if (add != null)
            {
                Visit(add);
                Visit(mul);
                var lhs_v = state[add];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[mul];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int lhs = (int)lhs_n;
                    int rhs = (int)rhs_n;
                    int res = plus != null ? lhs + rhs : lhs - rhs;
                    state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long lhs = (long)lhs_n;
                    long rhs = (long)rhs_n;
                    long res = plus != null ? lhs + rhs : lhs - rhs;
                    state[context] = res;
                }
                else throw new Exception();
            }
            else
            {
                Visit(mul);
                state[context] = state[mul];
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
                Visit(add);
                var lhs_v = state[shift];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[add];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int lhs = (int)lhs_n;
                    int rhs = (int)rhs_n;
                    int res = context.LeftShift() != null ? lhs << rhs : lhs >> rhs;
                    state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long lhs = (long)lhs_n;
                    int rhs = (int)rhs_n;
                    long res = context.LeftShift() != null ? lhs << rhs : lhs >> rhs;
                    state[context] = res;
                }
                else throw new Exception();
            }
            else
            {
                Visit(add);
                state[context] = state[add];
            }
            return null;
        }

        public override IParseTree VisitRelational_expression([NotNull] SaveParser.Relational_expressionContext context)
        {
            // relational_expression :  shift_expression |  relational_expression Less shift_expression |  relational_expression Greater shift_expression |  relational_expression LessEqual shift_expression |  relational_expression GreaterEqual shift_expression ;
            var shift = context.shift_expression();
            var rel = context.relational_expression();
            if (rel != null)
            {
                Visit(rel);
                Visit(shift);
                var lhs_v = state[rel];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[shift];
                if (rhs_v == null)
                {

                }
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (context.Less() != null)
                {
                    if (lhs_n is int && rhs_n is int)
                        state[context] = ((int)lhs_n).CompareTo((int)rhs_n) < 0;
                    else if (lhs_n is long || rhs_n is long)
                        state[context] = ((long)lhs_n).CompareTo((long)rhs_n) < 0;
                    else throw new Exception();
                }
                else if (context.LessEqual() != null)
                {
                    if (lhs_n is int && rhs_n is int)
                        state[context] = ((int)lhs_n).CompareTo((int)rhs_n) <= 0;
                    else if (lhs_n is long || rhs_n is long)
                        state[context] = ((long)lhs_n).CompareTo((long)rhs_n) <= 0;
                    else throw new Exception();
                }
                else if (context.Greater() != null)
                {
                    if (lhs_n is int && rhs_n is int)
                        state[context] = ((int)lhs_n).CompareTo((int)rhs_n) > 0;
                    else if (lhs_n is long || rhs_n is long)
                        state[context] = ((long)lhs_n).CompareTo((long)rhs_n) > 0;
                    else throw new Exception();
                }
                else if (context.GreaterEqual() != null)
                {
                    if (lhs_n is int && rhs_n is int)
                        state[context] = ((int)lhs_n).CompareTo((int)rhs_n) >= 0;
                    else if (lhs_n is long || rhs_n is long)
                        state[context] = ((long)lhs_n).CompareTo((long)rhs_n) >= 0;
                    else throw new Exception();
                }
            }
            else
            {
                Visit(shift);
                state[context] = state[shift];
            }
            return null;
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

        public override IParseTree VisitAnd_expression([NotNull] SaveParser.And_expressionContext context)
        {
            // and_expression :  equality_expression |  and_expression ( And | KWBitAnd ) equality_expression ;
            var eq = context.equality_expression();
            var and = context.and_expression();
            if (and != null)
            {
                Visit(and);
                Visit(eq);
                var lhs_v = state[and];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[eq];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int lhs = (int)lhs_n;
                    int rhs = (int)rhs_n;
                    int res = lhs & rhs;
                    state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long lhs = (long)lhs_n;
                    long rhs = (long)rhs_n;
                    long res = lhs & rhs;
                    state[context] = res;
                }
                else throw new Exception();
            }
            else
            {
                Visit(eq);
                state[context] = state[eq];
            }
            return null;
        }

        public override IParseTree VisitExclusive_or_expression([NotNull] SaveParser.Exclusive_or_expressionContext context)
        {
            // exclusive_or_expression :  and_expression |  exclusive_or_expression ( Caret | KWXor ) and_expression ;
            var and = context.and_expression();
            var xor = context.exclusive_or_expression();
            if (xor != null)
            {
                Visit(xor);
                Visit(and);
                var lhs_v = state[xor];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[and];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int lhs = (int)lhs_n;
                    int rhs = (int)rhs_n;
                    int res = lhs ^ rhs;
                    state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long lhs = (long)lhs_n;
                    long rhs = (long)rhs_n;
                    long res = lhs ^ rhs;
                    state[context] = res;
                }
                else throw new Exception();
            }
            else
            {
                Visit(and);
                state[context] = state[and];
            }
            return null;
        }

        public override IParseTree VisitInclusive_or_expression([NotNull] SaveParser.Inclusive_or_expressionContext context)
        {
            // inclusive_or_expression :  exclusive_or_expression |  inclusive_or_expression ( Or | KWBitOr ) exclusive_or_expression ;
            var ior = context.inclusive_or_expression();
            var xor = context.exclusive_or_expression();
            if (ior != null)
            {
                Visit(ior);
                Visit(xor);
                var lhs_v = state[ior];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var rhs_v = state[xor];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int lhs = (int)lhs_n;
                    int rhs = (int)rhs_n;
                    int res = lhs | rhs;
                    state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long lhs = (long)lhs_n;
                    long rhs = (long)rhs_n;
                    long res = lhs | rhs;
                    state[context] = res;
                }
                else throw new Exception();
            }
            else
            {
                Visit(xor);
                state[context] = state[xor];
            }
            return null;
        }

        public override IParseTree VisitLogical_and_expression([NotNull] SaveParser.Logical_and_expressionContext context)
        {
            // logical_and_expression :  inclusive_or_expression |  logical_and_expression ( AndAnd | KWAnd ) inclusive_or_expression ;
            var ior = context.inclusive_or_expression();
            var and = context.logical_and_expression();
            if (and != null)
            {
                Visit(and);
                var v = state[and];
                ConvertToBool(v, out bool b);
                if (!b)
                {
                    state[context] = b;
                    return null;
                }
                Visit(ior);
                var v2 = state[ior];
                ConvertToBool(v2, out bool b2);
                state[context] = b2;
                return null;
            }
            else
            {
                Visit(ior);
                state[context] = state[ior];
            }
            return null;
        }

        public override IParseTree VisitLogical_or_expression([NotNull] SaveParser.Logical_or_expressionContext context)
        {
            // logical_or_expression :  logical_and_expression |  logical_or_expression ( OrOr | KWOr ) logical_and_expression ;
            var or = context.logical_or_expression();
            var and = context.logical_and_expression();
            if (or != null)
            {
                Visit(or);
                var v = state[or];
                ConvertToBool(v, out bool b);
                if (b)
                {
                    state[context] = b;
                    return null;
                }
                Visit(and);
                var v2 = state[and];
                ConvertToBool(v2, out bool b2);
                state[context] = b2;
                return null;
            }
            else
            {
                Visit(and);
                state[context] = state[and];
            }
            return null;
        }

        public override IParseTree VisitConditional_expression([NotNull] SaveParser.Conditional_expressionContext context)
        {
            // conditional_expression :  logical_or_expression |  logical_or_expression Question expression Colon assignment_expression ;
            var lor = context.logical_or_expression();
            var exp = context.expression();
            var aexp = context.assignment_expression();
            if (context.Question() == null)
            {
                Visit(lor);
                state[context] = state[lor];
            }
            else
            {
                Visit(lor);
                var v = state[lor];
                ConvertToBool(v, out bool b);
                if (b)
                {
                    Visit(exp);
                    state[context] = state[exp];
                }
                else
                {
                    Visit(aexp);
                    state[context] = state[aexp];
                }
            }
            return null;
        }

        public override IParseTree VisitThrow_expression([NotNull] SaveParser.Throw_expressionContext context)
        {
            throw new NotImplementedException();
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

        public override IParseTree VisitAssignment_operator([NotNull] SaveParser.Assignment_operatorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitExpression([NotNull] SaveParser.ExpressionContext context)
        {
            var child = context.assignment_expression();
            Visit(child);
            state[context] = state[child];
            return null;
        }

        public override IParseTree VisitConstant_expression([NotNull] SaveParser.Constant_expressionContext context)
        {
            var child = context.conditional_expression();
            Visit(child);
            state[context] = state[child];
            return null;
        }

        public override IParseTree VisitConstant_expression_eof([NotNull] SaveParser.Constant_expression_eofContext context)
        {
            // constant_expression_eof :  conditional_expression EOF ;
            var cond = context.conditional_expression();
            VisitConditional_expression(cond);
            state[context] = state[cond];
            return null;
        }

        public override IParseTree VisitInitializer_clause([NotNull] SaveParser.Initializer_clauseContext context)
        {
            // initializer_clause :  assignment_expression |  braced_init_list ;
            var assign = context.assignment_expression();
            var brace = context.braced_init_list();
            if (assign != null)
            {
                Visit(assign);
                var v = state[assign];
                state[context] = v;
            }
            else
            {
                Visit(brace);
                var v = state[brace];
                state[context] = v;
            }
            return null;
        }

        public override IParseTree VisitInitializer_list([NotNull] SaveParser.Initializer_listContext context)
        {
            // initializer_list :  initializer_clause Ellipsis ? ( Comma initializer_clause Ellipsis ? )* ;
            var init_clauses = context.initializer_clause();
            var init_states = new List<object>();
            foreach (var ic in init_clauses)
            {
                Visit(ic);
                init_states.Add(state[ic]);
            }
            state[context] = init_states;
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
                ;
            else throw new Exception();
            try
            {
                l = int.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = long.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = float.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = double.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            l = 0;
        }

        object EvalExpr(string fun, SaveParser.Expression_listContext args)
        {
            if (this._preprocessor_symbols.Find(
                fun,
                out List<string> ids,
                out SaveParser.Replacement_listContext repls,
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
                var toks = pp_tokens.preprocessing_token();
                if (toks == null)
                {
                    return null;
                }
                StringBuilder eval = new StringBuilder();
                for (int i = 0; i < toks.Length; ++i)
                {
                    var value = toks[i].GetText();
                    if (map.TryGetValue(value, out string xxx))
                    {
                        eval.Append(" " + xxx);
                    }
                    else eval.Append(" " + value);
                }

                // Reparse and call recursively until fix-point.
                var todo = eval.ToString();
                do
                {
                    throw new Exception();
                    var str = new AntlrInputStream(todo);
                    var lexer = new SaveLexer(str);
                    lexer.PushMode(SaveLexer.PP);
                    var tokens = new CommonTokenStream(lexer);
                    var parser = new SaveParser(tokens);
                    var listener_lexer = new ErrorListener<int>();
                    var listener_parser = new ErrorListener<IToken>();
                    lexer.AddErrorListener(listener_lexer);
                    parser.AddErrorListener(listener_parser);
                    DateTime before = DateTime.Now;
                    var tree = parser.constant_expression_eof();
                    DateTime after = DateTime.Now;
                    System.Console.Error.WriteLine("Time: " + (after - before));
                    //var visitor = new ConstantExpressionMacroExpansion(tokens);
                    //visitor._current_file_name = this._current_file_name;
                    //visitor.state = this.state;
                    //visitor.preprocessor_symbols = this.preprocessor_symbols;
                    //visitor.probe_locations = this.probe_locations;
                    //visitor.Visit(tree);
                    //this.state = visitor.state;
                    //this.preprocessor_symbols = visitor.preprocessor_symbols;
                    //this.probe_locations = visitor.probe_locations;
                    //var new_todo = visitor.state[tree].ToString();
                    //if (new_todo.ToLower() == "true" || new_todo.ToLower() == "false")
                    //{
                    //    new_todo = new_todo.ToLower();
                    //}
                    //if (new_todo == todo)
                    //    break;
                    //todo = new_todo;
                } while (true);
                return todo;
            }
            //  else throw new Exception("Use of undefined macro " + fun + " in file " + this._current_file_name);
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

    public class PreprocessorSymbols
    {
        bool debug = false;

        public Dictionary<
            string,
            Tuple<
                List<string>, // params
                SaveParser.Replacement_listContext, // value of def
                CommonTokenStream, // token stream where define is.
                string>> map; // file name where define is.
        public CommonTokenStream Tokens { get; private set; }

        public PreprocessorSymbols(PreprocessorSymbols copy)
        {
            map = new Dictionary<string, Tuple<List<string>, SaveParser.Replacement_listContext, CommonTokenStream, string>>(copy.map);
        }

        public PreprocessorSymbols()
        {
            map = new Dictionary<string, Tuple<List<string>, SaveParser.Replacement_listContext, CommonTokenStream, string>>();
        }

        public void Add(string name,
            List<string> ids,
            SaveParser.Replacement_listContext repl,
            CommonTokenStream ts,
            string fn)
        {
            if (ids.Contains("..."))
            {

            }
            if (debug) System.Console.Error.WriteLine("Defining " + name);
            map[name] = new Tuple<List<string>, SaveParser.Replacement_listContext, CommonTokenStream, string>(ids, repl, ts, fn);
        }

        public void Delete(string name)
        {
            if (debug) System.Console.Error.WriteLine("Undefining " + name);
            this.map.Remove(name);
        }

        public (List<string>,
            SaveParser.Replacement_listContext,
            CommonTokenStream,
            string)
            Find(string name)
        {
            if (debug) System.Console.Error.WriteLine("Find " + name);
            if (map.TryGetValue(name, out Tuple<List<string>, // params
              SaveParser.Replacement_listContext, // value of def
              CommonTokenStream, // token stream where define is.
              string> t))
            {
                if (debug) System.Console.Error.WriteLine("Yes!");
                var ids = t.Item1;
                var repls = t.Item2;
                var stream = t.Item3;
                var fn = t.Item4;
                return (ids, repls, stream, fn);
            }
            else
            {
                if (debug) System.Console.Error.WriteLine("Nope!");
                return (null, null, null, null);
            }
        }

        public bool Find(string name,
            out List<string> ids,
            out SaveParser.Replacement_listContext repls,
            out CommonTokenStream stream,
            out string fn)
        {
            if (debug) System.Console.Error.WriteLine("Find " + name);
            if (map.TryGetValue(name, out Tuple<List<string>, // params
                SaveParser.Replacement_listContext, // value of def
                CommonTokenStream, // token stream where define is.
                string> t))
            {
                if (debug) System.Console.Error.WriteLine("Yes!");
                ids = t.Item1;
                repls = t.Item2;
                stream = t.Item3;
                fn = t.Item4;
                return true;
            }
            else
            {
                ids = null;
                repls = null;
                stream = null;
                fn = null;
                if (debug) System.Console.Error.WriteLine("Nope!");
                return false;
            }
        }

        public bool IsDefined(string name)
        {
            if (debug) System.Console.Error.WriteLine("IsDefined " + name);
            var result = map.TryGetValue(name, out Tuple<List<string>, // params
                SaveParser.Replacement_listContext, // value of def
                CommonTokenStream, // token stream where define is.
                string> t);
            if (debug) System.Console.Error.WriteLine("returning " + result);
            return result;
        }
    }

    public class ConstantExpressionMacroExpansion : SaveParserBaseVisitor<IParseTree>
    {
        PreprocessorSymbols _preprocessor_symbols;
        Antlr4.Runtime.TokenStreamRewriter _rewriter;
        ITokenStream _tokens;

        public ConstantExpressionMacroExpansion(PreprocessorSymbols preprocessor_symbols)
        {
            _preprocessor_symbols = preprocessor_symbols;
        }

        public string Expand(ITokenStream tokens, IParseTree tree)
        {
            var input = TreeOutput.Reconstruct(tokens, tree);
            do
            {
                System.Console.Error.WriteLine("Input for expand is " + input);
                var str = new AntlrInputStream(input);
                var lexer = new SaveLexer(str);
                lexer.PushMode(SaveLexer.PP);
                var cts = new CommonTokenStream(lexer);
                _tokens = cts;
                _rewriter = new TokenStreamRewriter(_tokens);
                var parser = new SaveParser(_tokens);
                var listener_lexer = new ErrorListener<int>();
                var listener_parser = new ErrorListener<IToken>();
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
                    System.Console.Error.WriteLine("Finished");
                    return other;
                }
                input = other;
            } while (true);
        }

        public override IParseTree VisitUnqualified_id([NotNull] SaveParser.Unqualified_idContext context)
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
                        out SaveParser.Replacement_listContext repls,
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

        public override IParseTree VisitPostfix_expression([NotNull] SaveParser.Postfix_expressionContext context)
        {
            // postfix_expression :  primary_expression |  postfix_expression LeftBracket expression RightBracket |  postfix_expression LeftBracket braced_init_list RightBracket |  postfix_expression LeftParen expression_list ? RightParen |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  postfix_expression Dot KWTemplate ?  id_expression |  postfix_expression Arrow KWTemplate ? id_expression |  postfix_expression Dot pseudo_destructor_name |  postfix_expression Arrow pseudo_destructor_name |  postfix_expression PlusPlus |  postfix_expression MinusMinus |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ;
            var pri = context.primary_expression();
            var post = context.postfix_expression();
            var exp = context.expression();
            var exp_list = context.expression_list();
            var simp_type = context.simple_type_specifier();
            var typename = context.typename_specifier();
            if (pri != null)
            {
                Visit(pri);
            }
            else if (post != null && exp_list != null)
            {
                //Visit(post);
                //Visit(exp_list);
                // Find post in symbol table.
                // Symbol is actually unevaluated.
                var s = post.GetText();
                //var s = v.ToString();
                var replace = EvalExpr(s, exp_list);
                if (replace != null)
                {
                    _rewriter.Replace(context.Start, context.Stop, replace);
                }
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitUnary_operator([NotNull] SaveParser.Unary_operatorContext context)
        {
            return null;
        }

        public override IParseTree VisitUnary_expression([NotNull] SaveParser.Unary_expressionContext context)
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


        object EvalExpr(string fun, SaveParser.Expression_listContext args)
        {
            if (_preprocessor_symbols.Find(
                fun, out List<string> ids,
                out SaveParser.Replacement_listContext repls,
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
                        sb.Append(xxx);
                    }
                    else sb.Append(value);
                }
                return sb.ToString();

                //// Reparse and call recursively until fix-point.
                //var todo = eval.ToString();
                //do
                //{
                //    var str = new AntlrInputStream(todo);
                //    var lexer = new SaveLexer(str);
                //    lexer.PushMode(SaveLexer.PP);
                //    var tokens = new CommonTokenStream(lexer);
                //    var parser = new SaveParser(tokens);
                //    var listener_lexer = new ErrorListener<int>();
                //    var listener_parser = new ErrorListener<IToken>();
                //    lexer.AddErrorListener(listener_lexer);
                //    parser.AddErrorListener(listener_parser);
                //    DateTime before = DateTime.Now;
                //    var tree = parser.constant_expression_eof();
                //    var visitor = new Preprocessor(tokens);
                //    visitor._current_file_name = this._current_file_name;
                //    visitor.state = this.state;
                //    visitor.preprocessor_symbols = this.preprocessor_symbols;
                //    visitor.probe_locations = this.probe_locations;
                //    visitor.Visit(tree);
                //    this.state = visitor.state;
                //    this.preprocessor_symbols = visitor.preprocessor_symbols;
                //    this.probe_locations = visitor.probe_locations;
                //    var new_todo = visitor.state[tree].ToString();
                //    if (new_todo.ToLower() == "true" || new_todo.ToLower() == "false")
                //    {
                //        new_todo = new_todo.ToLower();
                //    }
                //    if (new_todo == todo)
                //        break;
                //    todo = new_todo;
                //} while (true);
                //return todo;
            }
            //else throw new Exception("Use of undefined macro " + fun + " in file " + this._current_file_name);
            return null;
        }

        ////////////////////////////////////////////

    }

    public class Preprocessor : SaveParserBaseVisitor<IParseTree>
    {
        public PreprocessorSymbols _preprocessor_symbols = new PreprocessorSymbols();
        Dictionary<IParseTree, object> state = new Dictionary<IParseTree, object>();
        public StringBuilder sb = new StringBuilder();
        CommonTokenStream _stream;
        public string _current_file_name;
        public List<string> _probe_locations;

        public Preprocessor(CommonTokenStream stream, List<string> probe_locations)
        {
            _stream = stream;
            _probe_locations = probe_locations;
        }

        public override IParseTree VisitConstant_expression([NotNull] SaveParser.Constant_expressionContext context)
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

        public override IParseTree VisitPreprocessing_token([NotNull] SaveParser.Preprocessing_tokenContext context)
        {
            base.VisitPreprocessing_token(context);
            return null;
        }

        public override IParseTree VisitPreprocessing_op_or_punc([NotNull] SaveParser.Preprocessing_op_or_puncContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitBoolean_literal([NotNull] SaveParser.Boolean_literalContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitPointer_literal([NotNull] SaveParser.Pointer_literalContext context)
        {
            throw new NotImplementedException();
        }


        // A.14 Preprocessing directives 	 [gram.cpp]

        public override IParseTree VisitPreprocessing_file([NotNull] SaveParser.Preprocessing_fileContext context)
        {
            return base.VisitPreprocessing_file(context);
        }

        public override IParseTree VisitGroup([NotNull] SaveParser.GroupContext context)
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
                if (child is SaveParser.Text_lineContext)
                {
                    int count = 1;
                    var start = c;
                    var end = c + 1;
                    for (c++; c < context.ChildCount; ++c)
                    {
                        end = c;
                        if (!(context.GetChild(c).GetChild(0) is SaveParser.Text_lineContext))
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
                            var text = ch.GetChild(0) as SaveParser.Text_lineContext;
                            if (text == null) return new SaveParser.Preprocessing_tokenContext[0];
                            else if (text.pp_tokens() == null) return new SaveParser.Preprocessing_tokenContext[0];
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
                            if (tok is TerminalNodeImpl && (tok as TerminalNodeImpl).Symbol.Type == SaveParser.Identifier)
                            {
                                var fun = tok.GetText();
                                if (fun == "Q_NAMESPACE_EXPORT")
                                {

                                }
                                if (this._preprocessor_symbols.Find(
                                 fun,
                                 out List<string> ids,
                                 out SaveParser.Replacement_listContext repls,
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
                                        foreach (var t in toks2)
                                        {
                                            var value = t.Symbol.Text;
                                            if (map.TryGetValue(value, out string xxx))
                                            {
                                                Add(sb2, st, t, xxx);
                                            }
                                            else
                                            {
                                                Add(sb2, st, t);
                                            }
                                        }
                                        // Reparse and call recursively until fix-point.
                                        var todo = sb2.ToString();
                                        var regex = new Regex("[ \t]*[#][#][ \t]*");
                                        todo = regex.Replace(todo, "");
                                        do
                                        {
                                            System.Console.Error.WriteLine("Input reparse and expand " + todo);
                                            var str = new AntlrInputStream(todo);
                                            var lexer = new SaveLexer(str);
                                            lexer.PushMode(SaveLexer.PP);
                                            var tokens = new CommonTokenStream(lexer);
                                            var parser = new SaveParser(tokens);
                                            var listener_lexer = new ErrorListener<int>();
                                            var listener_parser = new ErrorListener<IToken>();
                                            lexer.AddErrorListener(listener_lexer);
                                            parser.AddErrorListener(listener_parser);
                                            DateTime before = DateTime.Now;
                                            var tree = parser.group();
                                            DateTime after = DateTime.Now;
                                            System.Console.Error.WriteLine("Time: " + (after - before));
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
                                            System.Console.Error.WriteLine("Got back " + new_todo);
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

        public override IParseTree VisitIf_group([NotNull] SaveParser.If_groupContext context)
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

        public override IParseTree VisitElif_groups([NotNull] SaveParser.Elif_groupsContext context)
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

        public override IParseTree VisitElif_group([NotNull] SaveParser.Elif_groupContext context)
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

        public override IParseTree VisitElse_group([NotNull] SaveParser.Else_groupContext context)
        {
            // Get state from ancestor if_section. Do not visit if true.
            var if_section = context.Parent as SaveParser.If_sectionContext;
            var v = state[if_section];
            ConvertToBool(v, out bool b);
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
                SaveParser.Replacement_listContext list = context.replacement_list();
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
                        if (strg.Contains("QT_VERSION_CHECK"))
                        {

                        }
                        strg = strg.Replace("\\\r\n", " ");
                        strg = strg.Replace("\\\n", " ");
                        strg = strg.Replace("\\\r", " ");
                        var str = new AntlrInputStream(strg);
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
                        DateTime after = DateTime.Now;
                        System.Console.Error.WriteLine("Time: " + (after - before));
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
                        System.Console.Error.WriteLine("Back on " + this._current_file_name);
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

        public override IParseTree VisitNon_directive([NotNull] SaveParser.Non_directiveContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLparen([NotNull] SaveParser.LparenContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitIdentifier_list([NotNull] SaveParser.Identifier_listContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitReplacement_list([NotNull] SaveParser.Replacement_listContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitPp_tokens([NotNull] SaveParser.Pp_tokensContext context)
        {
            base.VisitPp_tokens(context);
            return null;
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

        private void ParseNumber(string s, out object l)
        {
            s = s.ToLower();
            if (s.EndsWith("ll"))
                s = s.Substring(0, s.Length - 2);
            else if (s.EndsWith("l"))
                s = s.Substring(0, s.Length - 1);
            else if (char.IsDigit(s[s.Length - 1]))
                ;
            else throw new Exception();
            try
            {
                l = int.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = long.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = float.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            try
            {
                l = double.Parse(s);
                return;
            }
            catch (Exception e)
            {
            }
            l = 0;
        }

        public override IParseTree VisitText_line([NotNull] SaveParser.Text_lineContext context)
        {
            throw new Exception("Should not be here");
            if (context.GetText().Contains("QT_VERSION_CHECK"))
            {

            }
            var pp_toks = context.pp_tokens();
            if (pp_toks != null)
            {
                Visit(pp_toks);
            }
            Visit(context.new_line());
            return null;
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
                sb.Append(p3);
            }
            if (replacement == null)
            {
                //sb.Append(tree.GetText());
                var str = TreeOutput.Reconstruct(stream, tree);
                sb.Append(str);
            }
            else
                sb.Append(replacement);
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

        object EvalExpr(string fun, SaveParser.Expression_listContext args)
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
