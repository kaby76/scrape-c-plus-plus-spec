using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

public abstract class LexerBase : Lexer
{
    public bool _noisy { get; set; } = false;
    private ICharStream _input;
    protected LexerBase(ICharStream input)
        : base(input, Console.Out, Console.Error)
    {
        _input = input;
    }

    protected LexerBase(ICharStream input, TextWriter output, TextWriter errorOutput)
            : base(input, output, errorOutput)
    {
        _input = input;
    }

    public void NewStream(ICharStream input)
    {
    }

    protected bool NotEnd()
    {
	    if (_input.LA(1) == '*' && _input.LA(2) == '/')
            return false;
        else
            return true;
    }
}
