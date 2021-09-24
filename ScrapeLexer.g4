lexer grammar ScrapeLexer; 

KWAlignas: 'alignas';
KWAlignof: 'alignof';
KWAnd: 'and';
KWAndEq: 'and_eq';
KWAsm: 'asm';
KWAuto: 'auto';
KWBitAnd: 'bitand';
KWBitOr: 'bitor';
KWBool: 'bool';
KWBreak: 'break';
KWCase: 'case';
KWCatch: 'catch';
KWChar16: 'char16_t';
KWChar32: 'char32_t';
KWChar: 'char';
KWClass: 'class';
KWCompl: 'compl';
KWConst: 'const';
KWConst_cast: 'const_cast';
KWConstexpr: 'constexpr';
KWContinue: 'continue';
KWDecltype: 'decltype';
KWDefault: 'default';
KWDelete: 'delete';
KWDo: 'do';
KWDouble: 'double';
KWDynamic_cast: 'dynamic_cast';
KWElse: 'else';
KWEnum: 'enum';
KWExplicit: 'explicit';
KWExport: 'export';
KWExtern: 'extern';
KWFalse_: 'false';
KWFinal: 'final';
KWFloat: 'float';
KWFor: 'for';
KWFriend: 'friend';
KWGoto: 'goto';
KWIf: 'if';
KWInline: 'inline';
KWInt: 'int';
KWLong: 'long';
KWMutable: 'mutable';
KWNamespace: 'namespace';
KWNew: 'new';
KWNoexcept: 'noexcept';
KWNot: 'not';
KWNotEq: 'not_eq';
KWNullptr: 'nullptr';
KWOperator: 'operator';
KWOr: 'or';
KWOrEq: 'or_eq';
KWOverride: 'override';
KWPrivate: 'private';
KWProtected: 'protected';
KWPublic: 'public';
KWRegister: 'register';
KWReinterpret_cast: 'reinterpret_cast';
KWReturn: 'return';
KWShort: 'short';
KWSigned: 'signed';
KWSizeof: 'sizeof';
KWStatic: 'static';
KWStatic_assert: 'static_assert';
KWStatic_cast: 'static_cast';
KWStruct: 'struct';
KWSwitch: 'switch';
KWTemplate: 'template';
KWThis: 'this';
KWThread_local: 'thread_local';
KWThrow: 'throw';
KWTrue_: 'true';
KWTry: 'try';
KWTypedef: 'typedef';
KWTypeid_: 'typeid';
KWTypename_: 'typename';
KWUnion: 'union';
KWUnsigned: 'unsigned';
KWUsing: 'using';
KWVirtual: 'virtual';
KWVoid: 'void';
KWVolatile: 'volatile';
KWWchar: 'wchar_t';
KWWhile: 'while';
KWXor: 'xor';
KWXorEq: 'xor_eq';

/*Operators*/
And: '&';
AndAnd: '&&';
AndAssign: '&=';
Arrow: '->';
ArrowStar: '->*';
Assign: '=';
Caret: '^';
Colon: ':';
ColonGt: ':>';
Comma: ',';
Div: '/';
DivAssign: '/=';
Dot: '.';
DotStar: '.*';
Doublecolon: '::';
Ellipsis: '...';
Equal: '==';
Greater: '>';
GreaterEqual: '>=';
LeftBrace: '{';
LeftBracket: '[';
LeftParen: '(';
LeftShift: '<<';
LeftShiftAssign: '<<=';
Less: '<';
LessEqual: '<=';
LtColon: '<:';
LtPer: '<%';
Minus: '-';
MinusAssign: '-=';
MinusMinus: '--';
Mod: '%';
ModAssign: '%=';
Not: '!';
NotEqual: '!=';
Or: '|';
OrAssign: '|=';
OrOr: '||';
PerColon: '%:';
PerColonPerColon: '%:%:';
PerGt: '%>';
Plus: '+';
PlusAssign: '+=';
PlusPlus: '++';
Pound: '#';
PoundPound: '##';
Question: '?';
RightBrace: '}';
RightBracket: ']';
RightParen: ')';
RightShift: '>>';
RightShiftAssign: '>>=';
Semi: ';';
Star: '*';
StarAssign: '*=';
Tilde: '~';
XorAssign: '^=';


// A.1 Keywords 	 [gram.key] 
// typedef_name :  identifier ;
// namespace_name :  original_namespace_name |  namespace_alias ;
// namespace_alias :  identifier ;
// class_name :  identifier |  simple_template_id ;
// enum_name :  identifier ;
// template_name :  identifier ;

// A.2 Lexical conventions 	 [gram.lex] 
fragment FHex_quad :  FHexadecimal_digit FHexadecimal_digit FHexadecimal_digit FHexadecimal_digit ;
fragment FUniversal_character_name :  '\\u' FHex_quad |  '\\U' FHex_quad FHex_quad ;
//  preprocessing_token :  header_name |  Identifier |  pp_number |  Character_literal |  user_defined_character_literal |  string_literal |  user_defined_string_literal |  preprocessing_op_or_punc |  RESTRICTED_CHARS1 ;
// § A.2 	 1210  c ISO/IEC 	 N4296

