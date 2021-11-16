#

echo "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"

echo "Building scraper and generating a grammar from scratch."
name=CPlusPlus14
grep -v '^//' scraper/c++14.g4 > ./$name.g4

# Apply trkleene to the entire grammar to remove obvious recursions.
echo ""
echo "Rewrite recursion into kleene."
trparse $name.g4 | \
	trkleene | \
	trsponge -c true

# Change the start rules for the grammar. Here we just
# add an EOF to the end of each rule. There are actually three start
# rules, two for preprocessing, the third for parsing C++ code after
# preprocessing.
echo ""
echo "Fix start rules ..."
trparse $name.g4 | \
	trinsert "//parserRuleSpec[RULE_REF/text()='translation_unit' or RULE_REF/text()='preprocessing_file']/SEMI" "EOF" | \
	trinsert "//parserRuleSpec[RULE_REF/text()='constant_expression']" "constant_expression_eof :  conditional_expression EOF ;" | \
	trsponge -c true

# The ISO C++ grammar uses a naming convention that isn't the same as
# Antlr. Rename several RULE_REF symbols into lexer TOKEN_REF.
echo ""
echo "Renaming several basic literal rules..."
trparse $name.g4 | \
	trrename -r 'universal_character_name,FUniversal_character_name;hex_quad,FHex_quad;hexadecimal_digit,FHexadecimal_digit;binary_digit,FBinary_digit;octal_digit,FOctal_digit;nonzero_digit,FNonzero_digit;unsigned_suffix,FUnsigned_suffix;long_suffix,FLong_suffix;long_long_suffix,FLong_long_suffix;encoding_prefix,FEncoding_prefix' | \
	trrename -r 'sign,FSign;exponent_part,FExponent_part;digit_sequence,FDigit_sequence;fractional_constant,FFractional_constant;hexadecimal_escape_sequence,FHexadecimal_escape_sequence;octal_escape_sequence,FOctal_escape_sequence;simple_escape_sequence,FSimple_escape_sequence;escape_sequence,FEscape_sequence;c_char,FC_char;c_char_sequence,FC_char_sequence;digit,FDigit;nondigit,FNondigit;floating_suffix,FFloating_suffix;integer_suffix,FInteger_suffix' | \
	trrename -r 'h_char_sequence,FH_char_sequence;h_char,FH_char;q_char_sequence,FQ_char_sequence;q_char,FQ_char;s_char,FS_Char;s_char_sequence,FS_char_sequence;r_char,FR_char;r_char_sequence,FR_char_sequence;d_char,FD_char;d_char_sequence,FD_char_sequence' | \
	trrename -r 'header_name,Header_name' | \
	trrename -r 'identifier,Identifier;identifier_nondigit,FIdentifier_nondigit' | \
	trrename -r 'raw_string,FRaw_string' | \
	trsponge -c true

echo ""
echo "Taking care of several string literals"
echo 1
trparse $name.g4 | \
	trreplace "//STRING_LITERAL[text()='''any member of the source character set except new_line and >''']" "~[ <\t\n>]" | \
	trsponge -c true
echo 2
trparse $name.g4 | \
	trreplace "//STRING_LITERAL[text()='''any member of the source character set except new_line and \"''']" "~[ \t\n\"]" | \
	trsponge -c true
echo 3
trparse $name.g4 | \
	trreplace "//STRING_LITERAL[text()='''any member of the source character set except the single_quote \\'', backslash \\\\, or new_line character''']" "~['\\\r\n]" | \
	trsponge -c true
echo 4
trparse $name.g4 | \
	trreplace "//STRING_LITERAL[text()='''any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote \".''']"  '~[)\"]' | \
	trsponge -c true
echo 5
trparse $name.g4 | \
	trreplace "//STRING_LITERAL[text()='''any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\\\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.''']"  '~[ ()\\\r\n\t\u000B]' | \
	trsponge -c true

echo ""
echo "Adding 'fragment' to selected lexer rules."
trparse $name.g4 | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FHex_quad' or text()='FUniversal_character_name' or text()='FBinary_digit' or text()='FOctal_digit' or text()='FNonzero_digit' or text()='FHexadecimal_digit' or text()='FUnsigned_suffix' or text()='FLong_suffix' or text()='FLong_long_suffix' or text()='FEncoding_prefix']" "fragment" | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FSign' or text()='FExponent_part' or text()='FDigit_sequence' or text()='FFractional_constant' or text()='FOctal_escape_sequence' or text()='FSimple_escape_sequence' or text()='FEscape_sequence' or text()='FC_char' or text()='FDigit' or text()='FNondigit' or text()='FFloating_suffix' or text()='FInteger_suffix']" "fragment" | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FC_char_sequence' or text()='FHexadecimal_escape_sequence']" "fragment" | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FH_char_sequence' or text()='FH_char' or text()='FQ_char_sequence' or text()='FQ_char' or text()='FS_Char' or text()='FS_char_sequence' or text()='FR_char' or text()='FR_char_sequence' or text()='FD_char' or text()='FD_char_sequence']" "fragment" | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FRaw_string']" "fragment" | \
	trsponge -c true

