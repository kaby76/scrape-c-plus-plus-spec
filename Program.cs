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
//			string src = @"n4296.pdf";
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
            var regex = new Regex("[\r\n]([a-zA-Z_-]+[:]|[A][.])");
            var match = regex.Match(pdfText, cursor-1);
            if (match.Success)
            {
                var result = match.Index;
                for (; ; result++) if (!(pdfText[result] == '\r' || pdfText[result] == '\n')) break;
                return result;
            }
            else return -1;
        }

        static void Main(string[] args)
		{
            var result = new StringBuilder();

            string src_file_name = @"c-plus-plus-spec-draft.pdf";

            string pdfText = GetTextFromPDF(src_file_name);

            result.AppendLine("grammar " + Antlrize(System.IO.Path.GetFileNameWithoutExtension(src_file_name)) + ";");

            // Let's start parsing the spec text and extracting the
            // rules for C++.

            // First bracket the Annex A section.
            int annex_a = pdfText.IndexOf("Annex A (informative)");
            int annex_b = pdfText.IndexOf("Annex B (informative)");
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
                var lhs = pdfText.Substring(first, cursor - 1 - first);
                result.Append(Antlrize(lhs) + " : ");

                // Grab one line at a time and examine.
                var next_first = Find(cursor, pdfText);
                next_first = next_first >= 0 ? next_first + 1 : annex_b;
                bool first_time = true;
                bool one_of = false;
                bool term = true;
                for (; ; )
                {
                    var start = cursor;
                    if (start >= next_first) break;
                    var end = pdfText.IndexOf('\n', start);
                    if (end < 0) break;
                    if (end >= annex_b) break;
                    if (end > next_first) break;
                    cursor =  end + 1;
                    var len = end - start;
                    if (len <= 0) continue;
                    var rhs = pdfText.Substring(start, len);
                    rhs = rhs.Trim();
                    if (rhs == "") continue;
                    // Special cases that need to be delt with.
                    if (rhs.StartsWith("Note that a typedef-name naming")) continue;
                    if (rhs.StartsWith("any member of the source character set except new-line and >"))
                    {
                    }
                    if (rhs.StartsWith("§"))
                    {
                        term = false;
                        result.AppendLine(" ;");
                        result.AppendLine("// " + rhs);
                        result.AppendLine();
                        break;
                    }
                    if (rhs.StartsWith("A."))
                    {
                        term = false;
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
                        if (one_of) { r = (first_time ? "" : "| ") + "'" + PerformEscapes(r) + "'"; }
                        else if (r == "opt") r = "?";
                        else if (!IsName(r)) r = "'" + PerformEscapes(r) + "'";
                        else if (IsName(r)) r = Antlrize(r);
                        first_time = false;
                        result.Append(" " + r);
                    }
                }
                if (term) result.AppendLine(" ;");
			}

            result.AppendLine(@"keyword : 'alignas' | 'continue' | 'friend' | 'register' | 'true' 
    'alignof' | 'decltype' | 'goto' | 'reinterpret_cast' | 'try'
    'asm' | 'default' | 'if' | 'return' | 'typedef'
    'auto' | 'delete' | 'inline' | 'short' | 'typeid'
    'bool' | 'do' | 'int' | 'signed' | 'typename'
    'break' | 'double' | 'long' | 'sizeof' | 'union'
    'case' | 'dynamic_cast' | 'mutable' | 'static' | 'unsigned'
    'catch' | 'else' | 'namespace' | 'static_assert' | 'using'
    'char' | 'enum' | 'new' | 'static_cast' | 'virtual'
    'char16_t' | 'explicit' | 'noexcept' | 'struct' | 'void'
    'char32_t' | 'export' | 'nullptr' | 'switch' | 'volatile'
    'class' | 'extern' | 'operator' | 'template' | 'wchar_t'
    'const' | 'false' | 'private' | 'this' | 'while'
    'constexpr' | 'float' | 'protected' | 'thread_local'
    'const_cast' | 'for' | 'public' | 'throw'
    'and' | 'and_eq' | 'bitand' | 'bitor' | 'compl' | 'not'
    'not_eq' | 'or' | 'or_eq' | 'xor' | 'xor_eq' ;
");
            result.AppendLine("punctuator : preprocessing_op_or_punc;");

            var output = result.ToString();
            // Fix ups.
            output = output.Replace("|  ?", "?");
            output = output.Replace(" e ", " 'e' ");
            output = output.Replace(" E ", " 'E' ");
            output = ReplaceFirstOccurrence(output, "typedef_name :  identifier ;", "// typedef_name :  identifier ;");
            output = ReplaceFirstOccurrence(output, "enum_name :  identifier ;", "// enum_name :  identifier ;");
            output = ReplaceFirstOccurrence(output, "namespace_name :  original_namespace_name |  namespace_alias ;", "// namespace_name :  original_namespace_name |  namespace_alias ;");
            output = ReplaceFirstOccurrence(output, "namespace_alias :  identifier ;", "// namespace_alias :  identifier ;");
            output = ReplaceFirstOccurrence(output, "class_name :  identifier |  simple_template_id ;", "// class_name :  identifier |  simple_template_id ;");
            output = ReplaceFirstOccurrence(output, "template_name :  identifier ;", "// template_name :  identifier ;");
            output = ReplaceFirstOccurrence(output, "each non_white_space character that cannot be one of the above", "RESTRICTED_NON_WHITE");
            
                System.Console.Write(output);
		//	Console.WriteLine(pdfText);
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
            if (symbol == "private") symbol = symbol + "_";
            if (symbol == "protected") symbol = symbol + "_";
            if (symbol == "public") symbol = symbol + "_";
            if (symbol == "catch") symbol = symbol + "_";
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