//  token :  Identifier |  keyword |  literal |  operator |  punctuator ;
//  header_name :  '<' h_char_sequence '>' |  '"' q_char_sequence '"' ;
//  h_char_sequence : (  h_char ) ( h_char ) * ;
//  h_char :  RESTRICTED_CHARS2 ;
//  q_char_sequence : (  q_char ) ( q_char ) * ;
//  q_char :  RESTRICTED_CHARS3 ;
//  pp_number : (  FDigit |  '.' FDigit ) ( FDigit | FIdentifier_nondigit | '\'' FDigit | '\'' FNondigit | 'e' FSign | 'E' FSign | '.' ) * ;
Identifier : (  FIdentifier_nondigit ) ( FIdentifier_nondigit | FDigit ) * ;
fragment FIdentifier_nondigit :  FNondigit |  FUniversal_character_name ;
fragment FNondigit :  'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x' | 'y' | 'z' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' | 'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P' | 'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' | 'Y' | 'Z' | '_' ;
fragment FDigit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
Integer_literal :  Binary_literal FInteger_suffix ? |  Octal_literal FInteger_suffix ? |  Decimal_literal FInteger_suffix ? |  Hexadecimal_literal FInteger_suffix ? ;
Binary_literal : (  '0b' FBinary_digit |  '0B' FBinary_digit ) ( '’' ? FBinary_digit ) * ;
Octal_literal : (  '0' ) ( '’' ? FOctal_digit ) * ;
Decimal_literal : (  FNonzero_digit ) ( '’' ? FDigit ) * ;
Hexadecimal_literal : (  '0x' FHexadecimal_digit |  '0X' FHexadecimal_digit ) ( '’' ? FHexadecimal_digit ) * ;
fragment FBinary_digit :  '0' |  '1' ;
fragment FOctal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' ;
fragment FNonzero_digit :  '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
fragment FHexadecimal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' ;
fragment FInteger_suffix :  FUnsigned_suffix FLong_suffix ? |  FUnsigned_suffix FLong_long_suffix ? |  FLong_suffix FUnsigned_suffix ? |  FLong_long_suffix FUnsigned_suffix ? ;
// § A.2 	 1212  c ISO/IEC 	 N4296

fragment FUnsigned_suffix :  'u' | 'U' ;
fragment FLong_suffix :  'l' | 'L' ;
fragment FLong_long_suffix :  'll' | 'LL' ;
Character_literal :  FEncoding_prefix ? '\'' FC_char_sequence '\'' ;
fragment FEncoding_prefix :  'u8' | 'u' | 'U' | 'L' ;
fragment FC_char_sequence : (  FC_char ) ( FC_char ) * ;
fragment FC_char :  ~['\\r\n] |  FEscape_sequence |  FUniversal_character_name ;
fragment FEscape_sequence :  FSimple_escape_sequence |  FOctal_escape_sequence |  FHexadecimal_escape_sequence ;
fragment FSimple_escape_sequence :  '\\\'' | '\\"' | '\\?' | '\\\\' | '\\a' | '\\b' | '\\f' | '\\n' | '\\r' | '\\t' | '\\v' ;
fragment FOctal_escape_sequence :  '\\' FOctal_digit |  '\\' FOctal_digit FOctal_digit |  '\\' FOctal_digit FOctal_digit FOctal_digit ;
fragment FHexadecimal_escape_sequence : (  '\\x' FHexadecimal_digit ) ( FHexadecimal_digit ) * ;
Floating_literal :  FFractional_constant FExponent_part ? FFloating_suffix ? |  FDigit_sequence FExponent_part FFloating_suffix ? ;
fragment FFractional_constant :  FDigit_sequence ? '.' FDigit_sequence |  FDigit_sequence '.' ;
fragment FExponent_part :  'e' FSign ? FDigit_sequence |  'E' FSign ? FDigit_sequence ;
fragment FSign :  '+' | '-' ;
fragment FDigit_sequence : (  FDigit ) ( '\'' ? FDigit ) * ;
fragment FFloating_suffix :  'f' | 'l' | 'F' | 'L' ;
String_literal :  FEncoding_prefix ? '"' FS_char_sequence ? '"' | FEncoding_prefix ? 'R' FRaw_string ;
// § A.2 	 1213  c ISO/IEC 	 N4296

fragment FS_char_sequence : (  FS_char ) ( FS_char ) * ;
fragment FS_char : ~["\\\r\n] |  FEscape_sequence |  FUniversal_character_name ;
fragment FRaw_string :  '"' FD_char_sequence ? '(' FR_char_sequence ? ')' FD_char_sequence ? '"' ;
fragment FR_char_sequence : (  FR_char ) ( FR_char ) * ;
fragment FR_char :  ~[)"] ;
fragment FD_char_sequence : (  FD_char ) ( FD_char ) * ;
fragment FD_char : ~[ ()\\\r\n\t\u000B] ;
User_defined_literal :  User_defined_integer_literal |  User_defined_floating_literal |  User_defined_string_literal |  User_defined_character_literal ;
User_defined_integer_literal :  Decimal_literal FUd_suffix |  Octal_literal FUd_suffix |  Hexadecimal_literal FUd_suffix |  Binary_literal FUd_suffix ;
User_defined_floating_literal :  FFractional_constant FExponent_part ? FUd_suffix |  FDigit_sequence FExponent_part FUd_suffix ;
User_defined_string_literal :  String_literal FUd_suffix ;
User_defined_character_literal :  Character_literal FUd_suffix ;
fragment FUd_suffix :  Identifier ;
WS : [\n\r\t ]+ -> channel(HIDDEN);
COMMENT : '//' ~[\n\r]* -> channel(HIDDEN);
ML_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
Prep : '#' ~[\n\r]* -> channel(HIDDEN);
