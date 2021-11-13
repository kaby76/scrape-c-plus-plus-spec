/*
MIT License

Copyright (c) 2021 Ken Domino

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
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
    enum Version
    {
        draft_cpp_14 = 13,
        iso_cpp_14 = 14,
        draft_cpp_17 = 16,
        iso_cpp_17 = 17,
        draft_cpp_20 = 19,
        iso_cpp_20 = 20,
        cpp_23 = 23,
        unknown = 0
    }

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
        static bool dont_care = false;

        static void Main(string[] args)
        {
            var result = new StringBuilder();
            result.Append(@"
/*

MIT License

Copyright (c) 2021 Ken Domino

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

© ISO/IEC 2020
All rights reserved. Unless otherwise specified, or required in the context of its implementation, no part of this publication may
be reproduced or utilized otherwise in any form or by any means, electronic or mechanical, including photocopying, or posting
on the internet or an intranet, without prior written permission. Permission can be requested from either ISO at the address
below or ISO’s member body in the country of the requester.
ISO copyright office
CP 401 • Ch. de Blandonnet 8
CH-1214 Vernier, Geneva
Phone: +41 22 749 01 11
Email: copyright@iso.org
Website: www.iso.org
Published in Switzerland

*/

");

            string src_file_name = args[0];
            
            var just_fn = System.IO.Path.GetFileName(src_file_name);
            Version version = just_fn switch
            {
                "n3797.pdf" => Version.draft_cpp_14,
                "n4296.pdf" => Version.draft_cpp_14,
                "C++14-ISOIEC-148822014.pdf" => Version.iso_cpp_14,
                "n4660.pdf" => Version.draft_cpp_17,
                "C++17-ISOIEC-148822017.pdf" => Version.iso_cpp_17,
                "n4878.pdf" => Version.draft_cpp_20,
                "C++20-ISOIEC-148822020.pdf" => Version.iso_cpp_20,
                _ => Version.unknown
            };
            if (args.Length > 1) { version = (Version)int.Parse(args[1]); }
            if (args.Length > 2) { dont_care = true; }

            string pdfText = GetTextFromPDF(src_file_name);

            pdfText = pdfText.Replace("ﬁ", "fi"); // Wrong OCR of "identifier" throughout the text.
            pdfText = pdfText.Replace("’", "'"); // Wrong OCR of single quote in numerous locations.
            pdfText = pdfText.Replace("ˆ", "^"); // Wrong OCR of caret.
            pdfText = pdfText.Replace("ﬂ", "fl"); // Fix ﬂoating_literal in literal rule.
            pdfText = pdfText.Replace("ﬃ", "ffi");
            pdfText = pdfText.Replace(@"'''", @"'\''");
            pdfText = pdfText.Replace(@"∼", @"~");
            // Just make life easier and rename this to "standard" [a-zA-Z\-]+.
            pdfText = pdfText.Replace(@"static_assert-declaration", @"static-assert-declaration");


            if (!ebnf)
            {
//                result.AppendLine("grammar " + Antlrize(System.IO.Path.GetFileNameWithoutExtension(src_file_name)) + ";");
                result.AppendLine("grammar Scrape;");
            }

            // Let's start parsing the spec text and extracting the
            // rules for C++.

            // First bracket the Annex A section.
            int annex_a = pdfText.IndexOf("Annex A (informative)");
            int annex_b = version switch
            {
                Version.draft_cpp_14 => pdfText.IndexOf("Annex B (informative)"),
                Version.iso_cpp_14 => pdfText.IndexOf("Annex B (informative)"),
                Version.draft_cpp_17 => pdfText.IndexOf("Annex B (informative)"),
                Version.iso_cpp_17 => pdfText.IndexOf("Annex B (informative)"),
                Version.draft_cpp_20 => pdfText.IndexOf("Annex B (normative)"),
                Version.iso_cpp_20 => pdfText.IndexOf("Annex B (normative)"),
                _ => throw new Exception()
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

            Regex opt6 = new Regex(@"(?<id>[a-zA-Z][a-zA-Z_\-]*(prefix|seq))opt(?<o>[a-zA-Z][a-zA-Z_\-]*)[ ]");
            output = opt6.Replace(output, (Match match) =>
            {
                var g = match.Groups["id"];
                var id = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = Antlrize(id) + " ? " + Antlrize(o) + " ";
                return res;
            });

            Regex opt5 = new Regex(@"['](?<r>[<>{}\\'.,()\[\];:=""]+)opt(?<o>[a-zA-Z][a-zA-Z_\-]*)['](?=[ ])");
            output = opt5.Replace(output, (Match match) =>
            {
                var g = match.Groups["r"];
                var r = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = "'" + r + "' ? " + Antlrize(o);
                return res;
            });

            Regex opt4 = new Regex(@"['](?<r>[<>{}\\'.,()\[\];:=""]+)opt(?<o>[<>{}\\'.,()\[\];:=""]+)['](?=[ ])");
            output = opt4.Replace(output, (Match match) =>
            {
                var g = match.Groups["r"];
                var r = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = "'" + r + "' ? '" + o + "'";
                return res;
            });

            Regex opt4a = new Regex(@"['](?<r>[<>{}\\'.,()\[\];:=""]+)opt['](?=[ ])");
            output = opt4a.Replace(output, (Match match) =>
            {
                var g = match.Groups["r"];
                var r = g.Value;
                var res = "'" + r + "' ?";
                return res;
            });

            Regex opt3 = new Regex(@"['](?<id>[a-zA-Z][a-zA-Z_\-]*)opt(?<o>[<>{}\\'.,()\[\];:=""]+)[']");
            output = opt3.Replace(output, (Match match) =>
            {
                var g = match.Groups["id"];
                var id = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = Antlrize(id) + " ? '" + o + "'";
                return res;
            });

            Regex opt3a = new Regex(@"['](?<id>[a-zA-Z][a-zA-Z_\-]*)(?<o>[<>{}\\'.,()\[\];:=""]+)[']");
            output = opt3a.Replace(output, (Match match) =>
            {
                var g = match.Groups["id"];
                var id = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = Antlrize(id) + " '" + o + "'";
                return res;
            });

            Regex opt2 = new Regex(@"['](?<id>[a-zA-Z][a-zA-Z_\-]*)(?<o>['.)=]+)opt[']");
            output = opt2.Replace(output, (Match match) =>
            {
                var g = match.Groups["id"];
                var id = g.Value;
                var g2 = match.Groups["o"];
                var o = g2.Value;
                var res = Antlrize(id) + " '" + o + "' ?";
                return res;
            });

            Regex opt1 = new Regex(@"(?<id>[a-zA-Z][a-zA-Z_\-]*)opt(?![a-zA-Z])");
            output = opt1.Replace(output, (Match match) =>
            {
                var g = match.Groups["id"];
                var id = g.Value;
                var res = Antlrize(id) + " ?";
                return res;
            });

            output = output.Replace(@"'opt)'", @"? ')'");
            output = output.Replace(@"'...opt)'", @"'...' ? ')'");
            output = output.Replace(@"'opt='", @"? '='");
            output = output.Replace(@"'opt:'", @"? ':'");
            output = output.Replace(@"'opt;'", @"? ';'");
            output = output.Replace(@"'opt{'", @"? '{'");
            output = output.Replace(@"'opt}'", @"? '}'");
            output = output.Replace(@"'opt]'", @"? ']'");
            output = output.Replace(@"'opt>'", @"? '>'");
            output = output.Replace(@"'...opt'", @"'...' ?");
            output = output.Replace(@"'::optnew'", @"'::' ? 'new'");
            output = output.Replace(@"'::opt'", @"'::' ?");
            output = output.Replace(@"',opt'", @"',' ?");
            output = output.Replace(@"';opt'", @"';' ?");
            

            // Section 2

            FixupOutput(ref output,
                @"each non_white_space character that cannot be one of the above",
                @"'each non_white_space character that cannot be one of the above'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                }); // preprocessing_token

            FixupOutput(ref output, @"any member of the source character set except new_line and '>'",
                @"'any member of the source character set except new_line and >'");
            FixupOutput(ref output, @"any member of the source character set except new_line and '""'",
                @"'any member of the source character set except new_line and ""'");
            FixupOutput(ref output, @"other implementation_defined characters",
                @"'other implementation_defined characters'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"any identifier listed in Table '5'",
                                    @"'any identifier listed in Table 5'",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"any member of the source character set except |  the single_quote '\',' backslash '\\,' or new_line character",
                @"'any member of the source character set except the single_quote \', backslash \\, or new_line character'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"any member of the basic source character set except the single_quote '\',' backslash '\\,' or new_line character",
                                    @"'any member of the source character set except the single_quote \', backslash \\, or new_line character'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"basic_c_char :  any member of the basic source character set except the single_quote '’,' backslash '\\,' or new_line character ;",
                                    @"basic_c_char :  'any member of the basic source character set except the single_quote \', backslash \\, or new_line character ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"any member of the basic source character set that is not an 'octal-digit,' a 'simple-escape-sequence-char,' or |  the characters 'u,' 'U,' or x",
                                    @"'any member of the basic source character set that is not an octal_digit, a simple_escape_sequence_char, or the characters u, U, or x'",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"s_char :  any member of the source character set except |  the double_quote '"",' backslash '\\,' or new_line character |  escape_sequence |  universal_character_name ;",
                @"s_char :  'any member of the source character set except the double_quote "", backslash \\. or new_line character' | escape_sequence | universal_character_name ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"basic_s_char :  any member of the basic source character set except the double_quote '"",' backslash '\\,' or new_line character ;",
                                    @"basic_s_char :  'any member of the basic source character set except the double_quote "", backslash \\. or new_line character' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"r_char :  any member of the source character set ',' except |  a right parenthesis ')' followed by the initial d_char_sequence |  '(which' may be empty ')' followed by a 'double' quote '"".' ;",
                                    @"r_char :  'any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote "".' ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"r_char :  any member of the source character 'set,' except a right parenthesis ')' followed by |  the initial d_char_sequence '(which' may be 'empty)' followed by a 'double' quote '"".' ;",
                                    @"r_char :  'any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote "".' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });


            FixupOutput(ref output, @"d_char :  any member of the basic source character set except ':' |  space ',' the left parenthesis '(,' the right parenthesis '),' the backslash '\\,' |  and the control characters representing horizontal tab ',' |  vertical tab ',' form feed ',' and newline '.' ;",
                                    @"d_char :  'any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.' ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"d_char :  any member of the basic source character set 'except:' |  'space,' the left parenthesis '(,' the right parenthesis '),' the backslash '\\,' and the control characters |  representing horizontal 'tab,' vertical 'tab,' form 'feed,' and 'newline.' ;",
                                    @"d_char :  'any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });

            // Section 3

            // Section 4

            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' 'mutable' ? |  exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;",
                                    @"lambda_declarator :  '(' parameter_declaration_clause ')' 'mutable' ? exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? |  noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                                    @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq |  ? |  noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                                    @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? |  noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? requires_clause ? ;",
                                    @"lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? requires_clause ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"'logical-or-expression||'",
                @"logical_or_expression '||'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });

            // Section 5

            // Section 6

            FixupOutput(ref output, @"block_declaration :  simple_declaration |  asm_declaration |  namespace_alias_definition |  using_declaration |  using_enum_declaration |  using_directive |  'static_assert-declaration' |  alias_declaration |  opaque_enum_declaration ;",
                                    @"block_declaration :  simple_declaration |  asm_declaration |  namespace_alias_definition |  using_declaration |  using_enum_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"enum_head :  enum_key attribute_specifier_seq ? identifier ? enum_base ? |  enum_key attribute_specifier_seq ? nested_name_specifier identifier |  enum_base ? ;",
                                    @"enum_head :  enum_key attribute_specifier_seq ? identifier ? enum_base ? |  enum_key attribute_specifier_seq ? nested_name_specifier identifier enum_base ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });

            FixupOutput(ref output, @"any token other than a parenthesis ',' a bracket ',' or a brace",
                                    @"'any token other than a parenthesis, a bracket, or a brace'",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });

            // Section 7

            FixupOutput(ref output, @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? |  ref_qualifier ? noexcept_specifier ? attribute_specifier_seq ? ;",
                                    @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? ref_qualifier ? noexcept_specifier ? attribute_specifier_seq ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? |  ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;",
                                    @"parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ?  ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });

            // Section 8


            // Section 9

            // Section 10

            FixupOutput(ref output, @"conversion_function_id :  operator conversion_type_id ;",
                                    @"conversion_function_id :  'operator' conversion_type_id ;");

            // Section 11

            FixupOutput(ref output, @"operator_function_id :  operator operator ;",
                @"operator_function_id :  'operator' operator ;");
            //                                  @"operator :  'new' | 'delete' | 'new' '[]' | 'delete' '[]' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '∼' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '()' | '[]' ;"
            FixupOutput(ref output, @"operator :  'new' | 'delete' | 'new' '[]' | 'delete' '[]' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '()' | '[]' ;",
                                    @"operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '(' ')' | '[' ']' ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            
            FixupOutput(ref output, @"operator :  'new' | 'delete' | 'new' '[]' | 'delete' '[]' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|' | '=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '|' | '|' | '++' | '--' | ',' | '->*' | '->' | '(' | ')' | '[]' ;",
                                    @"operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '(' ')' | '[' ']' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });

            FixupOutput(ref output, @"operator :  'new' | 'delete' | 'new[]' | 'delete[]' | 'co_await' | '()' | '[]' | '->' | '->*' | '~' | '!' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '==' | '!=' | '<' | '>' | '<=' | '>=' | '<=>' | '&&' | '||' | '<<' | '>>' | '<<=' | '>>=' | '++' | '--' | ',' ;",
                                    @"operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | 'co_await' | '(' ')' | '[' ']' | '->' | '->*' | '~' | '!' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '==' | '!=' | '<' | '>' | '<=' | '>=' | '<=>' | '&&' | '||' | '<<' | '>>' | '<<=' | '>>=' | '++' | '--' | ',' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"literal_operator_id :  operator string_literal identifier |  operator user_defined_string_literal ;",
                @"literal_operator_id :  'operator' string_literal identifier |  'operator' user_defined_string_literal ;");

            // Section 12

            // Section 13

            // Section 14

            FixupOutput(ref output, @"lparen :  a '(' character not immediately preceded by white_space ;",
                                    @"lparen :  'a ( character not immediately preceded by white_space' ;",
                version switch
                {
                    Version.draft_cpp_14 => 1,
                    Version.iso_cpp_14 => 1,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 0,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"lparen :  a '(' character not immediately preceded by whitespace ;",
                                    @"lparen :  'a ( character not immediately preceded by whitespace' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 0,
                    _ => 1
                });
            FixupOutput(ref output, @"new_line :  the new_line character ;",
                                    @"new_line :  'the new_line character' ;");

            FixupOutput(ref output, @"defined_macro_expression :  defined identifier |  defined '(' identifier ')' ;",
                                    @"defined_macro_expression :  'defined' identifier |  'defined' '(' identifier ')' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @"h_preprocessing_token :  any preprocessing_token other than '>' ;",
                                    @"h_preprocessing_token :  'any preprocessing_token other than >' ;",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 1,
                    Version.iso_cpp_17 => 1,
                    Version.draft_cpp_20 => 1,
                    Version.iso_cpp_20 => 1,
                    _ => 1
                });
            FixupOutput(ref output, @" export ?",
                                    @" 'export' ?",
                version switch
                {
                    Version.draft_cpp_14 => 0,
                    Version.iso_cpp_14 => 0,
                    Version.draft_cpp_17 => 0,
                    Version.iso_cpp_17 => 0,
                    Version.draft_cpp_20 => 4,
                    Version.iso_cpp_20 => 0,
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
            if (symbol == "consteval") symbol = "'consteval'";
            if (symbol == "constinit") symbol = "'constinit'";
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
            if (symbol == "import") symbol = "'import'";
            if (symbol == "include") symbol = "'include'";
            if (symbol == "inline") symbol = "'inline'";
	    if (symbol == "int") symbol = "'int'";
	    if (symbol == "L") symbol = "'L'";
            if (symbol == "line") symbol = "'line'";
            if (symbol == "long") symbol = "'long'";
            if (symbol == "module") symbol = "'module'";
            if (symbol == "mutable") symbol = "'mutable'";
            if (symbol == "namespace") symbol = "'namespace'";
            if (symbol == "new") symbol = "'new'";
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
            if (symbol == "requires") symbol = "'requires'";
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
	    if (symbol == "u") symbol = "'u'";
	    if (symbol == "U") symbol = "'U'";
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
            if (!dont_care && count != expected) throw new Exception();
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