echo ""
echo "Renaming literals"
trparse $name.g4 | \
	trrename -r 'integer_literal,Integer_literal;decimal_literal,Decimal_literal;octal_literal,Octal_literal;hexadecimal_literal,Hexadecimal_literal;binary_literal,Binary_literal;character_literal,Character_literal;floating_literal,Floating_literal;string_literal,String_literal' | \
	trrename -r 'user_defined_literal,User_defined_literal;user_defined_integer_literal,User_defined_integer_literal;user_defined_floating_literal,User_defined_floating_literal;user_defined_string_literal,User_defined_string_literal;user_defined_character_literal,User_defined_character_literal;ud_suffix,FUd_suffix' | \
	trsponge -c true

echo ""
echo "Fixing identifer rules..."
trparse $name.g4 | \
	trinsert "//ruleSpec/lexerRuleSpec/TOKEN_REF[text()='FIdentifier_nondigit']" "fragment" | \
	trsponge -c true

echo ""
echo "Splitting"
trparse $name.g4 | \
	trsplit | \
	trsponge -c true

# The typedef_name and namespace_name rules are not used in parsing.
# Comment out these two productions.
# These are done after splitting so that the comments go to the parser.
echo ""
echo "Comment keyword productions."
trparse "$name"Parser.g4 | \
	trinsert "//parserRuleSpec[RULE_REF/text()='typedef_name' or RULE_REF/text()='namespace_name']" "//" | \
	trsponge -c true

# This has to be done in order for tr
echo ""
echo "Adding in superClass option."
trparse "$name"Parser.g4 | \
	trinsert -a "//optionsSpec[option/identifier/RULE_REF/text()='tokenVocab']/SEMI" "superClass=ParserBase;" | \
	trsponge -c true

echo ""
echo "Fixing balanced_token"
trparse "$name"Lexer.g4 | \
	trreplace "//TOKEN_REF[text()='RESTRICTED_CHARS9']" "~( '(' | ')' | '{' | '}' | '[' | ']' )+" | \
	trsponge -c true

trparse "$name"Parser.g4 | \
	trreplace "//parserRuleSpec[RULE_REF/text()='pure_specifier']" "pure_specifier :  '=' Octal_literal ;" | \
	trsponge -c true
	
echo ""
echo "Fixing alternative operators Table 6"
trparse "$name"Parser.g4 | \
	trreplace "//parserRuleSpec[RULE_REF/text()='logical_and_expression']//STRING_LITERAL" "( '&&' | 'and' )" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='assignment_operator']//SEMI" "| 'and_eq' | 'or_eq' | 'xor_eq' ;" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='and_expression']//STRING_LITERAL" "( '&' | 'bitand' )" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='inclusive_or_expression']//STRING_LITERAL" "( '|' | 'bitor' )" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='logical_or_expression']//STRING_LITERAL" "( '||' | 'or' )" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='exclusive_or_expression']//STRING_LITERAL" "( '^' | 'xor' )" | \
	trreplace "//parserRuleSpec[RULE_REF/text()='unary_operator']//SEMI" "| 'not' | 'compl' ;" | \
	trsponge -c true

xx="; `cat addin`"
trparse "$name"Lexer.g4 | \
	trreplace "//grammarSpec/grammarDecl/SEMI" "$xx" | \
	trsponge -c true

cat "$name"Lexer.g4 | unix2dos -f > temp."$name"Lexer.g4
mv temp."$name"Lexer.g4 "$name"Lexer.g4

echo ""
echo "Folding lexer symbols"
trparse "$name"Parser.g4 | \
	trfoldlit | \
	trsponge -c true



trparse "$name"Lexer.g4 | \
	trmove -a "//ruleSpec[lexerRuleSpec/FRAGMENT]" "//grammarSpec/grammarDecl/SEMI" | \
	trsponge -c true

	
exit 0

echo ""
echo "For now, we comment out preprocessing rules until we know what to do."
trparse Scrape.g4 | \
	trinsert "//ruleSpec/parserRuleSpec/RULE_REF[text()='preprocessing_token' or text()='token' or text()='pp_number' or text()='preprocessing_file' or text()='group' or text()='group_part' or text()='if_section' or text()='if_group' or text()='elif_groups' or text()='elif_group' or text()='else_group' or text()='endif_line' or text()='control_line' or text()='text_line' or text()='non_directive' or text()='lparen' or text()='replacement_list' or text()='pp_tokens' or text()='new_line']" '// ' | \
	trsponge -c true

	