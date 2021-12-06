using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class ConstantExpressionEvaluator : CPlusPlus14ParserBaseVisitor<IParseTree>
    {
        Dictionary<IParseTree, object> _state = new Dictionary<IParseTree, object>();
        public PreprocessorSymbols _preprocessor_symbols;

        public ConstantExpressionEvaluator(PreprocessorSymbols preprocessor_symbols)
        {
            _preprocessor_symbols = preprocessor_symbols;
        }

        public object Evaluate(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new CPlusPlus14Lexer(str);
            lexer.PushMode(CPlusPlus14Lexer.PP);
            var _tokens = new CommonTokenStream(lexer);
            var parser = new CPlusPlus14Parser(_tokens);
            var listener_lexer = new ErrorListener<int>(true);
            var listener_parser = new ErrorListener<IToken>(true);
            lexer.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);
            var subtree = parser.constant_expression_eof();
            this.Visit(subtree);
            return _state[subtree];
        }

        public override IParseTree VisitLiteral([NotNull] CPlusPlus14Parser.LiteralContext context)
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
                _state[context] = l;
            }
            else if (float_lit != null)
            {
                string s = float_lit.GetText();
                ParseNumber(s, out object l);
                _state[context] = l;
            }
            else if (bool_lit != null)
            {
                string s = bool_lit.GetText();
                try
                {
                    var b = bool.Parse(s);
                    _state[context] = b;
                }
                catch
                {
                    _state[context] = false;
                }
            }
            else if (str_lit != null)
            {
                _state[context] = str_lit.GetText();
            }
            else if (char_lit != null)
            {
                _state[context] = char_lit.GetText();
            }
            else throw new Exception();
            return null;
        }


        // A.4 Expressions   [gram.expr] 

        public override IParseTree VisitPrimary_expression([NotNull] CPlusPlus14Parser.Primary_expressionContext context)
        {
            // primary_expression :  literal |  KWThis |  LeftParen expression RightParen |  id_expression |  lambda_expression |  fold_expression ;
            var literal = context.literal();
            var id_expr = context.id_expression();
            var expr = context.expression();
            var lp = context.LeftParen();
            var kwthis = context.KWThis();
            var lambda = context.lambda_expression();
            //var fold = context.fold_expression();
            if (literal != null)
            {
                Visit(literal);
                _state[context] = _state[literal];
            }
            else if (id_expr != null)
            {
                Visit(id_expr);
                _state[context] = _state[id_expr];
            }
            else if (lp != null)
            {
                var exp = context.expression();
                Visit(exp);
                _state[context] = _state[exp];
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitId_expression([NotNull] CPlusPlus14Parser.Id_expressionContext context)
        {
            // id_expression :  unqualified_id |  qualified_id ;
            var unqual = context.unqualified_id();
            var qual = context.qualified_id();
            if (unqual != null)
            {
                Visit(unqual);
                _state[context] = _state[unqual];
            }
            else if (qual != null)
            {
                Visit(qual);
                _state[context] = _state[qual];
            }
            else throw new NotImplementedException();
            return null;
        }

        public override IParseTree VisitUnqualified_id([NotNull] CPlusPlus14Parser.Unqualified_idContext context)
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
                    _state[context] = v;
                }
                else
                {
                    _state[context] = 1;
                }
            }
            else
                throw new NotImplementedException();
            return null;
        }

        public override IParseTree VisitQualified_id([NotNull] CPlusPlus14Parser.Qualified_idContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNested_name_specifier([NotNull] CPlusPlus14Parser.Nested_name_specifierContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLambda_expression([NotNull] CPlusPlus14Parser.Lambda_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLambda_introducer([NotNull] CPlusPlus14Parser.Lambda_introducerContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLambda_capture([NotNull] CPlusPlus14Parser.Lambda_captureContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitCapture_default([NotNull] CPlusPlus14Parser.Capture_defaultContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitCapture_list([NotNull] CPlusPlus14Parser.Capture_listContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitCapture([NotNull] CPlusPlus14Parser.CaptureContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitSimple_capture([NotNull] CPlusPlus14Parser.Simple_captureContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitInit_capture([NotNull] CPlusPlus14Parser.Init_captureContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitLambda_declarator([NotNull] CPlusPlus14Parser.Lambda_declaratorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitPostfix_expression([NotNull] CPlusPlus14Parser.Postfix_expressionContext context)
        {
            // postfix_expression : (  primary_expression |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ) ( LeftBracket expression RightBracket | LeftBracket braced_init_list RightBracket | LeftParen expression_list ? RightParen | Dot KWTemplate ? id_expression | Arrow KWTemplate ? id_expression | Dot pseudo_destructor_name | Arrow pseudo_destructor_name | PlusPlus | MinusMinus )* ;
            var pri = context.primary_expression();
            var exp = context.expression();
            var exp_list = context.expression_list();
            var simp_type = context.simple_type_specifier();
            var typename = context.typename_specifier();
            if (pri != null)
            {
                Visit(pri);
                _state[context] = _state[pri];
            }
            //else if (post != null && exp_list != null)
            //{
                //Visit(post);
                //Visit(exp_list);
                //// Find post in symbol table.
                //var v = _state[post];
                //// Symbol is actually unevaluated.
                //var s = post.GetText();
                ////var s = v.ToString();
                //_state[context] = EvalExpr(s, exp_list);
            //}
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitExpression_list([NotNull] CPlusPlus14Parser.Expression_listContext context)
        {
            // expression_list :  initializer_list ;
            var init_list = context.initializer_list();
            Visit(init_list);
            _state[context] = _state[init_list];
            return null;
        }

        public override IParseTree VisitPseudo_destructor_name([NotNull] CPlusPlus14Parser.Pseudo_destructor_nameContext context)
        {
            throw new NotImplementedException();
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
                _state[context] = _state[post];
            }
            else if (unary != null && cast != null)
            {
                if (unary.GetText() == "defined")
                {
                    if (cast?.unary_expression()?.postfix_expression()?.primary_expression()?.LeftParen() != null)
                    {
                        var variable = cast?.unary_expression()?.postfix_expression()?.primary_expression()?.expression()?.GetText();
                        _state[context] = _preprocessor_symbols.IsDefined(variable);
                    }
                    else
                    {
                        var variable = cast.GetText();
                        _state[context] = _preprocessor_symbols.IsDefined(variable);
                    }
                }
                else if (unary.GetText() == "!")
                {
                    Visit(unary);
                    Visit(cast);
                    var v = _state[cast];
                    ConvertToBool(v, out bool b);
                    _state[context] = !b;
                }
                else throw new Exception();
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitNew_expression([NotNull] CPlusPlus14Parser.New_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_placement([NotNull] CPlusPlus14Parser.New_placementContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_type_id([NotNull] CPlusPlus14Parser.New_type_idContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_declarator([NotNull] CPlusPlus14Parser.New_declaratorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNoptr_abstract_declarator([NotNull] CPlusPlus14Parser.Noptr_abstract_declaratorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNew_initializer([NotNull] CPlusPlus14Parser.New_initializerContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitDelete_expression([NotNull] CPlusPlus14Parser.Delete_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitNoexcept_expression([NotNull] CPlusPlus14Parser.Noexcept_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitCast_expression([NotNull] CPlusPlus14Parser.Cast_expressionContext context)
        {
            // cast_expression :  unary_expression |  LeftParen type_id RightParen cast_expression ;
            if (context.unary_expression() != null)
            {
                var unary = context.unary_expression();
                Visit(unary);
                _state[context] = _state[unary];
            }
            else throw new Exception();
            return null;
        }

        public override IParseTree VisitPm_expression([NotNull] CPlusPlus14Parser.Pm_expressionContext context)
        {
            // pm_expression :  cast_expression |  pm_expression DotStar cast_expression |  pm_expression ArrowStar cast_expression ;
            // pm_expression :  cast_expression ( DotStar cast_expression | ArrowStar cast_expression )* ;
            var casts = context.cast_expression();
            //var pm = context.pm_expression();
            //if (pm != null)
            //{
            //    Visit(pm);
            //    var v = _state[pm];
            //    var l = (int)v;
            //    Visit(cast);
            //    var v2 = (int)_state[cast];
            //    if (context.DotStar() != null)
            //    {
            //        _state[context] = l * v2;
            //    }
            //    else if (context.ArrowStar() != null)
            //    {
            //        _state[context] = l / v2;
            //    }
            //}
            //else
            {
                if (casts.Count() > 1) throw new Exception();
                var cast = casts[0];
                Visit(cast);
                _state[context] = _state[cast];
            }
            return null;
        }

        public override IParseTree VisitMultiplicative_expression([NotNull] CPlusPlus14Parser.Multiplicative_expressionContext context)
        {
            // multiplicative_expression :  pm_expression ( Star pm_expression | Div pm_expression | Mod pm_expression )* ;
            var pms = context.pm_expression();
            //var mul = context.multiplicative_expression();
            //if (mul != null)
            //{
            //    Visit(mul);
            //    var v = _state[mul];
            //    var l = (int)v;
            //    Visit(pm);
            //    var v2 = (int)_state[pm];
            //    if (context.Star() != null)
            //    {
            //        _state[context] = l * v2;
            //    }
            //    else if (context.Div() != null)
            //    {
            //        _state[context] = l / v2;
            //    }
            //    else if (context.Mod() != null)
            //    {
            //        _state[context] = l % v2;
            //    }
            //}
            //else
            {
                if (pms.Count() > 1) throw new Exception();
                var pm = pms[0];
                Visit(pm);
                _state[context] = _state[pm];
            }
            return null;
        }

        public override IParseTree VisitAdditive_expression([NotNull] CPlusPlus14Parser.Additive_expressionContext context)
        {
            // additive_expression :  multiplicative_expression |  additive_expression Plus multiplicative_expression |  additive_expression Minus multiplicative_expression ;
            var muls = context.multiplicative_expression();
            //var plus = context.Plus();
            //if (plus != null)
            //{
            //    Visit(mul[0]);
            //    var lhs_v = _state[mul[0]];
            //    ParseNumber(lhs_v.ToString(), out object lhs_n);
            //    var rhs_v = _state[mul[1]];
            //    ParseNumber(rhs_v.ToString(), out object rhs_n);
            //    if (lhs_n is int && rhs_n is int)
            //    {
            //        int lhs = (int)lhs_n;
            //        int rhs = (int)rhs_n;
            //        int res = plus != null ? lhs + rhs : lhs - rhs;
            //        _state[context] = res;
            //    }
            //    else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
            //    {
            //        long lhs = (long)lhs_n;
            //        long rhs = (long)rhs_n;
            //        long res = plus != null ? lhs + rhs : lhs - rhs;
            //        _state[context] = res;
            //    }
            //    else throw new Exception();
            //}
            //else
            {
                if (muls.Count() > 1) throw new Exception();
                var mul = muls[0];
                Visit(mul);
                _state[context] = _state[mul];
            }
            return null;
        }

        public override IParseTree VisitShift_expression([NotNull] CPlusPlus14Parser.Shift_expressionContext context)
        {
            // shift_expression :  additive_expression |  shift_expression LeftShift additive_expression |  shift_expression RightShift additive_expression ;
            // shift_expression :  additive_expression ( LeftShift additive_expression | Greater Greater additive_expression )* ;
            var adds = context.additive_expression();
            //var shift = context.Greater();
            //if (shift != null)
            //{
            //    Visit(add[0]);
            //    var lhs_v = _state[add[0]];
            //    ParseNumber(lhs_v.ToString(), out object lhs_n);
            //    var rhs_v = _state[add[1]];
            //    ParseNumber(rhs_v.ToString(), out object rhs_n);
            //    if (lhs_n is int && rhs_n is int)
            //    {
            //        int lhs = (int)lhs_n;
            //        int rhs = (int)rhs_n;
            //        int res = context.LeftShift() != null ? lhs << rhs : lhs >> rhs;
            //        _state[context] = res;
            //    }
            //    else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
            //    {
            //        long lhs = (long)lhs_n;
            //        int rhs = (int)rhs_n;
            //        long res = context.LeftShift() != null ? lhs << rhs : lhs >> rhs;
            //        _state[context] = res;
            //    }
            //    else throw new Exception();
            //}
            //else
            {
                if (adds.Count() > 1) throw new Exception();
                var add = adds[0];
                Visit(add);
                _state[context] = _state[add];
            }
            return null;
        }

        public override IParseTree VisitRelational_expression([NotNull] CPlusPlus14Parser.Relational_expressionContext context)
        {
            // relational_expression :  shift_expression ( Less shift_expression | Greater shift_expression | LessEqual shift_expression | GreaterEqual shift_expression )* ;
            var shifts = context.shift_expression();
            var shift = shifts[0];
            Visit(shift);
            _state[context] = _state[shift];
            var previous = shift;
            for (int i = 1; i < context.children.Count(); i += 2)
            {
                var lhs_v = _state[context];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var op = context.children[i];
                var opt = op as TerminalNodeImpl;
                var rhs = context.children[i + 1];
                Visit(rhs);
                var rhs_v = _state[rhs];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int l = (int)lhs_n;
                    int r = (int)rhs_n;
                    bool res = false;
                    if (opt.Symbol.Type == CPlusPlus14Lexer.Less) res = l < r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.Greater) res = l > r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.LessEqual) res = l < r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.GreaterEqual) res = l > r;
                    _state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long l = (long)lhs_n;
                    long r = (long)rhs_n;
                    bool res = false;
                    if (opt.Symbol.Type == CPlusPlus14Lexer.Less) res = l < r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.Greater) res = l > r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.LessEqual) res = l < r;
                    else if (opt.Symbol.Type == CPlusPlus14Lexer.GreaterEqual) res = l > r;
                    _state[context] = res;
                }
                else throw new Exception();
            }
            return null;
        }

        public override IParseTree VisitEquality_expression([NotNull] CPlusPlus14Parser.Equality_expressionContext context)
        {
            // equality_expression :  relational_expression ( Equal relational_expression | NotEqual relational_expression )* ;
            var rels = context.relational_expression();
            //var eq = context.equality_expression();
            object l = null;
            //if (eq != null)
            //{
            //    Visit(eq);
            //    var v = _state[eq];
            //    l = v == null ? null : v;
            //    Visit(rel);
            //    var v2 = _state[rel];
            //    _state[context] = v == v2;
            //}
            //else
            {
                if (rels.Count() > 1) throw new Exception();
                var rel = rels[0];
                Visit(rel);
                _state[context] = _state[rel];
            }
            return null;
        }

        public override IParseTree VisitAnd_expression([NotNull] CPlusPlus14Parser.And_expressionContext context)
        {
            // and_expression :  equality_expression |  and_expression ( And | KWBitAnd ) equality_expression ;
            var eqs = context.equality_expression();
            //if (and != null)
            //{
            //    Visit(and);
            //    Visit(eq);
            //    var lhs_v = _state[and];
            //    ParseNumber(lhs_v.ToString(), out object lhs_n);
            //    var rhs_v = _state[eq];
            //    ParseNumber(rhs_v.ToString(), out object rhs_n);
            //    if (lhs_n is int && rhs_n is int)
            //    {
            //        int lhs = (int)lhs_n;
            //        int rhs = (int)rhs_n;
            //        int res = lhs & rhs;
            //        _state[context] = res;
            //    }
            //    else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
            //    {
            //        long lhs = (long)lhs_n;
            //        long rhs = (long)rhs_n;
            //        long res = lhs & rhs;
            //        _state[context] = res;
            //    }
            //    else throw new Exception();
            //}
            //else
            {
                if (eqs.Count() > 1) throw new Exception();
                var eq = eqs[0];
                Visit(eq);
                _state[context] = _state[eq];
            }
            return null;
        }

        public override IParseTree VisitExclusive_or_expression([NotNull] CPlusPlus14Parser.Exclusive_or_expressionContext context)
        {
            // exclusive_or_expression :  and_expression |  exclusive_or_expression ( Caret | KWXor ) and_expression ;
            var ands = context.and_expression();
            //var xor = context.exclusive_or_expression();
            //if (xor != null)
            //{
            //    Visit(xor);
            //    Visit(and);
            //    var lhs_v = _state[xor];
            //    ParseNumber(lhs_v.ToString(), out object lhs_n);
            //    var rhs_v = _state[and];
            //    ParseNumber(rhs_v.ToString(), out object rhs_n);
            //    if (lhs_n is int && rhs_n is int)
            //    {
            //        int lhs = (int)lhs_n;
            //        int rhs = (int)rhs_n;
            //        int res = lhs ^ rhs;
            //        _state[context] = res;
            //    }
            //    else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
            //    {
            //        long lhs = (long)lhs_n;
            //        long rhs = (long)rhs_n;
            //        long res = lhs ^ rhs;
            //        _state[context] = res;
            //    }
            //    else throw new Exception();
            //}
            //else
            {
                if (ands.Count() > 1) throw new Exception();
                var and = ands[0];
                Visit(and);
                _state[context] = _state[and];
            }
            return null;
        }

        public override IParseTree VisitInclusive_or_expression([NotNull] CPlusPlus14Parser.Inclusive_or_expressionContext context)
        {
            // inclusive_or_expression :  exclusive_or_expression ( ( Or | KWBitOr ) exclusive_or_expression )* ;
            var xors = context.exclusive_or_expression();
            var xor = xors[0];
            Visit(xor);
            _state[context] = _state[xor];
            var previous = xor;
            for (int i = 1; i < context.children.Count(); i += 2)
            {
                var lhs_v = _state[context];
                ParseNumber(lhs_v.ToString(), out object lhs_n);
                var op = context.children[i];
                var opt = op as TerminalNodeImpl;
                var rhs = context.children[i + 1];
                Visit(rhs);
                var rhs_v = _state[rhs];
                ParseNumber(rhs_v.ToString(), out object rhs_n);
                if (lhs_n is int && rhs_n is int)
                {
                    int l = (int)lhs_n;
                    int r = (int)rhs_n;
                    int res = l | r;
                    _state[context] = res;
                }
                else if ((lhs_n is long || lhs_n is int) && (rhs_n is int || rhs_n is long))
                {
                    long l = (long)lhs_n;
                    long r = (long)rhs_n;
                    long res = l | r;
                    _state[context] = res;
                }
                else throw new Exception();
            }
            return null;
        }

        public override IParseTree VisitLogical_and_expression([NotNull] CPlusPlus14Parser.Logical_and_expressionContext context)
        {
            // logical_and_expression :  inclusive_or_expression ( ( AndAnd | KWAnd ) inclusive_or_expression )* ;
            var iors = context.inclusive_or_expression();
            var ior = iors[0];
            Visit(ior);
            ConvertToBool(_state[ior], out bool b);
            _state[context] = b;
            for (int i = 1; i < context.children.Count(); i += 2)
            {
                if (!(bool)_state[context]) break;
                var op = context.children[i];
                var opt = op as TerminalNodeImpl;
                if (opt.Symbol.Type == CPlusPlus14Parser.AndAnd || opt.Symbol.Type == CPlusPlus14Parser.KWAnd)
                {
                    var next = context.children[i + 1];
                    Visit(next);
                    var v = _state[next];
                    ConvertToBool(v, out bool bnext);
                    if (! bnext)
                    {
                        _state[context] = bnext;
                    }
                }
            }
            return null;
        }

        public override IParseTree VisitLogical_or_expression([NotNull] CPlusPlus14Parser.Logical_or_expressionContext context)
        {
            // logical_or_expression :  logical_and_expression ( ( OrOr | KWOr ) logical_and_expression )* ;
            var ands = context.logical_and_expression();
            var and = ands[0];
            Visit(and);
            ConvertToBool(_state[and], out bool b);
            _state[context] = b;
            for (int i = 1; i < context.children.Count(); i += 2)
            {
                if ((bool)_state[context]) break;
                var op = context.children[i];
                var opt = op as TerminalNodeImpl;
                if (opt.Symbol.Type == CPlusPlus14Parser.OrOr || opt.Symbol.Type == CPlusPlus14Parser.KWOr)
                {
                    var next = context.children[i + 1];
                    Visit(next);
                    var v = _state[next];
                    ConvertToBool(v, out bool bnext);
                    if (bnext)
                    {
                        _state[context] = bnext;
                        break;
                    }
                }
            }
            return null;
        }

        public override IParseTree VisitConditional_expression([NotNull] CPlusPlus14Parser.Conditional_expressionContext context)
        {
            // conditional_expression :  logical_or_expression |  logical_or_expression Question expression Colon assignment_expression ;
            var lor = context.logical_or_expression();
            var exp = context.expression();
            var aexp = context.assignment_expression();
            if (context.Question() == null)
            {
                Visit(lor);
                _state[context] = _state[lor];
            }
            else
            {
                Visit(lor);
                var v = _state[lor];
                ConvertToBool(v, out bool b);
                if (b)
                {
                    Visit(exp);
                    _state[context] = _state[exp];
                }
                else
                {
                    Visit(aexp);
                    _state[context] = _state[aexp];
                }
            }
            return null;
        }

        public override IParseTree VisitThrow_expression([NotNull] CPlusPlus14Parser.Throw_expressionContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitAssignment_expression([NotNull] CPlusPlus14Parser.Assignment_expressionContext context)
        {
            var first = context.conditional_expression();
            var thrw = context.throw_expression();
            if (first != null)
            {
                Visit(first);
                _state[context] = _state[first];
            }
            else if (thrw != null)
            {
                Visit(thrw);
                _state[context] = _state[thrw];
            }
            else
            {
                //logical_or_expression
                //assignment_operator
                var clause = context.initializer_clause();
                Visit(clause);
                _state[context] = _state[clause];
            }
            return null;
        }

        public override IParseTree VisitAssignment_operator([NotNull] CPlusPlus14Parser.Assignment_operatorContext context)
        {
            throw new NotImplementedException();
        }

        public override IParseTree VisitExpression([NotNull] CPlusPlus14Parser.ExpressionContext context)
        {
            var assignments = context.assignment_expression();
            if (assignments.Count() > 1) throw new Exception();
            var assignment = assignments[0];
            Visit(assignment);
            _state[context] = _state[assignment];
            return null;
        }

        public override IParseTree VisitConstant_expression([NotNull] CPlusPlus14Parser.Constant_expressionContext context)
        {
            var child = context.conditional_expression();
            Visit(child);
            _state[context] = _state[child];
            return null;
        }

        public override IParseTree VisitConstant_expression_eof([NotNull] CPlusPlus14Parser.Constant_expression_eofContext context)
        {
            // constant_expression_eof :  conditional_expression EOF ;
            var cond = context.conditional_expression();
            VisitConditional_expression(cond);
            _state[context] = _state[cond];
            return null;
        }

        public override IParseTree VisitInitializer_clause([NotNull] CPlusPlus14Parser.Initializer_clauseContext context)
        {
            // initializer_clause :  assignment_expression |  braced_init_list ;
            var assign = context.assignment_expression();
            var brace = context.braced_init_list();
            if (assign != null)
            {
                Visit(assign);
                var v = _state[assign];
                _state[context] = v;
            }
            else
            {
                Visit(brace);
                var v = _state[brace];
                _state[context] = v;
            }
            return null;
        }

        public override IParseTree VisitInitializer_list([NotNull] CPlusPlus14Parser.Initializer_listContext context)
        {
            // initializer_list :  initializer_clause Ellipsis ? ( Comma initializer_clause Ellipsis ? )* ;
            var init_clauses = context.initializer_clause();
            var init_states = new List<object>();
            foreach (var ic in init_clauses)
            {
                Visit(ic);
                init_states.Add(_state[ic]);
            }
            _state[context] = init_states;
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
            try
            {
                l = Convert.ToInt32(s, 16);
                return;
            }
            catch (Exception)
            {
            }
            try
            {
                l = Convert.ToInt64(s, 16);
                return;
            }
            catch (Exception)
            {
            }
            try
            {
                if (s[0] == '0')
                {
                    l = Convert.ToInt32(s, 8);
                    return;
                }
            }
            catch (Exception)
            {
            }
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
            try
            {
                l = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
                return;
            }
            catch (Exception)
            {
            }
            throw new Exception("Trying to parse number ouf of '" + s + "' -- can't.");
        }

        object EvalExpr(string fun, CPlusPlus14Parser.Expression_listContext args)
        {
            if (this._preprocessor_symbols.Find(
                fun,
                out List<string> ids,
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
                        if (xxx.Contains("\n"))
                        { }
                        eval.Append(" " + xxx);
                    }
                    else
                    {
                        if (value.Contains("\n"))
                        { }
                        eval.Append(" " + value);
                    }
                }

                // Reparse and call recursively until fix-point.
                var todo = eval.ToString();
                do
                {
                    throw new Exception();
                } while (true);
                //return todo;
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
}
