
/*

MIT License

Copyright (c) 2021 Ken Domino

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

c ISO/IEC 2020
All rights reserved. Unless otherwise specified, or required in the context of its implementation, no part of this publication may
be reproduced or utilized otherwise in any form or by any means, electronic or mechanical, including photocopying, or posting
on the internet or an intranet, without prior written permission. Permission can be requested from either ISO at the address
below or ISO's member body in the country of the requester.
ISO copyright office
CP 401  Ch. de Blandonnet 8
CH-1214 Vernier, Geneva
Phone: +41 22 749 01 11
Email: copyright@iso.org
Website: www.iso.org
Published in Switzerland

*/

lexer grammar CPlusPlus14Lexer; 
// Defs from "addin".

options { superClass=LexerBase; }

tokens { KWDefine, KWDefined, KWInclude, KWUndef, KWIfndef, KWIfdef, KWElse, KWEndif, KWIf, KWPragma, KWElif, KWLine, KWError, KWWarning, Newline }

fragment FHex_quad :  FHexadecimal_digit FHexadecimal_digit FHexadecimal_digit FHexadecimal_digit ;
fragment FUniversal_character_name :  '\\u' FHex_quad |  '\\U' FHex_quad FHex_quad ;
fragment FHeader_name :  '<' FH_char_sequence '>' |  '"' FQ_char_sequence '"' ;
fragment FH_char_sequence :  FH_char+ ;
fragment FH_char :  ~[ <\t\n>] ;
fragment FQ_char_sequence :  FQ_char+ ;
fragment FQ_char :  ~[ \t\n"] ;
fragment FIdentifier_nondigit :  FNondigit |  FUniversal_character_name |  'other implementation_defined characters' ;
fragment FNondigit :  'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x' | 'y' | 'z' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' | 'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P' | 'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' | 'Y' | 'Z' | '_' ;
fragment FDigit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
fragment FNonzero_digit :  '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
fragment FOctal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' ;
fragment FHexadecimal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' ;
fragment FBinary_digit :  '0' |  '1' ;
fragment FInteger_suffix :  FUnsigned_suffix FLong_suffix ? |  FUnsigned_suffix FLong_long_suffix ? |  FLong_suffix FUnsigned_suffix ? |  FLong_long_suffix FUnsigned_suffix ? ;
fragment FUnsigned_suffix :  'u' | 'U' ;
fragment FLong_suffix :  'l' | 'L' ;
fragment FLong_long_suffix :  'll' | 'LL' ;
fragment FC_char_sequence :  FC_char+ ;
fragment FC_char :  ~['\\r\n] |  FEscape_sequence |  FUniversal_character_name ;
fragment FEscape_sequence :  FSimple_escape_sequence |  FOctal_escape_sequence |  FHexadecimal_escape_sequence ;
fragment FSimple_escape_sequence :  '\\\'' | '\\"' | '\\?' | '\\\\' | '\\a' | '\\b' | '\\f' | '\\n' | '\\r' | '\\t' | '\\v' ;
fragment FOctal_escape_sequence :  '\\' FOctal_digit |  '\\' FOctal_digit FOctal_digit |  '\\' FOctal_digit FOctal_digit FOctal_digit ;
fragment FHexadecimal_escape_sequence : (  '\\x' FHexadecimal_digit ) FHexadecimal_digit* ;
fragment FFractional_constant :  FDigit_sequence ? '.' FDigit_sequence |  FDigit_sequence '.' ;
fragment FExponent_part :  'e' FSign ? FDigit_sequence |  'E' FSign ? FDigit_sequence ;
fragment FSign :  '+' | '-' ;
fragment FDigit_sequence :  FDigit ( '\'' ? FDigit )* ;
fragment FFloating_suffix :  'f' | 'l' | 'F' | 'L' ;
fragment FEncoding_prefix :  'u8' |  'u' |  'U' |  'L' ;
fragment FS_char_sequence :  FS_Char+ ;
fragment FS_Char :  'any member of the source character set except the double_quote ", backslash \\. or new_line character' |  FEscape_sequence |  FUniversal_character_name ;
fragment FRaw_string :  '"' FD_char_sequence ? '(' FR_char_sequence ? ')' FD_char_sequence ? '"' ;
fragment FR_char_sequence :  FR_char+ ;
fragment FR_char :  ~[)"] ;
fragment FD_char_sequence :  FD_char+ ;
fragment FD_char :  ~[ ()\\\r\n\t\u000B] ;
fragment FUd_suffix :  Identifier ;

KWGnuAttribute: '__attribute__';
KWAlignas: 'alignas';
KWAlignof: 'alignof'
// GNU
| '__alignof__'
;
KWAnd: 'and';
KWAndEq: 'and_eq';
KWAsm: 'asm'
// GNU
 | '__asm__'
;
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
KWTypeid_: 'typeid'
// GNU
| '__typeof__'
| '__typeof'
;
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
// RightShift: '>>';
RightShiftAssign: '>>=';
Semi: ';';
Star: '*';
StarAssign: '*=';
Tilde: '~';
XorAssign: '^=';
Identifier :  FIdentifier_nondigit ( FIdentifier_nondigit | FDigit )* ;
Integer_literal :  Decimal_literal FInteger_suffix ? |  Octal_literal FInteger_suffix ? |  Hexadecimal_literal FInteger_suffix ? |  Binary_literal FInteger_suffix ? ;
Decimal_literal :  FNonzero_digit ( '\'' ? FDigit )* ;
Octal_literal :  '0' ( '\'' ? FOctal_digit )* ;
Hexadecimal_literal : (  '0x' FHexadecimal_digit |  '0X' FHexadecimal_digit ) ( '\'' ? FHexadecimal_digit )* ;
Binary_literal : (  '0b' FBinary_digit |  '0B' FBinary_digit ) ( '\'' ? FBinary_digit )* ;
Character_literal :  '\'' FC_char_sequence '\'' |  'u' '\'' FC_char_sequence '\'' |  'U' '\'' FC_char_sequence '\'' |  'L' '\'' FC_char_sequence '\'' ;
Floating_literal :  FFractional_constant FExponent_part ? FFloating_suffix ? |  FDigit_sequence FExponent_part FFloating_suffix ? ;
String_literal :  FEncoding_prefix ? '"' FS_char_sequence ? '"' |  FEncoding_prefix ? 'R' FRaw_string ;
User_defined_literal :  User_defined_integer_literal |  User_defined_floating_literal |  User_defined_string_literal |  User_defined_character_literal ;
User_defined_integer_literal :  Decimal_literal FUd_suffix |  Octal_literal FUd_suffix |  Hexadecimal_literal FUd_suffix |  Binary_literal FUd_suffix ;
User_defined_floating_literal :  FFractional_constant FExponent_part ? FUd_suffix |  FDigit_sequence FExponent_part FUd_suffix ;
User_defined_string_literal :  String_literal FUd_suffix ;
User_defined_character_literal :  Character_literal FUd_suffix ;

// Defs from "addin2".

WS : [\n\r\t ]+ -> channel(HIDDEN);
//Newline: [\n\r];
COMMENT : '//' ~[\n\r]* -> channel(HIDDEN);
ML_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
Prep : '#' ~[\n\r]* -> channel(HIDDEN);

mode PP;

PPCOMMENT : '//' ~[\n\r]* -> channel(HIDDEN);
PPML_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
PPKWAnd: 'and' -> type(KWAnd);
PPKWAndEq: 'and_eq' -> type(KWAndEq);
PPKWBitAnd: 'bitand' -> type(KWBitAnd);
PPKWBitOr: 'bitor' -> type(KWBitOr);
PPKWCompl: 'compl' -> type(KWCompl);
PPKWDefine: 'define' -> type(KWDefine);
PPKWDefined: 'defined' -> type(KWDefined);
PPKWDelete: 'delete' -> type(KWDelete);
PPKWElif: 'elif' -> type(KWElif);
PPKWElse: 'else' -> type(KWElse);
PPKWEndif: 'endif' -> type(KWEndif);
PPKWError: 'error' -> type(KWError);
PPKWWarning: 'warning' -> type(KWWarning);
PPKWFalse: 'false' -> type(KWFalse_);
PPKWTrue: 'true' -> type(KWTrue_);
PPKWIf: 'if' -> type(KWIf);
PPKWIfdef: 'ifdef' -> type(KWIfdef);
PPKWIfndef: 'ifndef' -> type(KWIfndef);
PPKWInclude: 'include' -> type(KWInclude);
PPKWLine: 'line' -> type(KWLine);
PPKWNew: 'new' -> type(KWNew);
PPKWNot: 'not' -> type(KWNot);
PPKWNotEq: 'not_eq' -> type(KWNotEq);
PPKWOr: 'or' -> type(KWOr);
PPKWOrEq: 'or_eq' -> type(KWOrEq);
PPKWPragma: 'pragma' -> type(KWPragma);
PPKWUndef: 'undef' -> type(KWUndef);
PPKWXor: 'xor' -> type(KWXor);
PPKWXorEq: 'xor_eq' -> type(KWXorEq);
Pp_number : (  FDigit |  '.' FDigit ) ( FDigit | FIdentifier_nondigit | '\'' FDigit | '\'' FNondigit | 'e' FSign | 'E' FSign | '.' ) * -> type(Floating_literal) ;
Header_name : FHeader_name -> type(String_literal);
PPEOL: [\r\n]+ -> type(Newline);
PPWS : [\t ]+ -> channel(HIDDEN);
PPIdentifier : (  FIdentifier_nondigit ) ( FIdentifier_nondigit | FDigit ) * -> type(Identifier);
PPLeftBrace: '{' -> type(LeftBrace);
PPRightBrace: '}' -> type(RightBrace);
PPLeftBracket: '[' -> type(LeftBracket);
PPRightBracket: ']' -> type(RightBracket);
PPPoundPound: '##' -> type(PoundPound);
PPLeftParen: '(' -> type(LeftParen);
PPRightParen: ')' -> type(RightParen);
PPLtColon: '<:' -> type(LtColon);
PPColonGt: ':>' -> type(ColonGt);
PPLtPer: '<%' -> type(LtPer);
PPPerGt: '%>' -> type(PerGt);
PPPerColon: '%:' -> type(PerColon);
PPPerColonPerColon: '%:%:' -> type(PerColonPerColon);
PPSemi: ';' -> type(Semi);
PPColon: ':' -> type(Colon);
PPEllipsis: '...' -> type(Ellipsis);
PPPound: '#' -> type(Pound);
PPQuestion: '?' -> type(Question);
PPDoublecolon: '::' -> type(Doublecolon);
PPDot: '.' -> type(Dot);
PPDotStar: '.*' -> type(DotStar);
PPPlus: '+' -> type(Plus);
PPMinus: '-' -> type(Minus);
PPAssign: '=' -> type(Assign);
PPStar: '*' -> type(Star);
PPLess: '<' -> type(Less);
PPDiv: '/' -> type(Div);
PPGreater: '>' -> type(Greater);
PPMod: '%' -> type(Mod);
PPPlusAssign: '+=' -> type(PlusAssign);
PPTilde: '~' -> type(Tilde);
PPNot: '!' -> type(Not);
PPCaret: '^' -> type(Caret);
PPMinusAssign: '-=' -> type(MinusAssign);
PPAnd: '&' -> type(And);
PPStarAssign: '*=' -> type(StarAssign);
PPOr: '|' -> type(Or);
PPDivAssign: '/=' -> type(DivAssign);
PPModAssign: '%=' -> type(ModAssign);
PPXorAssign: '^=' -> type(XorAssign);
PPAndAssign: '&=' -> type(AndAssign);
PPOrAssign: '|=' -> type(OrAssign);
PPLeftShift: '<<' -> type(LeftShift);
//PPRightShift: '>>' -> type(RightShift);
PPRightShiftAssign: '>>=' -> type(RightShiftAssign);
PPLeftShiftAssign: '<<=' -> type(LeftShiftAssign);
PPEqual: '==' -> type(Equal);
PPLessEqual: '<=' -> type(LessEqual);
PPGreaterEqual: '>=' -> type(GreaterEqual);
PPAndAnd: '&&' -> type(AndAnd);
PPOrOr: '||' -> type(OrOr);
PPPlusPlus: '++' -> type(PlusPlus);
PPMinusMinus: '--' -> type(MinusMinus);
PPComma: ',' -> type(Comma);
PPArrow: '->' -> type(Arrow);
PPArrowStar: '->*' -> type(ArrowStar);
PPNotEqual: '!=' -> type(NotEqual);
PPContinue : [\\][\r\n]+ -> channel(HIDDEN);
PPString_literal : ( FEncoding_prefix ? '"' FS_char_sequence ? '"' | FEncoding_prefix ? 'R' FRaw_string ) -> type(String_literal) ;
PPCharacter_literal :  ( FEncoding_prefix ? '\'' FC_char_sequence '\'' ) -> type(Character_literal) ;
PPAny : .;
