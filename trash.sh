#

echo Generating c_plus_plus_spec_draft.g4 from scratch...
./bin/Debug/net5.0/scrape-pdf.exe > c_plus_plus_spec_draft.g4

echo Rewriting seveal basic literal rules...
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'universal_character_name,Universal_character_name;hex_quad,Hex_quad;hexadecimal_digit,Hexadecimal_digit;binary_digit,Binary_digit;octal_digit,Octal_digit;nonzero_digit,Nonzero_digit;unsigned_suffix,Unsigned_suffix;long_suffix,Long_suffix;long_long_suffix,Long_long_suffix;encoding_prefix,Encoding_prefix' | \
	trsponge -c true

trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'sign,Sign;exponent_part,Exponent_part;digit_sequence,Digit_sequence;fractional_constant,Fractional_constant;hexadecimal_escape_sequence,Hexadecimal_escape_sequence;octal_escape_sequence,Octal_escape_sequence;simple_escape_sequence,Simple_escape_sequence;escape_sequence,Escape_sequence;c_char,C_char;c_char_sequence,C_char_sequence;digit,Digit;nondigit,Nondigit;floating_suffix,Floating_suffix;integer_suffix,Integer_suffix' | \
	trsponge -c true

	exit 0
	
trparse c_plus_plus_spec_draft.g4 | \
	trinsert " //ruleSpec/lexerRuleSpec/TOKEN_REF[text()='Hex_quad' or text()='Universal_character_name' or text()='Binary_digit' or text()='Octal_digit' or text()='Nonzero_digit' or text()='Hexadecimal_digit' or text()='Unsigned_suffix' or text()='Long_suffix' or text()='Long_long_suffix' or text()='Encoding_prefix']" "fragment" | \
	trinsert " //ruleSpec/lexerRuleSpec/TOKEN_REF[text()='Sign' or text()='Exponent_part' or text()='Digit_sequence' or text()='Fractional_constant' or text()='Hexadecimal_escape_sequence' or text()='Octal_escape_sequence' or text()='Simple_escape_sequence' or text()='Escape_sequence' or text()='C_char' or text()='C_char_sequence' or text()='Digit' or text()='Nondigit' or text()='Floating_suffix' or text()='Integer_suffix']" "fragment" | \
	trrename -r 'integer_suffix,Integer_suffix;integer_literal,Integer_literal;binary_literal,Binary_literal;octal_literal,Octal_literal;decimal_literal,Decimal_literal;hexadecimal_literal,Hexadecimal_literal' | \
	trsponge -c true

	exit 0
	trrename -r 'binary_literal,Binary_literal;integer_suffix,Integer_suffix;octal_literal,Octal_literal;hexadecimal_literal,Hexadecimal_literal'

exit 0

echo Rewriting lexer String rules ...
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'string_literal,String_literal;encoding_prefix,Encoding_prefix;s_char_sequence,S_char_sequence;raw_string,Raw_string;s_char,S_char;escape_sequence,Escape_sequence;universal_character_name,Universal_character_name;hex_quad,Hex_quad;hexadecimal_digit,Hexadecimal_digit' | \
	trsponge -c true

echo Correcting raw grammar due to expected parse errors.
