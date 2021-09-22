using Antlr4.Runtime;
using System.IO;

public abstract class ScrapeLexerBase : Lexer {
	private readonly ICharStream _input;

	protected ScrapeLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput)
			: base(input, output, errorOutput)
	{
		_input = input;
	}

//	public override IToken NextToken()
//	{
//	}
}
