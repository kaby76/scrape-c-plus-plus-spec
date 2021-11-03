// Notes on open-source C++
// https://devblogs.microsoft.com/cppblog/vcpkg-a-tool-to-acquire-and-build-c-open-source-libraries-on-windows/
// https://github.com/microsoft/vcpkg

using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace scrape_pdf
{
    class Program
    {
        static string GetTextFromPDF(string src_file_name)
        {
            StringBuilder text = new StringBuilder();
//          string src = @"n4296.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src_file_name));
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var pdfPage = pdfDoc.GetPage(i);
                    text.Append(PdfTextExtractor.GetTextFromPage(pdfPage, new MySimpleTextExtractionStrategy()));
                }
            }
            return text.ToString();
        }

        static int Find(int cursor, string pdfText)
        {
            var regex = new Regex(@"[\r\n]([\w-]+[:]|[A][.])");
            var match = regex.Match(pdfText, cursor-1);
            if (match.Success)
            {
                var result = match.Index;
                for (; ; result++) if (!(pdfText[result] == '\r' || pdfText[result] == '\n')) break;
                return result;
            }
            else return -1;
        }

        static bool ebnf = false;

        static void Main(string[] args)
        {
            var result = new StringBuilder();

            string src_file_name = args[0];
            var just_fn = System.IO.Path.GetFileName(src_file_name);
            string pdfText = GetTextFromPDF(src_file_name);

            if (!ebnf)
            {
//                result.AppendLine("grammar " + Antlrize(System.IO.Path.GetFileNameWithoutExtension(src_file_name)) + ";");
                result.AppendLine("grammar Scrape;");
            }

            // Let's start parsing the spec text and extracting the
            // rules for C++.

            // First bracket the Annex A section.
            int annex_a = pdfText.IndexOf("Annex A (informative)");
            int annex_b = just_fn switch
            {
                "n4296.pdf" => pdfText.IndexOf("Annex B (informative)"),
                "n4660.pdf" => pdfText.IndexOf("Annex B (informative)"),
                "n4878.pdf" => pdfText.IndexOf("Annex B (normative)"),
                _ => -1
            };
            var cursor = annex_a;
            int after = cursor + "Annex A (informative)".Length;
            cursor = after;

            // Now start looking for rules or section headers.
            for (; cursor < annex_b; )
            {
                // Look for start of section or beginning of rule.
                int first = Find(cursor, pdfText);
                if (first >= annex_b) break;

                // If section header, output that and restart.
                if (pdfText.Substring(first).StartsWith("A."))
                {
                    cursor = pdfText.IndexOf('\n', first);
                    result.AppendLine();
                    result.AppendLine("// " + pdfText.Substring(first, cursor - first));
                    continue;
                }

                // Starting at a rule...
                cursor = pdfText.IndexOf(':', first) + 1;
                var cursor_on = pdfText.Substring(cursor);
                var lhs = pdfText.Substring(first, cursor - 1 - first);
                if (ebnf)
                    result.Append(Antlrize(lhs) + " ::= ");
                else
                    result.Append(Antlrize(lhs) + " : ");

                // Grab one line at a time and examine.
                var next_first = Find(cursor, pdfText);
                next_first = next_first >= 0 ? next_first : annex_b;
                var next_lhs_on = pdfText.Substring(next_first);
                bool first_time = true;
                bool one_of = false;
                bool term = true;
                for (; ; )
                {
                    var start = cursor;
                    var hereon = pdfText.Substring(start);
                    if (start >= next_first) break;
                    var end = pdfText.IndexOf('\n', start);
                    if (end < 0) break;
                    if (end >= annex_b) break;
                    if (end > next_first) break;
                    var len = end - start;
                    if (len <= 0)
                    {
                        cursor = end + 1;
                        continue;
                    }
                    var rhs = pdfText.Substring(start, len);
                    rhs = rhs.Trim();
                    cursor = end + 1;
                    if (rhs == "") continue;
                    // Special cases that need to be delt with.
                    if (rhs.StartsWith("Note that a typedef-name naming")) continue;
                    if (rhs.StartsWith("any member of the source character set except new-line and >"))
                    {
                    }
                    if (rhs.StartsWith("§"))
                    {
                        term = false;
                        if (ebnf)
                            result.AppendLine();
                        else
                            result.AppendLine(" ;");
                        result.AppendLine("// " + rhs);
                        result.AppendLine();
                        break;
                    }
                    if (rhs.StartsWith("A."))
                    {
                        term = false;
                        if (ebnf)
                            result.AppendLine();
                        else
                            result.AppendLine(" ;");
                        result.AppendLine();
                        result.AppendLine("// " + rhs);
                        break;
                    }
                    if (first_time)
                    {
                        one_of = one_of || rhs == "one of";
                        if (rhs == "one of")
                        {
                            continue;
                        }
                    }
                    if (!first_time && !one_of) result.Append(" | ");
                    rhs = rhs.Replace("opt'", "opt '");
                    var ss = rhs.Split(' ')
                        .Select(s => s.Trim())
                        .Where(s => s != "")
                        .ToList();
                    foreach (var s in ss)
                    {
                        var r = s;
                        if (r == "N4296") continue;
                        if (one_of) { r = (first_time ? "" : "| ") + Quotify(r); }
                        else if (r == "opt") r = "?";
                        else if (!IsName(r)) r = Quotify(r);
                        else if (IsName(r)) r = Antlrize(r);
                        first_time = false;
                        result.Append(" " + r);
                    }
                }
                if (term)
                {
                    if (ebnf)
                        result.AppendLine();
                    else
                        result.AppendLine(" ;");
                }
            }

//            result.AppendLine(@"keyword " + (ebnf ? "::=" : ":") + @" 'alignas' | 'continue' | 'friend' | 'register' | 'true' 
//                'alignof' | 'decltype' | 'goto' | 'reinterpret_cast' | 'try'
//                'asm' | 'default' | 'if' | 'return' | 'typedef'
//                'auto' | 'delete' | 'inline' | 'short' | 'typeid'
//                'bool' | 'do' | 'int' | 'signed' | 'typename'
//                'break' | 'double' | 'long' | 'sizeof' | 'union'
//                'case' | 'dynamic_cast' | 'mutable' | 'static' | 'unsigned'
//                'catch' | 'else' | 'namespace' | 'static_assert' | 'using'
//                'char' | 'enum' | 'new' | 'static_cast' | 'virtual'
//                'char16_t' | 'explicit' | 'noexcept' | 'struct' | 'void'
//                'char32_t' | 'export' | 'nullptr' | 'switch' | 'volatile'
//                'class' | 'extern' | 'operator' | 'template' | 'wchar_t'
//                'const' | 'false' | 'private' | 'this' | 'while'
//                'constexpr' | 'float' | 'protected' | 'thread_local'
//                'const_cast' | 'for' | 'public' | 'throw'
//                'and' | 'and_eq' | 'bitand' | 'bitor' | 'compl' | 'not'
//                'not_eq' | 'or' | 'or_eq' | 'xor' | 'xor_eq'" + (ebnf ? "" : " ;"));
//            result.AppendLine(@"punctuator " + (ebnf ? "::=" : ":") + " preprocessing_op_or_punc" + (ebnf ? "" : " ;"));
//            result.AppendLine(@"WS : [\n\r\t ]+ -> channel(HIDDEN);
//COMMENT : '//' ~[\n\r]* -> channel(HIDDEN);
//ML_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
//Prep : '#' ~[\n\r]* -> channel(HIDDEN);");

            // Fix ups. Here we are only concerned about single character (more or less)
            // permutations from the Spec in to bring it into a more or less proper
            // grammar. It does not comment out useless productions, replace textual
            // descriptions in the grammar that must be replaced with proper lexer
            // syntax, etc.
            //
            // Note, the order of these corrections are in the order that the rules
            // appear in the Spec.
            var output = result.ToString();

            // General changes.

            FixupOutput(ref output, "ﬁ", "fi",
                just_fn switch
                {
                    "n4296.pdf" => 319,
                    "n4660.pdf" => 339,
                    "n4878.pdf" => 364,
                    _ => 1
                }); // Wrong OCR of "identifier" throughout the text.
            FixupOutput(ref output, "'’'", "'\\''",
                just_fn switch
                {
                    "n4296.pdf" => 3,
                    "n4660.pdf" => 4,
                    "n4878.pdf" => 5,
                    _ => 1
                }); // Wrong OCR of single quote in numerous locations.
            FixupOutput(ref output, "'ˆ", "'^",
                just_fn switch
                {
                    "n4296.pdf" => 7,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // Wrong OCR of caret.
            Regex opt = new Regex("(?<id>[a-zA-Z_])opt(?![a-zA-Z])");
            output = opt.Replace(output, "${id} ?");
            output = output.Replace(@"'opt)'", @"? ')'");
            output = output.Replace(@"'opt='", @"? '='");
            output = output.Replace(@"'opt:'", @"? ':'");
            output = output.Replace(@"'opt;'", @"? ';'");
            output = output.Replace(@"'opt{'", @"? '{'");
            output = output.Replace(@"'opt}'", @"? '}'");
            output = output.Replace(@"'opt]'", @"? ']'");
            output = output.Replace(@"'opt>'", @"? '>'");
            output = output.Replace(@"'...opt'", @" '...' ?");
            output = output.Replace(@"'::optnew'", @"'::' ? 'new'");
            output = output.Replace(@"'::opt'", @"'::' ?");

            // Section 2

            FixupOutput(ref output,
                @"each non_white_space character that cannot be one of the above",
                @"'each non_white_space character that cannot be one of the above'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // preprocessing_token
            FixupOutput(ref output,
                @"each non_whitespace character that cannot be one of the above",
                @"'each non_whitespace character that cannot be one of the above'",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                }); // preprocessing_token

            FixupOutput(ref output, @"any member of the source character set except new_line and '>'",
                @"'any member of the source character set except new_line and >'");
            FixupOutput(ref output, @"any member of the source character set except new_line and '""'",
                @"'any member of the source character set except new_line and ""'");
            FixupOutput(ref output, @"other implementation_defined characters",
                @"'other implementation_defined characters'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
//            FixupOutput(ref output, "pp_number 'E' sign |  pp_number '.' |  'identifier:' | ",
//                @"pp_number 'E' sign |  pp_number '.';
//identifier : ");
//            FixupOutput(ref output, "identifier digit |  'identifier-nondigit:' |",
//                @"identifier digit ;
//identifier_nondigit : ");
            FixupOutput(ref output, "ﬂ", "fl",
                just_fn switch
                {
                    "n4296.pdf" => 7,
                    "n4660.pdf" => 13,
                    "n4878.pdf" => 13,
                    _ => 1
                }); // Fix ﬂoating_literal in literal rule.
            FixupOutput(ref output, "_suﬃx ?", "_suffix ?",
                just_fn switch
                {
                    "n4296.pdf" => 10,
                    "n4660.pdf" => 12,
                    "n4878.pdf" => 14,
                    _ => 1
                }); // Fix integer-literal rule.
            FixupOutput(ref output, "'’opt'", "'\\'' ?", 5); // Fix binary-literal rule.
            FixupOutput(ref output, "suﬃx", "suffix",
                just_fn switch
                {
                    "n4296.pdf" => 18,
                    "n4660.pdf" => 20,
                    "n4878.pdf" => 23,
                    _ => 1
                }); // Numerous locations.
//            FixupOutput(ref output, "| 'integer-suffix:' | 'unsigned-suffix' | 'long-suffixopt' | 'unsigned-suffix' | 'long-long-suffixopt' | 'long-suffix' | 'unsigned-suffixopt' | 'long-long-suffix' | 'unsigned-suffixopt' ;",
//                @";
//integer_suffix : unsigned_suffix long_suffix ? | unsigned_suffix long_long_suffix ? | long_suffix unsigned_suffix ? | long_long_suffix unsigned_suffix ? ;"); // Entire integer-suffix rule messed up.
            FixupOutput(ref output, @"'encoding-prefix ?’'", @"encoding_prefix ? '\''",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // Fix character-literal
            FixupOutput(ref output, @"c_char :  any member of the source character set except |  the single_quote '’,' backslash '\\,' or new_line character",
                @"c_char :  'any member of the source character set except the single_quote \', backslash \\, or new_line character'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"basic_c_char :  any member of the basic source character set except the single_quote '’,' backslash '\\,' or new_line character ;",
                                    @"basic_c_char :  'any member of the basic source character set except the single_quote \', backslash \\, or new_line character ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"'\\’'", @"'\\\''",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // Fix simple-escape-sequence.
            FixupOutput(ref output, @"'digit-sequence ?.'", @"digit_sequence ? '.'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // fractional_constant
            FixupOutput(ref output, @"string_literal :  'encoding-prefix ?""' 's-char-sequence ?""' |  encoding_prefixoptR raw_string ;",
                @"string_literal :  encoding_prefix ? '""' s_char_sequence ? '""' |  encoding_prefix ? 'R' raw_string ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"s_char :  any member of the source character set except |  the double_quote '"",' backslash '\\,' or new_line character |  escape_sequence |  universal_character_name ;",
                @"s_char :  'any member of the source character set except the double_quote "", backslash \\. or new_line character' | escape_sequence | universal_character_name ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"basic_s_char :  any member of the basic source character set except the double_quote '"",' backslash '\\,' or new_line character ;",
                @"s_char :  'any member of the basic source character set except the double_quote "", backslash \\. or new_line character' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"raw_string :  '""' 'd-char-sequence ?(' 'r-char-sequence ?)' 'd-char-sequence ?""' ;",
                @"raw_string :  '""' d_char_sequence ? '(' r_char_sequence ? ')' d_char_sequence ? '""' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"r_char :  any member of the source character 'set,' except |  a right parenthesis ')' followed by the initial d_char_sequence |  '(which' may be 'empty)' followed by a 'double' quote '"".' ;",
                @"r_char :  'any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote "".' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"r_char :  any member of the source character 'set,' except a right parenthesis ')' followed by |  the initial d_char_sequence '(which' may be 'empty)' followed by a 'double' quote '"".' ;",
                                    @"r_char :  'any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote "".' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                });


            FixupOutput(ref output, @"d_char :  any member of the basic source character set 'except:' |  'space,' the left parenthesis '(,' the right parenthesis '),' the backslash '\\,' |  and the control characters representing horizontal 'tab,' |  vertical 'tab,' form 'feed,' and 'newline.' ;",
                @"d_char :  'any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"d_char :  any member of the basic source character set 'except:' |  'space,' the left parenthesis '(,' the right parenthesis '),' the backslash '\\,' and the control characters |  representing horizontal 'tab,' vertical 'tab,' form 'feed,' and 'newline.' ;",
                                    @"d_char :  any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                });

            // Section 3

            //output = output.Replace(@"translation_unit :  declaration_seqopt ;",
            //    @"translation_unit :  declaration_seq ? ;");

            // Section 4

            FixupOutput(ref output, @"'new-placement ?('",
                @"new_placement ? '('",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_introducer :  '[' 'lambda-capture ?]' ;",
                @"lambda_introducer :  '[' lambda_capture ? ']' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' mutable ? |  exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;",
                @"lambda_declarator :  '(' parameter_declaration_clause ')' 'mutable' ? exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? |  noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? |  noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? requires_clause ? ;",
                                    @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? requires_clause ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"'expression-list ?)'",
                @"expression_list ? ')'",
                just_fn switch
                {
                    "n4296.pdf" => 5,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // postfix_expression
            FixupOutput(ref output, @"'->' template ?",
                @"'->' 'template' ?");
            FixupOutput(ref output, @"'.' template ?",
                @"'.' 'template' ?");
            FixupOutput(ref output, @"'expression ?;'",
                @"expression ? ';'",
                just_fn switch
                {
                    "n4296.pdf" => 2,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"exclusive_or_expression :  and_expression |  exclusive_or_expression ˆ and_expression ;",
                @"exclusive_or_expression :  and_expression |  exclusive_or_expression '^' and_expression ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"'logical-or-expression||'",
                @"logical_or_expression '||'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"attribute_specifier_seqoptcase",
                @"attribute_specifier_seq ? 'case'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                }); // labeled_statement
            FixupOutput(ref output, @"attribute_specifier_seqoptdefault",
                @"attribute_specifier_seq ? 'default'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"qualified_id :  nested_name_specifier template ? unqualified_id ;",
                @"qualified_id :  nested_name_specifier 'template' ? unqualified_id ;");
            FixupOutput(ref output, @"nested_name_specifier :  '::' |  type_name '::' |  namespace_name '::' |  decltype_specifier '::' |  nested_name_specifier identifier '::' |  nested_name_specifier template ? simple_template_id '::' ;",
                @"nested_name_specifier :  '::' |  type_name '::' |  namespace_name '::' |  decltype_specifier '::' |  nested_name_specifier identifier '::' |  nested_name_specifier 'template' ? simple_template_id '::' ;");
            FixupOutput(ref output, @"new_expression :  '::' ? new new_placement ? new_type_id new_initializer ? |  '::' ? new new_placement ? '(' type_id ')' new_initializer ? ;",
                @"new_expression :  '::' ? 'new' new_placement ? new_type_id new_initializer ? |  '::' ? 'new' new_placement ? '(' type_id ')' new_initializer ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"delete_expression :  '::optdelete' cast_expression |  '::optdelete' '[' ']' cast_expression ;",
                @"delete_expression :  '::' ? 'delete' cast_expression |  '::' ? 'delete' '[' ']' cast_expression ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 5

            FixupOutput(ref output, @"compound_statement :  '{' 'statement-seq ?}' ;",
                @"compound_statement :  '{' statement_seq ? '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"selection_statement :  'if' constexpr ? '(' init_statement ? condition ')' statement |  'if' constexpr ? '(' init_statement ? condition ')' statement 'else' statement |  'switch' '(' init_statement ? condition ')' statement ;",
                                    @"selection_statement :  'if' 'constexpr' ? '(' init_statement ? condition ')' statement |  'if' 'constexpr' ? '(' init_statement ? condition ')' statement 'else' statement |  'switch' '(' init_statement ? condition ')' statement ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"iteration_statement :  'while' '(' condition ')' statement |  'do' statement 'while' '(' expression ')' ';' |  'for' '(' for_init_statement 'condition ?;' 'expression ?)' statement |  'for' '(' for_range_declaration ':' for_range_initializer ')' statement ;",
                @"iteration_statement :  'while' '(' condition ')' statement |  'do' statement 'while' '(' expression ')' ';' |  'for' '(' for_init_statement condition ? ';' expression ? ')' statement |  'for' '(' for_range_declaration ':' for_range_initializer ')' statement ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            //FixupOutput(ref output, @"jump_statement :  'break' ';' |  'continue' ';' |  'return' expression ';' |  'return' braced_init_list ';' |  'goto' identifier ';' ;",
            //                        @"jump_statement :  'break' ';' |  'continue' ';' |  'return' expression ? ';' |  'return' braced_init_list ';' |  'goto' identifier ';' ;");
            //FixupOutput(ref output, @"'static_assert-declaration'",
            //    @"static_assert_declaration");
            FixupOutput(ref output, @"'attribute-specifier-seq ?='",
                @"attribute_specifier_seq ? '='",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"'init-declarator-list ?;'",
                @"init_declarator_list ? ';'",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 6

            FixupOutput(ref output, @"block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  'static_assert-declaration' |  alias_declaration |  opaque_enum_declaration ;",
                @"block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? identifier |  class_key simple_template_id |  class_key nested_name_specifier template ? simple_template_id |  'enum' nested_name_specifier ? identifier ;",
                @"elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? identifier |  class_key simple_template_id |  class_key nested_name_specifier 'template' ? simple_template_id |  'enum' nested_name_specifier ? identifier ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"enum_specifier :  enum_head '{' 'enumerator-list ?}' |  enum_head '{' enumerator_list ',' '}' ;",
                @"enum_specifier :  enum_head '{' enumerator_list ? '}' |  enum_head '{' enumerator_list ',' '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"enum_head :  enum_key attribute_specifier_seq ? identifier ? enum_base ? |  enum_key attribute_specifier_seq ? nested_name_specifier identifier |  enum_base ? ;",
                @"enum_head :  enum_key attribute_specifier_seq ? identifier ? enum_base ? |  enum_key attribute_specifier_seq ? nested_name_specifier identifier enum_base ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"opaque_enum_declaration :  enum_key attribute_specifier_seq ? identifier 'enum-base ?;' ;",
                @"opaque_enum_declaration:  enum_key attribute_specifier_seq ? identifier enum_base ? ';' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"named_namespace_definition :  inline ? 'namespace' attribute_specifier_seq ? identifier '{' namespace_body '}' ;",
                @"named_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? identifier '{' namespace_body '}' ;");
            FixupOutput(ref output, @"unnamed_namespace_definition :  inline ? 'namespace' 'attribute-specifier-seq ?{' namespace_body '}' ;",
                @"unnamed_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? '{' namespace_body '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"unnamed_namespace_definition :  inline ? 'namespace' attribute_specifier_seq ? '{' namespace_body '}' ;",
                                    @"unnamed_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? '{' namespace_body '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"using_declaration :  'using' typename ? nested_name_specifier unqualified_id ';' ;",
                                    @"using_declaration :  'using' 'typename' ? nested_name_specifier unqualified_id ';' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"using_declarator :  typename ? nested_name_specifier unqualified_id ;",
                                    @"using_declarator :  'typename' ? nested_name_specifier unqualified_id ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });

            FixupOutput(ref output, @"using_directive :  attribute_specifier_seqoptusing 'namespace' nested_name_specifier ? namespace_name ';' ;",
                @"using_directive :  attribute_specifier_seq ? 'using' 'namespace' nested_name_specifier ? namespace_name ';' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"linkage_specification :  'extern' string_literal '{' 'declaration-seq ?}' |  'extern' string_literal declaration ;",
                @"linkage_specification :  'extern' string_literal '{' declaration_seq ? '}' |  'extern' string_literal declaration ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"balanced_token :  '(' balanced_token_seq ')' |  '[' balanced_token_seq ']' |  '{' balanced_token_seq '}' |  any token other than a 'parenthesis,' a 'bracket,' or a brace ;",
                @"balanced_token :  '(' balanced_token_seq ')' |  '[' balanced_token_seq ']' |  '{' balanced_token_seq '}' |  'any token other than a parenthesis, a bracket, or a brace' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"balanced_token :  '(' balanced_token_seq ? ')' |  '[' balanced_token_seq ? ']' |  '{' balanced_token_seq ? '}' |  any token other than a 'parenthesis,' a 'bracket,' or a brace ;",
                                    @"balanced_token :  '(' balanced_token_seq ? ')' |  '[' balanced_token_seq ? ']' |  '{' balanced_token_seq ? '}' |  'any token other than a parenthesis, a bracket, or a brace' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"alignment_specifier :  'alignas' '(' type_id '...opt)' |  'alignas' '(' constant_expression '...opt)' ;",
                @"alignment_specifier :  'alignas' '(' type_id '...' ? ')' |  'alignas' '(' constant_expression '...' ? ')' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 7

            FixupOutput(ref output, @"noptr_declarator :  declarator_id attribute_specifier_seq ? |  noptr_declarator parameters_and_qualifiers |  noptr_declarator '[' 'constant-expression ?]' attribute_specifier_seq ? |  '(' ptr_declarator ')' ;",
                @"noptr_declarator :  declarator_id attribute_specifier_seq ? |  noptr_declarator parameters_and_qualifiers |  noptr_declarator '[' constant_expression ? ']' attribute_specifier_seq ? |  '(' ptr_declarator ')' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? |  ref_qualifier ? noexcept_specifier ? attribute_specifier_seq ? ;",
                                    @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? ref_qualifier ? noexcept_specifier ? attribute_specifier_seq ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? |  ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;",
                                    @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ?  ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"noptr_abstract_declarator :  noptr_abstract_declarator ? parameters_and_qualifiers |  'noptr-abstract-declarator ?[' constant_expression ? ']' attribute_specifier_seq ? |  '(' ptr_abstract_declarator ')' ;",
                @"noptr_abstract_declarator :  noptr_abstract_declarator ? parameters_and_qualifiers |  noptr_abstract_declarator ? '[' constant_expression ? ']' attribute_specifier_seq ? |  '(' ptr_abstract_declarator ')' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"parameter_declaration_clause :  'parameter-declaration-list ?...opt' |  parameter_declaration_list ',' '...' ;",
                @"parameter_declaration_clause :  parameter_declaration_list ? '...' ? |  parameter_declaration_list ',' '...' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator '=' initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq 'abstract-declarator ?=' initializer_clause ;",
                @"parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator '=' initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? '=' initializer_clause ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"braced_init_list :  '{' initializer_list ',opt' '}' |  '{' '}' ;",
                @"braced_init_list :  '{' initializer_list ',' ? '}' |  '{' '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 8

            FixupOutput(ref output, @"class_specifier :  class_head '{' 'member-specification ?}' ;",
                @"class_specifier :  class_head '{' member_specification ? '}' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? 'member-declarator-list ?;' |  function_definition |  using_declaration |  'static_assert-declaration' |  template_declaration |  alias_declaration |  empty_declaration ;",
                                    @"member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? ';' |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  alias_declaration |  empty_declaration ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? ';' |  function_definition |  using_declaration |  'static_assert-declaration' |  template_declaration |  deduction_guide |  alias_declaration |  empty_declaration ;",
                                    @"member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? ';' |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  deduction_guide |  alias_declaration |  empty_declaration ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"member_declarator :  declarator virt_specifier_seq ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  identifier ? 'attribute-specifier-seq ?:' constant_expression ;",
                                    @"member_declarator :  declarator virt_specifier_seq ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  identifier ? attribute_specifier_seq ? ':' constant_expression ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 9

            FixupOutput(ref output, @"base_specifier :  attribute_specifier_seq ? base_type_specifier |  attribute_specifier_seqoptvirtual access_specifier ? base_type_specifier |  attribute_specifier_seq ? access_specifier virtual ? base_type_specifier ;",
                                    @"base_specifier :  attribute_specifier_seq ? base_type_specifier |  attribute_specifier_seq ? 'virtual' access_specifier ? base_type_specifier |  attribute_specifier_seq ? access_specifier 'virtual' ? base_type_specifier ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"base_specifier :  attribute_specifier_seq ? class_or_decltype |  attribute_specifier_seq ? 'virtual' access_specifier ? class_or_decltype |  attribute_specifier_seq ? access_specifier virtual ? class_or_decltype ;",
                                    @"base_specifier :  attribute_specifier_seq ? class_or_decltype |  attribute_specifier_seq ? 'virtual' access_specifier ? class_or_decltype |  attribute_specifier_seq ? access_specifier 'virtual' ? class_or_decltype ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });

            // Section 10

            FixupOutput(ref output, @"conversion_function_id :  operator conversion_type_id ;",
                @"conversion_function_id :  'operator' conversion_type_id ;");

            // Section 11

            FixupOutput(ref output, @"operator_function_id :  operator operator ;",
                @"operator_function_id :  'operator' operator ;");
            FixupOutput(ref output, @"operator :  'new' | 'delete' | 'new[]' | 'delete[]' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '()' | '[]' ;",
                @"operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '(' ')' | '[' ']' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"literal_operator_id :  operator string_literal identifier |  operator user_defined_string_literal ;",
                @"literal_operator_id :  'operator' string_literal identifier |  'operator' user_defined_string_literal ;");

            // Section 12

            FixupOutput(ref output, @"type_parameter :  type_parameter_key  '...' ? identifier ? |  type_parameter_key 'identifier ?=' type_id |  'template' '<' template_parameter_list '>' type_parameter_key  '...' ? identifier ? |  'template' '<' template_parameter_list '>' type_parameter_key 'identifier ?=' id_expression ;",
                @"type_parameter :  type_parameter_key  '...' ? identifier ? |  type_parameter_key identifier ? '=' type_id |  'template' '<' template_parameter_list '>' type_parameter_key  '...' ? identifier ? |  'template' '<' template_parameter_list '>' type_parameter_key identifier ? '=' id_expression ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"simple_template_id :  template_name '<' 'template-argument-list ?>' ;",
                @"simple_template_id :  template_name '<' template_argument_list ? '>' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"template_id :  simple_template_id |  operator_function_id '<' 'template-argument-list ?>' |  literal_operator_id '<' 'template-argument-list ?>' ;",
                @"template_id :  simple_template_id |  operator_function_id '<' template_argument_list ? '>' |  literal_operator_id '<' template_argument_list ? '>' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"typename_specifier :  'typename' nested_name_specifier identifier |  'typename' nested_name_specifier template ? simple_template_id ;",
                @"typename_specifier :  'typename' nested_name_specifier identifier |  'typename' nested_name_specifier 'template' ? simple_template_id ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"explicit_instantiation :  extern ? 'template' declaration ;",
                @"explicit_instantiation :  'extern' ? 'template' declaration ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"deduction_guide :  explicit ? template_name '(' parameter_declaration_clause ')' '->' simple_template_id ';' ;",
                                    @"deduction_guide :  'explicit' ? template_name '(' parameter_declaration_clause ')' '->' simple_template_id ';' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });


            // Section 13

            FixupOutput(ref output, @"dynamic_exception_specification :  'throw' '(' 'type-id-list ?)' ;",
                @"dynamic_exception_specification :  'throw' '(' type_id_list ? ')' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });

            // Section 14

            FixupOutput(ref output, @"control_line :  '#' 'include' pp_tokens new_line |  '#' 'define' identifier replacement_list new_line |  '#' 'define' identifier lparen 'identifier-list ?)' replacement_list new_line |  '#' 'define' identifier lparen '...' ')' replacement_list new_line |  '#' 'define' identifier lparen 'identifier-list,' '...' ')' replacement_list new_line |  '#' 'undef' identifier new_line |  '#' 'line' pp_tokens new_line |  '#' 'error' pp_tokens ? new_line |  '#' 'pragma' pp_tokens ? new_line |  '#' new_line ;",
                @"control_line :  '#' 'include' pp_tokens new_line |  '#' 'define' identifier replacement_list new_line |  '#' 'define' identifier lparen identifier_list ? ')' replacement_list new_line |  '#' 'define' identifier lparen '...' ')' replacement_list new_line |  '#' 'define' identifier lparen identifier_list ',' '...' ')' replacement_list new_line |  '#' 'undef' identifier new_line |  '#' 'line' pp_tokens new_line |  '#' 'error' pp_tokens ? new_line |  '#' 'pragma' pp_tokens ? new_line |  '#' new_line ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 0,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lparen :  a '(' character not immediately preceded by white_space ;",
                @"lparen :  'a ( character not immediately preceded by white_space' ;",
                just_fn switch
                {
                    "n4296.pdf" => 1,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"new_line :  the new_line character ;",
                @"new_line :  'the new_line character' ;");

            FixupOutput(ref output, @"defined_macro_expression :  defined identifier |  defined '(' identifier ')' ;",
                                    @"defined_macro_expression :  'defined' identifier |  'defined' '(' identifier ')' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"h_preprocessing_token :  any preprocessing_token other than '>' ;",
                                    @"h_preprocessing_token :  'any preprocessing_token other than >' ;",
                just_fn switch
                {
                    "n4296.pdf" => 0,
                    "n4660.pdf" => 1,
                    "n4878.pdf" => 1,
                    _ => 1
                });

            System.Console.Write(output);
        }
        public static string ReplaceFirstOccurrence(string source, string search, string replace)
        {
            int index = source.IndexOf(search);
            if (index < 0) return source;
            var sourceSpan = source.AsSpan();
            return string.Concat(sourceSpan.Slice(0, index), replace, sourceSpan.Slice(index + search.Length));
        }
        private static string Antlrize(string symbol)
        {
            symbol = symbol.Replace('-', '_');
            if (symbol == "p") symbol = "'p'";
            if (symbol == "P") symbol = "'P'";
            if (symbol == "e") symbol = "'e'";
            if (symbol == "E") symbol = "'E'";
            if (symbol == "R") symbol = "'R'";
            if (symbol == "alignas") symbol = "'alignas'";
            if (symbol == "alignof") symbol = "'alignof'";
            if (symbol == "asm") symbol = "'asm'";
            if (symbol == "auto") symbol = "'auto'";
            if (symbol == "bool") symbol = "'bool'";
            if (symbol == "break") symbol = "'break'";
            if (symbol == "case") symbol = "'case'";
            if (symbol == "catch") symbol = "'catch'";
            if (symbol == "char") symbol = "'char'";
            if (symbol == "class") symbol = "'class'";
            if (symbol == "const") symbol = "'const'";
            if (symbol == "constexpr") symbol = "'constexpr'";
            if (symbol == "continue") symbol = "'continue'";
            if (symbol == "decltype") symbol = "'decltype'";
            if (symbol == "default") symbol = "'default'";
            if (symbol == "define") symbol = "'define'";
            if (symbol == "delete") symbol = "'delete'";
            if (symbol == "do") symbol = "'do'";
            if (symbol == "double") symbol = "'double'";
            if (symbol == "elif") symbol = "'elif'";
            if (symbol == "else") symbol = "'else'";
            if (symbol == "endif") symbol = "'endif'";
            if (symbol == "enum") symbol = "'enum'";
            if (symbol == "error") symbol = "'error'";
            if (symbol == "explicit") symbol = "'explicit'";
            if (symbol == "export") symbol = "'export'";
            if (symbol == "extern") symbol = "'extern'";
            if (symbol == "false") symbol = "'false'";
            if (symbol == "final") symbol = "'final'";
            if (symbol == "float") symbol = "'float'";
            if (symbol == "for") symbol = "'for'";
            if (symbol == "friend") symbol = "'friend'";
            if (symbol == "goto") symbol = "'goto'";
            if (symbol == "if") symbol = "'if'";
            if (symbol == "if") symbol = "'if'";
            if (symbol == "ifdef") symbol = "'ifdef'";
            if (symbol == "ifndef") symbol = "'ifndef'";
            if (symbol == "include") symbol = "'include'";
            if (symbol == "inline") symbol = "'inline'";
            if (symbol == "int") symbol = "'int'";
            if (symbol == "line") symbol = "'line'";
            if (symbol == "long") symbol = "'long'";
            if (symbol == "mutable") symbol = "'mutable'";
            if (symbol == "namespace") symbol = "'namespace'";
            if (symbol == "noexcept") symbol = "'noexcept'";
            if (symbol == "nullptr") symbol = "'nullptr'";
            if (symbol == "optdefault") symbol = "? 'default'";
            if (symbol == "optdelete") symbol = "? 'delete'";
            if (symbol == "optnew") symbol = "? 'new'";
            if (symbol == "optusing") symbol = "? 'using'";
            if (symbol == "optvirtual") symbol = "? 'virtual'";
            if (symbol == "override") symbol = "'override'";
            if (symbol == "pragma") symbol = "'pragma'";
            if (symbol == "private") symbol = "'private'";
            if (symbol == "protected") symbol = "'protected'";
            if (symbol == "public") symbol = "'public'";
            if (symbol == "register") symbol = "'register'";
            if (symbol == "return") symbol = "'return'";
            if (symbol == "short") symbol = "'short'";
            if (symbol == "signed") symbol = "'signed'";
            if (symbol == "sizeof") symbol = "'sizeof'";
            if (symbol == "static") symbol = "'static'";
            if (symbol == "static_assert") symbol = "'static_assert'";
            if (symbol == "struct") symbol = "'struct'";
            if (symbol == "switch") symbol = "'switch'";
            if (symbol == "template") symbol = "'template'";
            if (symbol == "this") symbol = "'this'";
            if (symbol == "thread_local") symbol = "'thread_local'";
            if (symbol == "throw") symbol = "'throw'";
            if (symbol == "throw") symbol = "'throw'";
            if (symbol == "throw") symbol = "'throw'";
            if (symbol == "true") symbol = "'true'";
            if (symbol == "try") symbol = "'try'";
            if (symbol == "typedef") symbol = "'typedef'";
            if (symbol == "typeid") symbol = "'typeid'";
            if (symbol == "typename") symbol = "'typename'";
            if (symbol == "typeof") symbol = "'typeof'";
            if (symbol == "undef") symbol = "'undef'";
            if (symbol == "union") symbol = "'union'";
            if (symbol == "unsigned") symbol = "'unsigned'";
            if (symbol == "using") symbol = "'using'";
            if (symbol == "virtual") symbol = "'virtual'";
            if (symbol == "void") symbol = "'void'";
            if (symbol == "volatile") symbol = "'volatile'";
            if (symbol == "while") symbol = "'while'";
            return symbol;
        }

        private static bool IsName(string rhs)
        {
            if (rhs == "") return false;
            if (!char.IsLetter(rhs[0])) return false;
            for (var i = 1; i < rhs.Length; ++i)
            {
                if (!(char.IsLetter(rhs[i]) || rhs[i] == '-')) return false;
            }
            return true;
        }

        private static string ToLiteral(string input)
        {
            var literal = input;
            literal = literal.Replace("\\", "\\\\");
            literal = literal.Replace("\b", "\\b");
            literal = literal.Replace("\n", "\\n");
            literal = literal.Replace("\t", "\\t");
            literal = literal.Replace("\r", "\\r");
            literal = literal.Replace("\f", "\\f");
            //literal = literal.Replace("\"", "\\\"");
            literal = literal.Replace("'", "\\'");
            literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
            return literal;
        }

        public static string PerformEscapes(string s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.Append(ToLiteral(s));
            return new_s.ToString();
        }

        public static string Quotify(string s)
        {
            if (s == "'")
            {
                if (ebnf) return "\"'\"";
                else return "'" + PerformEscapes(s) + "'";
            } else if (s.Contains("'"))
            {
                if (ebnf) return "\"" + PerformEscapes(s) + "\"";
                else return "'" + PerformEscapes(s) + "'";
            }
            return "'" + PerformEscapes(s) + "'";
        }

        private static void FixupOutput(ref string input, string find, string replacement, int expected = 1)
        {
            string pattern = Regex.Escape(find);
            int count = 0;
            input = Regex.Replace(input, pattern, (_match) =>
            {
                Group group = _match.Groups[0];
                string replace = replacement;
                var z = String.Format("{0}{1}{2}", _match.Value.Substring(0, group.Index - _match.Index), replace, _match.Value.Substring(group.Index - _match.Index + group.Length));
                count++;
                return z;
            });
            if (count != expected) throw new Exception();
        }
    }


    public class MySimpleTextExtractionStrategy : ITextExtractionStrategy
    {
        private Vector lastStart;

        private Vector lastEnd;

        /// <summary>used to store the resulting String.</summary>
        private readonly StringBuilder result = new StringBuilder();

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                bool firstRender = result.Length == 0;
                bool hardReturn = false;
                LineSegment segment = renderInfo.GetBaseline();
                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();
                if (!firstRender)
                {
                    Vector x1 = lastStart;
                    Vector x2 = lastEnd;
                    // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                    float dist = (x2.Subtract(x1)).Cross((x1.Subtract(start))).LengthSquared() / x2.Subtract(x1).LengthSquared
                        ();
                    // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
                    float sameLineThreshold = 1f;
                    if (dist > sameLineThreshold)
                    {
                        hardReturn = true;
                    }
                }
                // Note:  Technically, we should check both the start and end positions, in case the angle of the text changed without any displacement
                // but this sort of thing probably doesn't happen much in reality, so we'll leave it alone for now
                if (hardReturn)
                {
                    //System.Console.WriteLine("<< Hard Return >>");
                    AppendTextChunk("\n");
                }
                else
                {
                    if (!firstRender)
                    {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (result[result.Length - 1] != ' ' && renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ')
                        {
                            float spacing = lastEnd.Subtract(start).Length();
                            if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
                            {
                                AppendTextChunk(" ");
                            }
                        }
                    }
                }
                //System.Console.WriteLine("Inserting implied space before '" + renderInfo.GetText() + "'");
                AppendTextChunk(renderInfo.GetText());
                lastStart = start;
                lastEnd = end;
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return JavaCollectionsUtil.UnmodifiableSet(new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(
                EventType.RENDER_TEXT)));
        }

        /// <summary>Returns the result so far.</summary>
        /// <returns>a String with the resulting text.</returns>
        public virtual String GetResultantText()
        {
            return result.ToString();
        }

        /// <summary>Used to actually append text to the text results.</summary>
        /// <remarks>
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        /// text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        /// image content)
        /// </remarks>
        /// <param name="text">the text to append to the text results accumulated so far</param>
        protected internal void AppendTextChunk(String text)
        {
            result.Append(text);
        }
    }
}