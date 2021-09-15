﻿using iText.IO.Util;
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
		static string GetTextFromPDF()
		{
			StringBuilder text = new StringBuilder();
//			string src = @"C:\Users\kenne\Downloads\n4296.pdf";
			string src = @"C:\Users\kenne\Downloads\c-plus-plus-spec-draft.pdf";
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(src));
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
			string pdfText = GetTextFromPDF();

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
                    System.Console.WriteLine();
                    System.Console.WriteLine("// " + pdfText.Substring(first, cursor - first));
                    continue;
                }

                // Starting at a rule...
                cursor = pdfText.IndexOf(':', first) + 1;
                var lhs = pdfText.Substring(first, cursor - 1 - first);
                System.Console.Write(lhs + " : ");

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
                    if (rhs.StartsWith("§"))
                    {
                        term = false;
                        System.Console.WriteLine(" ;");
                        System.Console.WriteLine("// " + rhs);
                        System.Console.WriteLine();
                        break;
                    }
                    if (rhs.StartsWith("A."))
                    {
                        term = false;
                        System.Console.WriteLine(" ;");
                        System.Console.WriteLine();
                        System.Console.WriteLine("// " + rhs);
                        break;
                    }
                    if (first_time)
                    {
                        one_of = rhs == "one of";
                    }
                    else
                    {
                        if (!one_of) System.Console.Write(" | ");
                        else System.Console.Write(" ");
                    }
                    var ss = rhs.Split(' ').ToList();
                    first_time = false;
                    foreach (var s in ss)
                    {
                        var r = s;
                        if (!IsName(r)) r = "'" + r + "'";
                        if (r == "opt") r = "?";
                        System.Console.Write(" " + r);
                    }
                }
                if (term) System.Console.WriteLine(" ;");
			}

		//	Console.WriteLine(pdfText);
		}

        private static bool IsName(string rhs)
        {
            for (var i = 0; i < rhs.Length; ++i)
            {
                if (!(char.IsLetter(rhs[i]) || rhs[i] == '-')) return false;
            }
            return true;
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