namespace Test
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TreeOutput
    {
        private static int changed = 0;
        private static bool first_time = true;

        public static StringBuilder OutputTree(IParseTree tree, Lexer lexer, Parser parser, CommonTokenStream stream)
        {
            changed = 0;
            first_time = true;
            var sb = new StringBuilder();
            ParenthesizedAST(tree, sb, lexer, parser, stream);
            return sb;
        }

        private static void ParenthesizedAST(IParseTree tree, StringBuilder sb, Lexer lexer, Parser parser, CommonTokenStream stream, int level = 0)
        {
            // Antlr always names a non-terminal with first letter lowercase,
            // but renames it when creating the type in C#. So, remove the prefix,
            // lowercase the first letter, and remove the trailing "Context" part of
            // the name. Saves big time on output!
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                Interval interval = tok.SourceInterval;
                IList<IToken> inter = null;
                if (tok.Symbol.TokenIndex >= 0)
                    inter = stream?.GetHiddenTokensToLeft(tok.Symbol.TokenIndex);
                if (inter != null)
                    foreach (var t in inter)
                    {
                        var ty = tok.Symbol.Type;
                        var name = lexer.Vocabulary.GetSymbolicName(ty);
                        StartLine(sb, level);
                        sb.AppendLine("( " + name + " text=" + PerformEscapes(t.Text) + " " + lexer.ChannelNames[t.Channel]);
                    }
                {
                    var ty = tok.Symbol.Type;
                    var name = lexer.Vocabulary.GetSymbolicName(ty);
                    StartLine(sb, level);
                    sb.AppendLine("( " + name + " i=" + tree.SourceInterval.a
                        + " txt=" + PerformEscapes(tree.GetText())
                        + " tt=" + tok.Symbol.Type
                        + " " + lexer.ChannelNames[tok.Symbol.Channel]);
                }
            }
            else
            {
                var x = tree as RuleContext;
                var ri = x.RuleIndex;
                var name = parser.RuleNames.Length <= ri ? "unknown" : parser.RuleNames[ri];
                StartLine(sb, level);
                sb.Append("( " + name);
                sb.AppendLine();
            }
            for (int i = 0; i < tree.ChildCount; ++i)
            {
                var c = tree.GetChild(i);
                ParenthesizedAST(c, sb, lexer, parser, stream, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                sb.AppendLine();
                changed = 0;
            }
        }

        private static void StartLine(StringBuilder sb, int level = 0)
        {
            if (changed - level >= 0)
            {
                if (!first_time)
                {
                    for (int j = 0; j < level; ++j) sb.Append("  ");
                    for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                    sb.AppendLine();
                }
                changed = 0;
                first_time = false;
            }
            changed = level;
            for (int j = 0; j < level; ++j) sb.Append("  ");
        }

        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                var literal = input;
                literal = literal.Replace("\\", "\\\\");
                literal = literal.Replace("\b", "\\b");
                literal = literal.Replace("\n", "\\n");
                literal = literal.Replace("\t", "\\t");
                literal = literal.Replace("\r", "\\r");
                literal = literal.Replace("\f", "\\f");
                literal = literal.Replace("\"", "\\\"");
                literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                return literal;
            }
        }

        public static string PerformEscapes(string s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.Append(ToLiteral(s));
            return new_s.ToString();
        }

        public static string OutputTokens(CommonTokenStream cts)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = cts.Get(i);
                var token = (CommonToken)ro_token;
                token.TokenIndex = i;
                sb.AppendLine(token.ToString());
                if (token.Type == Antlr4.Runtime.TokenConstants.EOF)
                    break;
            }
            return sb.ToString();
        }

        public static string Reconstruct(ITokenStream tokens, IParseTree tree)
        {
            StringBuilder sb = new StringBuilder();
            if (tree is ParserRuleContext con)
            {
                var val = con.SourceInterval;
                for (int i = val.a; i <= val.b; ++i)
                {
                    sb.Append(tokens.Get(i).Text);
                }
                return sb.ToString();
            }
            else if (tree is ErrorNodeImpl err)
            {
                return "";
            }
            else if (tree is TerminalNodeImpl term)
            {
                return term.GetText();
            }
            else throw new Exception("Unknown type");
        }
    }
}
