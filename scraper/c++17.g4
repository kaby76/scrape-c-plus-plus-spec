
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

grammar Scrape;
typedef_name :  identifier ;
namespace_name :  identifier |  namespace_alias ;
namespace_alias :  identifier ;
class_name :  identifier |  simple_template_id ;
enum_name :  identifier ;
template_name :  identifier ;
hex_quad :  hexadecimal_digit hexadecimal_digit hexadecimal_digit hexadecimal_digit ;
universal_character_name :  '\\u' hex_quad |  '\\U' hex_quad hex_quad ;
preprocessing_token :  header_name |  identifier |  pp_number |  character_literal |  user_defined_character_literal |  string_literal |  user_defined_string_literal |  preprocessing_op_or_punc |  'each non_white_space character that cannot be one of the above' ;
token :  identifier |  keyword |  literal |  operator |  punctuator ;
header_name :  '<' h_char_sequence '>' |  '"' q_char_sequence '"' ;
h_char_sequence :  h_char |  h_char_sequence h_char ;
h_char :  'any member of the source character set except new_line and >' ;
q_char_sequence :  q_char |  q_char_sequence q_char ;
q_char :  'any member of the source character set except new_line and "' ;
pp_number :  digit |  '.' digit |  pp_number digit |  pp_number identifier_nondigit |  pp_number '\'' digit |  pp_number '\'' nondigit |  pp_number 'e' sign |  pp_number 'E' sign |  pp_number 'p' sign |  pp_number 'P' sign |  pp_number '.' ;
identifier :  identifier_nondigit |  identifier identifier_nondigit |  identifier digit ;
identifier_nondigit :  nondigit |  universal_character_name ;
nondigit :  'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x' | 'y' | 'z' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' | 'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P' | 'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' | 'Y' | 'Z' | '_' ;
digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
preprocessing_op_or_punc :  '{' | '}' | '[' | ']' | '#' | '##' | '(' | ')' | '<:' | ':>' | '<%' | '%>' | '%:' | '%:%:' | ';' | ':' | '...' | 'new' | 'delete' | '?' | '::' | '.' | '.*' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | 'and' | 'and_eq' | 'bitand' | 'bitor' | 'compl' | 'not' | 'not_eq' | 'or' | 'or_eq' | 'xor' | 'xor_eq' ;
literal :  integer_literal |  character_literal |  floating_literal |  string_literal |  boolean_literal |  pointer_literal |  user_defined_literal ;
integer_literal :  binary_literal integer_suffix ? |  octal_literal integer_suffix ? |  decimal_literal integer_suffix ? |  hexadecimal_literal integer_suffix ? ;
binary_literal :  '0b' binary_digit |  '0B' binary_digit |  binary_literal '\'' ? binary_digit ;
octal_literal :  '0' |  octal_literal '\'' ? octal_digit ;
decimal_literal :  nonzero_digit |  decimal_literal '\'' ? digit ;
hexadecimal_literal :  hexadecimal_prefix hexadecimal_digit_sequence ;
binary_digit :  '0' |  '1' ;
octal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' ;
nonzero_digit :  '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;
hexadecimal_prefix :  '0x' | '0X' ;
hexadecimal_digit_sequence :  hexadecimal_digit |  hexadecimal_digit_sequence '\'' ? hexadecimal_digit ;
hexadecimal_digit :  '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' ;
integer_suffix :  unsigned_suffix long_suffix ? |  unsigned_suffix long_long_suffix ? |  long_suffix unsigned_suffix ? |  long_long_suffix unsigned_suffix ? ;
unsigned_suffix :  'u' | 'U' ;
long_suffix :  'l' | 'L' ;
long_long_suffix :  'll' | 'LL' ;
character_literal :  encoding_prefix ? '\'' c_char_sequence '\'' ;
encoding_prefix :  'u8' | 'u' | 'U' | 'L' ;
c_char_sequence :  c_char |  c_char_sequence c_char ;
c_char :  'any member of the source character set except the single_quote \', backslash \\, or new_line character' |  escape_sequence |  universal_character_name ;
escape_sequence :  simple_escape_sequence |  octal_escape_sequence |  hexadecimal_escape_sequence ;
simple_escape_sequence :  '\\\'' | '\\"' | '\\?' | '\\\\' | '\\a' | '\\b' | '\\f' | '\\n' | '\\r' | '\\t' | '\\v' ;
octal_escape_sequence :  '\\' octal_digit |  '\\' octal_digit octal_digit |  '\\' octal_digit octal_digit octal_digit ;
hexadecimal_escape_sequence :  '\\x' hexadecimal_digit |  hexadecimal_escape_sequence hexadecimal_digit ;
floating_literal :  decimal_floating_literal |  hexadecimal_floating_literal ;
decimal_floating_literal :  fractional_constant exponent_part ? floating_suffix ? |  digit_sequence exponent_part floating_suffix ? ;
hexadecimal_floating_literal :  hexadecimal_prefix hexadecimal_fractional_constant binary_exponent_part floating_suffix ? |  hexadecimal_prefix hexadecimal_digit_sequence binary_exponent_part floating_suffix ? ;
fractional_constant :  digit_sequence ? '.' digit_sequence |  digit_sequence '.' ;
hexadecimal_fractional_constant :  hexadecimal_digit_sequence ? '.' hexadecimal_digit_sequence |  hexadecimal_digit_sequence '.' ;
exponent_part :  'e' sign ? digit_sequence |  'E' sign ? digit_sequence ;
binary_exponent_part :  'p' sign |  ? digit_sequence |  'P' sign ? digit_sequence ;
sign :  '+' | '-' ;
digit_sequence :  digit |  digit_sequence '\'' ? digit ;
floating_suffix :  'f' | 'l' | 'F' | 'L' ;
string_literal :  encoding_prefix ? '"' s_char_sequence ? '"' |  encoding_prefix ? 'R' raw_string ;
s_char_sequence :  s_char |  s_char_sequence s_char ;
s_char :  'any member of the source character set except the double_quote ", backslash \\. or new_line character' |  escape_sequence |  universal_character_name ;
raw_string :  '"' d_char_sequence ? '(' r_char_sequence ? ')' d_char_sequence ? '"' ;
r_char_sequence :  r_char |  r_char_sequence r_char ;
r_char :  'any member of the source character set, except a right parenthesis ) followed by the initial d_char_sequence (which may be empty) followed by a double quote ".' ;
d_char_sequence :  d_char |  d_char_sequence d_char ;
d_char :  'any member of the basic source character set except: space, the left parenthesis (, the right parenthesis ), the backslash \\, and the control characters representing horizontal tab, vertical tab, form feed, and newline.' ;
boolean_literal :  'false' |  'true' ;
pointer_literal :  'nullptr' ;
user_defined_literal :  user_defined_integer_literal |  user_defined_floating_literal |  user_defined_string_literal |  user_defined_character_literal ;
user_defined_integer_literal :  decimal_literal ud_suffix |  octal_literal ud_suffix |  hexadecimal_literal ud_suffix |  binary_literal ud_suffix ;
user_defined_floating_literal :  fractional_constant exponent_part ? ud_suffix |  digit_sequence exponent_part ud_suffix |  hexadecimal_prefix hexadecimal_fractional_constant binary_exponent_part ud_suffix |  hexadecimal_prefix hexadecimal_digit_sequence binary_exponent_part ud_suffix ;
user_defined_string_literal :  string_literal ud_suffix ;
user_defined_character_literal :  character_literal ud_suffix ;
ud_suffix :  identifier ;
translation_unit :  declaration_seq ? ;
primary_expression :  literal |  'this' |  '(' expression ')' |  id_expression |  lambda_expression |  fold_expression ;
id_expression :  unqualified_id |  qualified_id ;
unqualified_id :  identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  '~' class_name |  '~' decltype_specifier |  template_id ;
qualified_id :  nested_name_specifier 'template' ? unqualified_id ;
nested_name_specifier :  '::' |  type_name '::' |  namespace_name '::' |  decltype_specifier '::' |  nested_name_specifier identifier '::' |  nested_name_specifier 'template' ? simple_template_id '::' ;
lambda_expression :  lambda_introducer lambda_declarator |  ? compound_statement ;
lambda_introducer :  '[' lambda_capture ? ']' ;
lambda_declarator :  '(' parameter_declaration_clause ')' decl_specifier_seq ? noexcept_specifier ? attribute_specifier_seq ? trailing_return_type ? ;
lambda_capture :  capture_default |  capture_list |  capture_default ',' capture_list ;
capture_default :  '&' |  '=' ;
capture_list :  capture '...' |  ? |  capture_list ',' capture '...' ? ;
capture :  simple_capture |  init_capture ;
simple_capture :  identifier |  '&' identifier |  'this' |  '*' 'this' ;
init_capture :  identifier initializer |  '&' identifier initializer ;
fold_expression :  '(' cast_expression fold_operator '...' ')' |  '(' '...' fold_operator cast_expression ')' |  '(' cast_expression fold_operator '...' fold_operator cast_expression ')' ;
fold_operator :  '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '<<' | '>>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|' | '=' | '<<=' | '>>=' | '=' | '==' | '!=' | '<' | '>' | '<=' | '>=' | '&&' | '|' | '|' | ',' | '.*' | '->*' ;
postfix_expression :  primary_expression |  postfix_expression '[' expr_or_braced_init_list ']' |  postfix_expression '(' expression_list ? ')' |  simple_type_specifier '(' expression_list ? ')' |  typename_specifier '(' expression_list ? ')' |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  postfix_expression '.' 'template' ? id_expression |  postfix_expression '->' 'template' ? id_expression |  postfix_expression '.' pseudo_destructor_name |  postfix_expression '->' pseudo_destructor_name |  postfix_expression '++' |  postfix_expression '--' |  'dynamic_cast' '<' type_id '>' '(' expression ')' |  'static_cast' '<' type_id '>' '(' expression ')' |  'reinterpret_cast' '<' type_id '>' '(' expression ')' |  'const_cast' '<' type_id '>' '(' expression ')' |  'typeid' '(' expression ')' |  'typeid' '(' type_id ')' ;
expression_list :  initializer_list ;
pseudo_destructor_name :  nested_name_specifier ? type_name '::' '~' type_name |  nested_name_specifier 'template' simple_template_id '::' '~' type_name |  '~' type_name |  '~' decltype_specifier ;
unary_expression :  postfix_expression |  '++' cast_expression |  '--' cast_expression |  unary_operator cast_expression |  'sizeof' unary_expression |  'sizeof' '(' type_id ')' |  'sizeof' '...' '(' identifier ')' |  'alignof' '(' type_id ')' |  noexcept_expression |  new_expression |  delete_expression ;
unary_operator :  '*' | '&' | '+' | '-' | '!' | '~' ;
new_expression :  '::' ? 'new' new_placement ? new_type_id new_initializer ? |  '::' ? 'new' new_placement ? '(' type_id ')' new_initializer ? ;
new_placement :  '(' expression_list ')' ;
new_type_id :  type_specifier_seq new_declarator ? ;
new_declarator :  ptr_operator new_declarator ? |  noptr_new_declarator ;
noptr_new_declarator :  '[' expression ']' attribute_specifier_seq |  ? |  noptr_new_declarator '[' constant_expression ']' attribute_specifier_seq ? ;
new_initializer :  '(' expression_list ? ')' |  braced_init_list ;
delete_expression :  '::' ? 'delete' cast_expression |  '::' ? 'delete' '[' ']' cast_expression ;
noexcept_expression :  'noexcept' '(' expression ')' ;
cast_expression :  unary_expression |  '(' type_id ')' cast_expression ;
pm_expression :  cast_expression |  pm_expression '.*' cast_expression |  pm_expression '->*' cast_expression ;
multiplicative_expression :  pm_expression |  multiplicative_expression '*' pm_expression |  multiplicative_expression '/' pm_expression |  multiplicative_expression '%' pm_expression ;
additive_expression :  multiplicative_expression |  additive_expression '+' multiplicative_expression |  additive_expression '-' multiplicative_expression ;
shift_expression :  additive_expression |  shift_expression '<<' additive_expression |  shift_expression '>>' additive_expression ;
relational_expression :  shift_expression |  relational_expression '<' shift_expression |  relational_expression '>' shift_expression |  relational_expression '<=' shift_expression |  relational_expression '>=' shift_expression ;
equality_expression :  relational_expression |  equality_expression '==' relational_expression |  equality_expression '!=' relational_expression ;
and_expression :  equality_expression |  and_expression '&' equality_expression ;
exclusive_or_expression :  and_expression |  exclusive_or_expression '^' and_expression ;
inclusive_or_expression :  exclusive_or_expression |  inclusive_or_expression '|' exclusive_or_expression ;
logical_and_expression :  inclusive_or_expression |  logical_and_expression '&&' inclusive_or_expression ;
logical_or_expression :  logical_and_expression |  logical_or_expression '||' logical_and_expression ;
conditional_expression :  logical_or_expression |  logical_or_expression '?' expression ':' assignment_expression ;
throw_expression :  'throw' assignment_expression ? ;
assignment_expression :  conditional_expression |  logical_or_expression assignment_operator initializer_clause |  throw_expression ;
assignment_operator :  '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '>>=' | '<<=' | '&=' | '^=' | '|=' ;
expression :  assignment_expression |  expression ',' assignment_expression ;
constant_expression :  conditional_expression ;
statement :  labeled_statement |  attribute_specifier_seq ? expression_statement |  attribute_specifier_seq ? compound_statement |  attribute_specifier_seq ? selection_statement |  attribute_specifier_seq ? iteration_statement |  attribute_specifier_seq ? jump_statement |  declaration_statement |  attribute_specifier_seq ? try_block ;
init_statement :  expression_statement |  simple_declaration ;
condition :  expression |  attribute_specifier_seq ? decl_specifier_seq declarator brace_or_equal_initializer ;
labeled_statement :  attribute_specifier_seq |  ? identifier ':' statement |  attribute_specifier_seq ? 'case' constant_expression ':' statement |  attribute_specifier_seq ? 'default' ':' statement ;
expression_statement :  expression ? ';' ;
compound_statement :  '{' statement_seq ? '}' ;
statement_seq :  statement |  statement_seq statement ;
selection_statement :  'if' 'constexpr' |  ? '(' init_statement ? condition ')' statement |  'if' 'constexpr' ? '(' init_statement ? condition ')' statement 'else' statement |  'switch' '(' init_statement ? condition ')' statement ;
iteration_statement :  'while' '(' condition ')' statement |  'do' statement 'while' '(' expression ')' ';' |  'for' '(' init_statement condition |  ? ';' expression ? ')' statement |  'for' '(' for_range_declaration ':' for_range_initializer ')' statement ;
for_range_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq ref_qualifier ? '[' identifier_list ']' ;
for_range_initializer :  expr_or_braced_init_list ;
jump_statement :  'break' ';' |  'continue' ';' |  'return' expr_or_braced_init_list |  ? ';' |  'goto' identifier ';' ;
declaration_statement :  block_declaration ;
declaration_seq :  declaration |  declaration_seq declaration ;
declaration :  block_declaration |  nodeclspec_function_declaration |  function_definition |  template_declaration |  deduction_guide |  explicit_instantiation |  explicit_specialization |  linkage_specification |  namespace_definition |  empty_declaration |  attribute_declaration ;
block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;
nodeclspec_function_declaration :  attribute_specifier_seq ? declarator ';' ;
alias_declaration :  'using' identifier attribute_specifier_seq |  ? '=' defining_type_id ';' ;
simple_declaration :  decl_specifier_seq init_declarator_list ? ';' |  attribute_specifier_seq decl_specifier_seq init_declarator_list ';' |  attribute_specifier_seq ? decl_specifier_seq ref_qualifier ? '[' identifier_list ']' initializer ';' ;
static_assert_declaration :  'static_assert' '(' constant_expression ')' ';' |  'static_assert' '(' constant_expression ',' string_literal ')' ';' ;
empty_declaration :  ';' ;
attribute_declaration :  attribute_specifier_seq ';' ;
decl_specifier :  storage_class_specifier |  defining_type_specifier |  function_specifier |  'friend' |  'typedef' |  'constexpr' |  'inline' ;
decl_specifier_seq :  decl_specifier attribute_specifier_seq ? |  decl_specifier decl_specifier_seq ;
storage_class_specifier :  'static' |  'thread_local' |  'extern' |  'mutable' ;
function_specifier :  'virtual' |  'explicit' ;
typedef_name :  identifier ;
type_specifier :  simple_type_specifier |  elaborated_type_specifier |  typename_specifier |  cv_qualifier ;
type_specifier_seq :  type_specifier attribute_specifier_seq ? |  type_specifier type_specifier_seq ;
defining_type_specifier :  type_specifier |  class_specifier |  enum_specifier ;
defining_type_specifier_seq :  defining_type_specifier attribute_specifier_seq ? |  defining_type_specifier defining_type_specifier_seq ;
simple_type_specifier :  nested_name_specifier |  ? type_name |  nested_name_specifier 'template' simple_template_id |  nested_name_specifier ? template_name |  'char' |  'char16_t' |  'char32_t' |  'wchar_t' |  'bool' |  'short' |  'int' |  'long' |  'signed' |  'unsigned' |  'float' |  'double' |  'void' |  'auto' |  decltype_specifier ;
type_name :  class_name |  enum_name |  typedef_name |  simple_template_id ;
decltype_specifier :  'decltype' '(' expression ')' |  'decltype' '(' 'auto' ')' ;
elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? identifier |  class_key simple_template_id |  class_key nested_name_specifier 'template' ? simple_template_id |  'enum' nested_name_specifier ? identifier ;
enum_name :  identifier ;
enum_specifier :  enum_head '{' enumerator_list ? '}' |  enum_head '{' enumerator_list ',' '}' ;
enum_head :  enum_key attribute_specifier_seq ? enum_head_name ? enum_base ? ;
enum_head_name :  nested_name_specifier ? identifier ;
opaque_enum_declaration :  enum_key attribute_specifier_seq ? nested_name_specifier ? identifier enum_base ? ';' ;
enum_key :  'enum' |  'enum' 'class' |  'enum' 'struct' ;
enum_base :  ':' type_specifier_seq ;
enumerator_list :  enumerator_definition |  enumerator_list ',' enumerator_definition ;
enumerator_definition :  enumerator |  enumerator '=' constant_expression ;
enumerator :  identifier attribute_specifier_seq ? ;
namespace_name :  identifier |  namespace_alias ;
namespace_definition :  named_namespace_definition |  unnamed_namespace_definition |  nested_namespace_definition ;
named_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? identifier '{' namespace_body '}' ;
unnamed_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? '{' namespace_body '}' ;
nested_namespace_definition :  'namespace' enclosing_namespace_specifier '::' identifier '{' namespace_body '}' ;
enclosing_namespace_specifier :  identifier |  enclosing_namespace_specifier '::' identifier ;
namespace_body :  declaration_seq ? ;
namespace_alias :  identifier ;
namespace_alias_definition :  'namespace' identifier '=' qualified_namespace_specifier ';' ;
qualified_namespace_specifier :  nested_name_specifier ? namespace_name ;
using_declaration :  'using' using_declarator_list ';' ;
using_declarator_list :  using_declarator '...' ? |  using_declarator_list ',' using_declarator '...' ? ;
using_declarator :  'typename' ? nested_name_specifier unqualified_id ;
using_directive :  attribute_specifier_seq ? 'using' 'namespace' nested_name_specifier ? namespace_name ';' ;
asm_definition :  attribute_specifier_seq |  ? 'asm' '(' string_literal ')' ';' ;
linkage_specification :  'extern' string_literal '{' declaration_seq ? '}' |  'extern' string_literal declaration ;
attribute_specifier_seq :  attribute_specifier_seq ? attribute_specifier ;
attribute_specifier :  '[' '[' attribute_using_prefix |  ? attribute_list ']' ']' |  alignment_specifier ;
alignment_specifier :  'alignas' '(' type_id '...' ? ')' |  'alignas' '(' constant_expression '...' ? ')' ;
attribute_using_prefix :  'using' attribute_namespace ':' ;
attribute_list :  attribute ? |  attribute_list ',' attribute ? |  attribute '...' |  attribute_list ',' attribute '...' ;
attribute :  attribute_token attribute_argument_clause ? ;
attribute_token :  identifier |  attribute_scoped_token ;
attribute_scoped_token :  attribute_namespace '::' identifier ;
attribute_namespace :  identifier ;
attribute_argument_clause :  '(' balanced_token_seq |  ? ')' ;
balanced_token_seq :  balanced_token |  balanced_token_seq balanced_token ;
balanced_token :  '(' balanced_token_seq ? ')' |  '[' balanced_token_seq ? ']' |  '{' balanced_token_seq ? '}' |  'any token other than a parenthesis, a bracket, or a brace' ;
init_declarator_list :  init_declarator |  init_declarator_list ',' init_declarator ;
init_declarator :  declarator initializer |  ? ;
declarator :  ptr_declarator |  noptr_declarator parameters_and_qualifiers trailing_return_type ;
ptr_declarator :  noptr_declarator |  ptr_operator ptr_declarator ;
noptr_declarator :  declarator_id attribute_specifier_seq ? |  noptr_declarator parameters_and_qualifiers |  noptr_declarator '[' constant_expression ? ']' attribute_specifier_seq ? |  '(' ptr_declarator ')' ;
parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? ref_qualifier ? noexcept_specifier ? attribute_specifier_seq ? ;
trailing_return_type :  '->' type_id ;
ptr_operator :  '*' attribute_specifier_seq ? cv_qualifier_seq ? |  '&' attribute_specifier_seq ? |  '&&' attribute_specifier_seq ? |  nested_name_specifier '*' attribute_specifier_seq ? cv_qualifier_seq ? ;
cv_qualifier_seq :  cv_qualifier cv_qualifier_seq ? ;
cv_qualifier :  'const' |  'volatile' ;
ref_qualifier :  '&' |  '&&' ;
declarator_id :  '...' ? id_expression ;
type_id :  type_specifier_seq abstract_declarator ? ;
defining_type_id :  defining_type_specifier_seq abstract_declarator ? ;
abstract_declarator :  ptr_abstract_declarator |  noptr_abstract_declarator ? parameters_and_qualifiers trailing_return_type |  abstract_pack_declarator ;
ptr_abstract_declarator :  noptr_abstract_declarator |  ptr_operator ptr_abstract_declarator ? ;
noptr_abstract_declarator :  noptr_abstract_declarator ? parameters_and_qualifiers |  noptr_abstract_declarator ? '[' constant_expression ? ']' attribute_specifier_seq ? |  '(' ptr_abstract_declarator ')' ;
abstract_pack_declarator :  noptr_abstract_pack_declarator |  ptr_operator abstract_pack_declarator ;
noptr_abstract_pack_declarator :  noptr_abstract_pack_declarator parameters_and_qualifiers |  noptr_abstract_pack_declarator '[' constant_expression ? ']' attribute_specifier_seq ? |  '...' ;
parameter_declaration_clause :  parameter_declaration_list ? '...' ? |  parameter_declaration_list ',' '...' ;
parameter_declaration_list :  parameter_declaration |  parameter_declaration_list ',' parameter_declaration ;
parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator '=' initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? '=' initializer_clause ;
function_definition :  attribute_specifier_seq ? decl_specifier_seq ? declarator virt_specifier_seq ? function_body ;
function_body :  ctor_initializer |  ? compound_statement |  function_try_block |  '=' 'default' ';' |  '=' 'delete' ';' ;
initializer :  brace_or_equal_initializer |  '(' expression_list ')' ;
brace_or_equal_initializer :  '=' initializer_clause |  braced_init_list ;
initializer_clause :  assignment_expression |  braced_init_list ;
initializer_list :  initializer_clause '...' ? |  initializer_list ',' initializer_clause '...' ? ;
braced_init_list :  '{' initializer_list ',' ? '}' |  '{' '}' ;
expr_or_braced_init_list :  expression |  braced_init_list ;
class_name :  identifier |  simple_template_id ;
class_specifier :  class_head '{' member_specification ? '}' ;
class_head :  class_key attribute_specifier_seq ? class_head_name class_virt_specifier ? base_clause ? |  class_key attribute_specifier_seq ? base_clause ? ;
class_head_name :  nested_name_specifier ? class_name ;
class_virt_specifier :  'final' ;
class_key :  'class' |  'struct' |  'union' ;
member_specification :  member_declaration member_specification ? |  access_specifier ':' member_specification ? ;
member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? ';' |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  deduction_guide |  alias_declaration |  empty_declaration ;
member_declarator_list :  member_declarator |  member_declarator_list ',' member_declarator ;
member_declarator :  declarator virt_specifier_seq |  ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  identifier ? attribute_specifier_seq ? ':' constant_expression ;
virt_specifier_seq :  virt_specifier |  virt_specifier_seq virt_specifier ;
virt_specifier :  'override' |  'final' ;
pure_specifier :  '=' '0' ;
base_clause :  ':' base_specifier_list ;
base_specifier_list :  base_specifier '...' ? |  base_specifier_list ',' base_specifier '...' ? ;
base_specifier :  attribute_specifier_seq ? class_or_decltype |  attribute_specifier_seq ? 'virtual' access_specifier ? class_or_decltype |  attribute_specifier_seq ? access_specifier 'virtual' ? class_or_decltype ;
class_or_decltype :  nested_name_specifier |  ? class_name |  nested_name_specifier 'template' simple_template_id |  decltype_specifier ;
access_specifier :  'private' |  'protected' |  'public' ;
conversion_function_id :  'operator' conversion_type_id ;
conversion_type_id :  type_specifier_seq conversion_declarator ? ;
conversion_declarator :  ptr_operator conversion_declarator ? ;
ctor_initializer :  ':' mem_initializer_list ;
mem_initializer_list :  mem_initializer '...' ? |  mem_initializer_list ',' mem_initializer '...' ? ;
mem_initializer :  mem_initializer_id '(' expression_list ? ')' |  mem_initializer_id braced_init_list ;
mem_initializer_id :  class_or_decltype |  identifier ;
operator_function_id :  'operator' operator ;
operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '~' | '!' | '=' | '<' | '>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '>=' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' | '(' ')' | '[' ']' ;
literal_operator_id :  'operator' string_literal identifier |  'operator' user_defined_string_literal ;
template_declaration :  'template' '<' template_parameter_list '>' declaration ;
template_parameter_list :  template_parameter |  template_parameter_list ',' template_parameter ;
template_parameter :  type_parameter |  parameter_declaration ;
type_parameter :  type_parameter_key '...' ? identifier ? |  type_parameter_key identifier ? '=' type_id |  'template' '<' template_parameter_list '>' type_parameter_key '...' ? identifier ? |  'template' '<' template_parameter_list '>' type_parameter_key identifier ? '=' id_expression ;
type_parameter_key :  'class' |  'typename' ;
simple_template_id :  template_name '<' template_argument_list ? '>' ;
template_id :  simple_template_id |  operator_function_id '<' template_argument_list ? '>' |  literal_operator_id '<' template_argument_list ? '>' ;
template_name :  identifier ;
template_argument_list :  template_argument '...' ? |  template_argument_list ',' template_argument '...' ? ;
template_argument :  constant_expression |  type_id |  id_expression ;
typename_specifier :  'typename' nested_name_specifier identifier |  'typename' nested_name_specifier 'template' |  ? simple_template_id ;
explicit_instantiation :  'extern' ? 'template' declaration ;
explicit_specialization :  'template' '<' '>' declaration ;
deduction_guide :  'explicit' ? template_name '(' parameter_declaration_clause ')' '->' simple_template_id ';' ;
try_block :  'try' compound_statement handler_seq ;
function_try_block :  'try' ctor_initializer ? compound_statement handler_seq ;
handler_seq :  handler handler_seq |  ? ;
handler :  'catch' '(' exception_declaration ')' compound_statement ;
exception_declaration :  attribute_specifier_seq ? type_specifier_seq declarator |  attribute_specifier_seq ? type_specifier_seq abstract_declarator ? |  '...' ;
noexcept_specifier :  'noexcept' '(' constant_expression ')' |  'noexcept' |  'throw' '(' ')' ;
preprocessing_file :  group ? ;
group :  group_part |  group group_part ;
group_part :  control_line |  if_section |  text_line |  '#' conditionally_supported_directive ;
control_line :  '#' 'include' pp_tokens new_line |  '#' 'define' identifier replacement_list new_line |  '#' 'define' identifier lparen identifier_list ? ')' replacement_list new_line |  '#' 'define' identifier lparen '...' ')' replacement_list new_line |  '#' 'define' identifier lparen identifier_list ',' '...' ')' replacement_list new_line |  '#' 'undef' identifier new_line |  '#' 'line' pp_tokens new_line |  '#' 'error' pp_tokens ? new_line |  '#' 'pragma' pp_tokens ? new_line |  '#' new_line ;
if_section :  if_group elif_groups ? else_group ? endif_line ;
if_group :  '#' 'if' constant_expression new_line group ? |  '#' 'ifdef' identifier new_line group ? |  '#' 'ifndef' identifier new_line group ? ;
elif_groups :  elif_group |  elif_groups elif_group ;
elif_group :  '#' 'elif' constant_expression new_line group |  ? ;
else_group :  '#' 'else' new_line group ? ;
endif_line :  '#' 'endif' new_line ;
text_line :  pp_tokens ? new_line ;
conditionally_supported_directive :  pp_tokens new_line ;
lparen :  'a ( character not immediately preceded by white_space' ;
identifier_list :  identifier |  identifier_list ',' identifier ;
replacement_list :  pp_tokens |  ? ;
pp_tokens :  preprocessing_token |  pp_tokens preprocessing_token ;
new_line :  'the new_line character' ;
defined_macro_expression :  'defined' identifier |  'defined' '(' identifier ')' ;
h_preprocessing_token :  'any preprocessing_token other than >' ;
h_pp_tokens :  h_preprocessing_token |  h_pp_tokens h_preprocessing_token ;
has_include_expression :  '__has_include' '(' '<' h_char_sequence '>' ')' |  '__has_include' '(' '"' q_char_sequence '"' ')' |  '__has_include' '(' string_literal ')' |  '__has_include' '(' '<' h_pp_tokens '>' ')' ;
