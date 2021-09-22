using Antlr4.Runtime;
using System.IO;

public abstract class ScrapeParserBase : Parser {
	private readonly ITokenStream _input;

	protected ScrapeParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		_input = input;
	}
}
