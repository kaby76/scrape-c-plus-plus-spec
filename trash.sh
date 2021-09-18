#

export MSYS2_ARG_CONV_EXCL="*"

echo 'Generating a basic c_plus_plus_spec_draft.g4 from scratch.'
echo 'This grammar passes the Antlr4 tool with warnings, but it is not really usable by Antlr4.'
./bin/Debug/net5.0/scrape-pdf.exe > c_plus_plus_spec_draft.g4
cp c_plus_plus_spec_draft.g4 _orig.g4

echo ""
echo 'Renaming several basic literal rules...'
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'universal_character_name,FUniversal_character_name;hex_quad,FHex_quad;hexadecimal_digit,FHexadecimal_digit;binary_digit,FBinary_digit;octal_digit,FOctal_digit;nonzero_digit,FNonzero_digit;unsigned_suffix,FUnsigned_suffix;long_suffix,FLong_suffix;long_long_suffix,FLong_long_suffix;encoding_prefix,FEncoding_prefix' | \
	trsponge -c true

echo ""
echo 'Additional renaming of several basic literal rules...'
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'sign,FSign;exponent_part,FExponent_part;digit_sequence,FDigit_sequence;fractional_constant,FFractional_constant;hexadecimal_escape_sequence,FHexadecimal_escape_sequence;octal_escape_sequence,FOctal_escape_sequence;simple_escape_sequence,FSimple_escape_sequence;escape_sequence,FEscape_sequence;c_char,FC_char;c_char_sequence,FC_char_sequence;digit,FDigit;nondigit,FNondigit;floating_suffix,FFloating_suffix;integer_suffix,FInteger_suffix' | \
	trsponge -c true

echo ""
echo 'Taking care of converting RESTRICTED_CHARS5 into its proper form...'
trparse c_plus_plus_spec_draft.g4 | \
	trinsert "//terminal/TOKEN_REF[text()='RESTRICTED_CHARS5']" "~['\\\r\n]" | \
	trdelete "//lexerElement/lexerAtom/terminal/TOKEN_REF[text()='RESTRICTED_CHARS5']" | \
	trsponge -c true

echo ""
echo Inserting "'fragment'" into basic rules...
trparse c_plus_plus_spec_draft.g4 | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FHex_quad' or text()='FUniversal_character_name' or text()='FBinary_digit' or text()='FOctal_digit' or text()='FNonzero_digit' or text()='FHexadecimal_digit' or text()='FUnsigned_suffix' or text()='FLong_suffix' or text()='FLong_long_suffix' or text()='FEncoding_prefix']" "fragment" | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FSign' or text()='FExponent_part' or text()='FDigit_sequence' or text()='FFractional_constant' or text()='FOctal_escape_sequence' or text()='FSimple_escape_sequence' or text()='FEscape_sequence' or text()='FC_char' or text()='FDigit' or text()='FNondigit' or text()='FFloating_suffix' or text()='FInteger_suffix']" "fragment" | \
	trsponge -c true

echo ""
echo 'We converted some rules that were parser rules into lexer rules.'
echo 'Unfortunately, the grammar does not compile because Antlr4 does not handle'
echo 'left-recursion in lexer rule (it does for parser rules).'
echo 'In particular, the rules are Hexadecimal_escape_sequence, C_char_sequence, Digit_sequence'

trparse c_plus_plus_spec_draft.g4 | \
	trkleene "//lexerRuleSpec/TOKEN_REF[text()='FHexadecimal_escape_sequence']" | \
	trkleene "//lexerRuleSpec/TOKEN_REF[text()='FC_char_sequence']" | \
	trkleene "//lexerRuleSpec/TOKEN_REF[text()='FDigit_sequence']" | \
	trsponge -c true

echo ""
echo "Adding 'fragment' to selected lexer rules."
trparse c_plus_plus_spec_draft.g4 | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FDigit_sequence' or text()='FC_char_sequence' or text()='FHexadecimal_escape_sequence']" "fragment" | \
	trsponge -c true
	
echo ""
echo "Now renaming integer_literal, binary_literal, octal_literal, hexadecimal_literal"
echo "into lexer rules."
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'integer_literal,Integer_literal;binary_literal,Binary_literal;octal_literal,Octal_literal;decimal_literal,Decimal_literal;hexadecimal_literal,Hexadecimal_literal' | \
	trkleene "//lexerRuleSpec/TOKEN_REF[text()='Integer_literal' or text()='Binary_literal' or text()='Octal_literal' or text()='Decimal_literal' or text()='Hexadecimal_literal']" | \
	trsponge -c true

trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'character_literal,Character_literal;floating_literal,Floating_literal' | \
	trsponge -c true

echo ""
echo "Fixing identifer rules..."
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'identifier,Identifier;identifier_nondigit,FIdentifier_nondigit' | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FIdentifier_nondigit']" "fragment" | \
	trkleene "//lexerRuleSpec/TOKEN_REF[text()='Identifier']" | \
	trsponge -c true

echo ""
echo "For now, we comment out preprocessing rules until we know what to do."
trparse c_plus_plus_spec_draft.g4 | \
	trinsert "//ruleSpec/parserRuleSpec/RULE_REF[text()='preprocessing_token' or text()='token' or text()='header_name' or text()='h_char_sequence' or text()='h_char' or text()='q_char_sequence' or text()='q_char' or text()='pp_number' or text()='preprocessing_file' or text()='group' or text()='group_part' or text()='if_section' or text()='if_group' or text()='elif_groups' or text()='elif_group' or text()='else_group' or text()='endif_line' or text()='control_line' or text()='text_line' or text()='non_directive' or text()='lparen' or text()='replacement_list' or text()='pp_tokens' or text()='new_line']" '// ' | \
	trsponge -c true


	
exit 0

echo Rewriting lexer String rules ...
trparse c_plus_plus_spec_draft.g4 | \
	trrename -r 'string_literal,String_literal;encoding_prefix,Encoding_prefix;s_char_sequence,S_char_sequence;raw_string,Raw_string;s_char,S_char;escape_sequence,Escape_sequence;universal_character_name,Universal_character_name;hex_quad,Hex_quad;hexadecimal_digit,Hexadecimal_digit' | \
	trsponge -c true
