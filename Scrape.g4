grammar Scrape;

// A.1 Keywords 	 [gram.key] 
// typedef_name :  identifier ;
// namespace_name :  original_namespace_name |  namespace_alias ;
original_namespace_name :  Identifier ;
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
// § A.2 	 1211  c ISO/IEC

preprocessing_op_or_punc :  '{' | '}' | '[' | ']' | '#' | '##' | '(' | ')' | '<:' | ':>' | '<%' | '%>' | '%:' | '%:%:' | ';' | ':' | '...' | 'new' | 'delete' | '?' | '::' | '.' | '.*' | '+' | '-' | '=' | '*' | '<' | '/' | '>' | '%' | '+=' | '~' | '!' | '^' | '-=' | '&' | '*=' | '|' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | 'and' | 'or' | '>=' | 'and_eq' | 'or_eq' | '&&' | 'bitand' | 'xor' | '||' | 'bitor' | 'xor_eq' | '++' | 'compl' | '--' | 'not' | ',' | 'not_eq' | '->*' | '->' ;
literal :  Integer_literal |  Character_literal |  Floating_literal |  String_literal |  boolean_literal |  pointer_literal |  User_defined_literal ;
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
boolean_literal :  'false' |  'true' ;
pointer_literal :  'nullptr' ;
User_defined_literal :  User_defined_integer_literal |  User_defined_floating_literal |  User_defined_string_literal |  User_defined_character_literal ;
User_defined_integer_literal :  Decimal_literal FUd_suffix |  Octal_literal FUd_suffix |  Hexadecimal_literal FUd_suffix |  Binary_literal FUd_suffix ;
User_defined_floating_literal :  FFractional_constant FExponent_part ? FUd_suffix |  FDigit_sequence FExponent_part FUd_suffix ;
User_defined_string_literal :  String_literal FUd_suffix ;
User_defined_character_literal :  Character_literal FUd_suffix ;
fragment FUd_suffix :  Identifier ;
// § A.2 	 1214  c ISO/IEC 	 N4296


// A.3 Basic concepts 	 [gram.basic] 
translation_unit :  declaration_seq ? EOF ;

// A.4 Expressions 	 [gram.expr] 
primary_expression :  literal |  'this' |  '(' expression ')' |  id_expression |  lambda_expression |  fold_expression ;
id_expression :  unqualified_id |  qualified_id ;
unqualified_id :  Identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  '-' class_name |  '-' decltype_specifier |  template_id ;
qualified_id :  nested_name_specifier 'template' ? unqualified_id ;
nested_name_specifier : (  '::' |  type_name '::' |  namespace_name '::' |  decltype_specifier '::' ) ( Identifier '::' | 'template' ? simple_template_id '::' ) * ;
lambda_expression :  lambda_introducer lambda_declarator ? compound_statement ;
lambda_introducer :  '[' lambda_capture ? ']' ;
lambda_capture :  capture_default |  capture_list |  capture_default ',' capture_list ;
capture_default :  '&' |  '=' ;
capture_list : (  capture '...' ? ) ( ',' capture '...' ? ) * ;
capture :  simple_capture |  init_capture ;
simple_capture :  Identifier |  '&' Identifier |  'this' ;
// § A.4 	 1215  c ISO/IEC 	 N4296

init_capture :  Identifier initializer |  '&' Identifier initializer ;
lambda_declarator :  '(' parameter_declaration_clause ')' 'mutable' ? exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;
fold_expression :  '(' cast_expression fold_operator '...' ')' |  '(' '...' fold_operator cast_expression ')' |  '(' cast_expression fold_operator '...' fold_operator cast_expression ')' ;
fold_operator :  '+' | '-' | '*' | '/' | '%' | '^' | '&' | '|' | '<<' | '>>' | '+=' | '-=' | '*=' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<=' | '>>=' | '=' | '==' | '!=' | '<' | '>' | '<=' | '>=' | '&&' | '||' | ',' | '.*' | '->*' ;
postfix_expression : (  primary_expression |  simple_type_specifier '(' expression_list ? ')' |  typename_specifier '(' expression_list ? ')' |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  'dynamic_cast' '<' type_id '>' '(' expression ')' |  'static_cast' '<' type_id '>' '(' expression ')' |  'reinterpret_cast' '<' type_id '>' '(' expression ')' |  'const_cast' '<' type_id '>' '(' expression ')' |  'typeid' '(' expression ')' |  'typeid' '(' type_id ')' ) ( '[' expression ']' | '[' braced_init_list ']' | '(' expression_list ? ')' | '.' 'template' ?  id_expression | '->' 'template' ? id_expression | '.' pseudo_destructor_name | '->' pseudo_destructor_name | '++' | '--' ) * ;
expression_list :  initializer_list ;
pseudo_destructor_name :  nested_name_specifier ? type_name '::' '-' type_name |  nested_name_specifier 'template' simple_template_id '::' '-' type_name |  '-' type_name |  '-' decltype_specifier ;
unary_expression : (  'sizeof' ) * (  postfix_expression |  '++' cast_expression |  '--' cast_expression |  unary_operator cast_expression |  'sizeof' '(' type_id ')' |  'sizeof' '...' '(' Identifier ')' |  'alignof' '(' type_id ')' |  noexcept_expression |  new_expression |  delete_expression ) ;
// § A.4 	 1216  c ISO/IEC 	 N4296

unary_operator :  '*' | '&' | '+' | '-' | '!' | '~' | 'not' | 'compl' ;
new_expression :  '::' ? 'new' new_placement ? new_type_id new_initializer ? |  '::' ? 'new' new_placement ? '(' type_id ')' new_initializer ? ;
new_placement :  '(' expression_list ')' ;
new_type_id :  type_specifier_seq new_declarator ? ;
new_declarator : (  ptr_operator ) * (  noptr_new_declarator ) ;
noptr_new_declarator : (  '[' expression ']' attribute_specifier_seq ? ) ( '[' constant_expression ']' attribute_specifier_seq ? ) * ;
new_initializer :  '(' expression_list ? ')' |  braced_init_list ;
delete_expression :  '::' ? 'delete' cast_expression |  '::' ? 'delete' '[' ']' cast_expression ;
noexcept_expression :  'noexcept' '(' expression ')' ;
cast_expression : (  '(' type_id ')' ) * (  unary_expression ) ;
pm_expression : (  cast_expression ) ( '.*' cast_expression | '->*' cast_expression ) * ;
multiplicative_expression : (  pm_expression ) ( '*' pm_expression | '/' pm_expression | '%' pm_expression ) * ;
additive_expression : (  multiplicative_expression ) ( '+' multiplicative_expression | '-' multiplicative_expression ) * ;
shift_expression : (  additive_expression ) ( '<<' additive_expression | '>>' additive_expression ) * ;
relational_expression : (  shift_expression ) ( '<' shift_expression | '>' shift_expression | '<=' shift_expression | '>=' shift_expression ) * ;
equality_expression : (  relational_expression ) ( '==' relational_expression | '!=' relational_expression ) * ;
// § A.4 	 1217  c ISO/IEC 	 N4296

and_expression : (  equality_expression ) ( ( '&' | 'bitand' ) equality_expression ) * ;
exclusive_or_expression : (  and_expression ) ( ( '^' | 'xor' ) and_expression ) * ;
inclusive_or_expression : (  exclusive_or_expression ) ( ( '|' | 'bitor' ) exclusive_or_expression ) * ;
logical_and_expression : (  inclusive_or_expression ) ( ( '&&' | 'and' ) inclusive_or_expression ) * ;
logical_or_expression : (  logical_and_expression ) ( ( '||' | 'or' ) logical_and_expression ) * ;
conditional_expression :  logical_or_expression |  logical_or_expression '?' expression ':' assignment_expression ;
throw_expression :  'throw' assignment_expression ? ;
assignment_expression :  conditional_expression |  logical_or_expression assignment_operator initializer_clause |  throw_expression ;
assignment_operator :  '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '>>=' | '<<=' | '&=' | '^=' | '|=' | 'and_eq' | 'or_eq' | 'xor_eq' ;
expression : (  assignment_expression ) ( ',' assignment_expression ) * ;
constant_expression :  conditional_expression ;

// A.5 Statements 	 [gram.stmt] 
statement :  labeled_statement |  attribute_specifier_seq ? expression_statement |  attribute_specifier_seq ? compound_statement |  attribute_specifier_seq ? selection_statement |  attribute_specifier_seq ? iteration_statement |  attribute_specifier_seq ? jump_statement |  declaration_statement |  attribute_specifier_seq ? try_block ;
labeled_statement :  attribute_specifier_seq ? Identifier ':' statement |  attribute_specifier_seq ? 'case' constant_expression ':' statement |  attribute_specifier_seq ? 'default' ':' statement ;
expression_statement :  expression ? ';' ;
compound_statement :  '{' statement_seq ? '}' ;
statement_seq : (  statement ) ( statement ) * ;
// § A.5 	 1218  c ISO/IEC 	 N4296

selection_statement :  'if' '(' condition ')' statement |  'if' '(' condition ')' statement 'else' statement |  'switch' '(' condition ')' statement ;
condition :  expression |  attribute_specifier_seq ? decl_specifier_seq declarator '=' initializer_clause |  attribute_specifier_seq ? decl_specifier_seq declarator braced_init_list ;
iteration_statement :  'while' '(' condition ')' statement |  'do' statement 'while' '(' expression ')' ';' |  'for' '(' for_init_statement condition ? ';' expression ? ')' statement |  'for' '(' for_range_declaration ':' for_range_initializer ')' statement ;
for_init_statement :  expression_statement |  simple_declaration ;
for_range_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator ;
for_range_initializer :  expression |  braced_init_list ;
jump_statement :  'break' ';' |  'continue' ';' |  'return' expression ? ';' |  'return' braced_init_list ';' |  'goto' Identifier ';' ;
declaration_statement :  block_declaration ;

// A.6 Declarations 	 [gram.dcl] 
declaration_seq : (  declaration ) ( declaration ) * ;
declaration :  block_declaration |  function_definition |  template_declaration |  explicit_instantiation |  explicit_specialization |  linkage_specification |  namespace_definition |  empty_declaration |  attribute_declaration ;
block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;
// § A.6 	 1219  c ISO/IEC 	 N4296

alias_declaration :  'using' Identifier attribute_specifier_seq ? '=' type_id ';' ;
simple_declaration :  decl_specifier_seq ? init_declarator_list ? ';' |  attribute_specifier_seq decl_specifier_seq ? init_declarator_list ';' ;
static_assert_declaration :  'static_assert' '(' constant_expression ')' ';' |  'static_assert' '(' constant_expression ',' String_literal ')' ';' ;
empty_declaration :  ';' ;
attribute_declaration :  attribute_specifier_seq ';' ;
decl_specifier :  storage_class_specifier |  type_specifier |  function_specifier |  'friend' |  'typedef' |  'constexpr' ;
decl_specifier_seq : (  decl_specifier ) * (  decl_specifier attribute_specifier_seq ? ) ;
storage_class_specifier :  'register' |  'static' |  'thread_local' |  'extern' |  'mutable' ;
function_specifier :  'inline' |  'virtual' |  'explicit' ;
typedef_name :  Identifier ;
type_specifier :  trailing_type_specifier |  class_specifier |  enum_specifier ;
trailing_type_specifier :  simple_type_specifier |  elaborated_type_specifier |  typename_specifier |  cv_qualifier ;
type_specifier_seq : (  type_specifier ) * (  type_specifier attribute_specifier_seq ? ) ;
trailing_type_specifier_seq : (  trailing_type_specifier ) * (  trailing_type_specifier attribute_specifier_seq ? ) ;
// § A.6 	 1220  c ISO/IEC 	 N4296

simple_type_specifier :  nested_name_specifier ? type_name |  nested_name_specifier 'template' simple_template_id |  'char' |  'char16_t' |  'char32_t' |  'wchar_t' |  'bool' |  'short' |  'int' |  'long' |  'signed' |  'unsigned' |  'float' |  'double' |  'void' |  'auto' |  decltype_specifier ;
type_name :  class_name |  enum_name |  typedef_name |  simple_template_id ;
decltype_specifier :  'decltype' '(' expression ')' |  'decltype' '(' 'auto' ')' ;
elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? Identifier |  class_key simple_template_id |  class_key nested_name_specifier 'template' ? |  simple_template_id |  'enum' nested_name_specifier ? Identifier ;
enum_name :  Identifier ;
enum_specifier :  enum_head '{' enumerator_list ? '}' |  enum_head '{' enumerator_list ',' '}' ;
enum_head :  enum_key attribute_specifier_seq ? Identifier ? enum_base ? |  enum_key attribute_specifier_seq ? |  nested_name_specifier Identifier |  enum_base ? ;
opaque_enum_declaration :  enum_key attribute_specifier_seq ? |  Identifier enum_base ? |  ';' ;
enum_key :  'enum' |  'enum' 'class' |  'enum' 'struct' ;
enum_base :  ':' type_specifier_seq ;
enumerator_list : (  enumerator_definition ) ( ',' enumerator_definition ) * ;
enumerator_definition :  enumerator |  enumerator '=' constant_expression ;
// § A.6 	 1221  c ISO/IEC 	 N4296

enumerator :  Identifier attribute_specifier_seq ? ;
namespace_name :  Identifier |  namespace_alias ;
namespace_definition :  named_namespace_definition |  unnamed_namespace_definition nested_namespace_definition ;
named_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? Identifier '{' namespace_body '}' ;
unnamed_namespace_definition :  'inline' ? 'namespace' attribute_specifier_seq ? '{' namespace_body '}' ;
nested_namespace_definition :  'namespace' enclosing_namespace_specifier '::' Identifier '{' namespace_body '}' ;
enclosing_namespace_specifier :  Identifier enclosing_namespace_specifier '::' Identifier ;
namespace_body :  declaration_seq ? ;
namespace_alias :  Identifier ;
namespace_alias_definition :  'namespace' Identifier '=' qualified_namespace_specifier ';' ;
qualified_namespace_specifier :  nested_name_specifier ? namespace_name ;
using_declaration :  'using' 'typename' ? nested_name_specifier unqualified_id ';' ;
using_directive :  attribute_specifier_seq ? 'using' 'namespace' nested_name_specifier ? namespace_name ';' ;
asm_definition :  'asm' '(' String_literal ')' ';' ;
linkage_specification :  'extern' String_literal '{' declaration_seq ? '}' |  'extern' String_literal declaration ;
attribute_specifier_seq : ( attribute_specifier ) ( attribute_specifier ) * ;
attribute_specifier :  '[' '[' attribute_list ']' ']' |  alignment_specifier ;
alignment_specifier :  'alignas' '(' type_id '...' ? ')' |  'alignas' '(' constant_expression '...' ? ')' ;
attribute_list : (  attribute ? |  attribute '...' ) ( ',' attribute ? | ',' attribute '...' ) * ;
attribute :  attribute_token attribute_argument_clause ? ;
attribute_token :  Identifier |  attribute_scoped_token ;
// § A.6 	 1222  c ISO/IEC 	 N4296

attribute_scoped_token :  attribute_namespace '::' Identifier ;
attribute_namespace :  Identifier ;
attribute_argument_clause :  '(' balanced_token_seq ')' ;
balanced_token_seq : (  balanced_token ? ) ( balanced_token ) * ;
balanced_token :  '(' balanced_token_seq ')' |  '[' balanced_token_seq ']' |  '{' balanced_token_seq '}' |  ~( '(' | ')' | '{' | '}' | '[' | ']' )+ ;

// A.7 Declarators 	 [gram.decl] 
init_declarator_list : (  init_declarator ) ( ',' init_declarator ) * ;
init_declarator :  declarator initializer ? ;
declarator :  ptr_declarator |  noptr_declarator parameters_and_qualifiers trailing_return_type ;
ptr_declarator : (  ptr_operator ) * (  noptr_declarator ) ;
noptr_declarator : (  declarator_id attribute_specifier_seq ? |  '(' ptr_declarator ')' ) ( parameters_and_qualifiers | '[' constant_expression ? ']' attribute_specifier_seq ? ) * ;
parameters_and_qualifiers :  '(' parameter_declaration_clause ')' cv_qualifier_seq ? ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;
trailing_return_type :  '->' trailing_type_specifier_seq abstract_declarator ? ;
ptr_operator :  '*' attribute_specifier_seq ? cv_qualifier_seq ? |  '&' attribute_specifier_seq ? |  '&&' attribute_specifier_seq ? |  nested_name_specifier '*' attribute_specifier_seq ? cv_qualifier_seq ? ;
cv_qualifier_seq : (  cv_qualifier ) * ( ) ;
cv_qualifier :  'const' |  'volatile' ;
ref_qualifier :  '&' |  '&&' ;
declarator_id :  '...' ? id_expression ;
// § A.7 	 1223  c ISO/IEC 	 N4296

type_id :  type_specifier_seq abstract_declarator ? ;
abstract_declarator :  ptr_abstract_declarator |  noptr_abstract_declarator ? parameters_and_qualifiers trailing_return_type |  abstract_pack_declarator ;
ptr_abstract_declarator : (  ptr_operator ) * (  noptr_abstract_declarator ) ;
noptr_abstract_declarator : ( '(' ptr_abstract_declarator ')' | parameters_and_qualifiers | '[' constant_expression ? ']' attribute_specifier_seq ? ) ( parameters_and_qualifiers | '[' constant_expression ? ']' attribute_specifier_seq ? ) * ;
abstract_pack_declarator : (  ptr_operator ) * (  noptr_abstract_pack_declarator ) ;
noptr_abstract_pack_declarator : (  '...' ) ( parameters_and_qualifiers | '[' constant_expression ? ']' attribute_specifier_seq ? ) * ;
parameter_declaration_clause :  parameter_declaration_list ? '...' ? |  parameter_declaration_list ',' '...' ;
parameter_declaration_list : (  parameter_declaration ) ( ',' parameter_declaration ) * ;
parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator '=' initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? '=' initializer_clause ;
function_definition :  attribute_specifier_seq ? decl_specifier_seq ? declarator virt_specifier_seq ? function_body ;
function_body :  ctor_initializer ? compound_statement |  function_try_block |  '=' 'default' ';' |  '=' 'delete' ';' ;
initializer :  brace_or_equal_initializer |  '(' expression_list ')' ;
brace_or_equal_initializer :  '=' initializer_clause |  braced_init_list ;
initializer_clause :  assignment_expression |  braced_init_list ;
initializer_list : (  initializer_clause '...' ? ) ( ',' initializer_clause '...' ? ) * ;
braced_init_list :  '{' initializer_list ',' ? '}' |  '{' '}' ;
// § A.7 	 1224  c ISO/IEC 	 N4296


// A.8 Classes 	 [gram.class] 
class_name :  Identifier |  simple_template_id ;
class_specifier :  class_head '{' member_specification ? '}' ;
class_head :  class_key attribute_specifier_seq ? class_head_name class_virt_specifier ? base_clause ? |  class_key attribute_specifier_seq ? base_clause ? ;
class_head_name :  nested_name_specifier ? class_name ;
class_virt_specifier :  'final' ;
class_key :  'class' |  'struct' |  'union' ;
member_specification : (  member_declaration |  access_specifier ':' ) * ( ) ;
member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? ';' |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  alias_declaration |  empty_declaration ;
member_declarator_list : (  member_declarator ) ( ',' member_declarator ) * ;
member_declarator :  declarator virt_specifier_seq ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  Identifier ? attribute_specifier_seq ? ':' constant_expression ;
virt_specifier_seq : (  virt_specifier ) ( virt_specifier ) * ;
virt_specifier :  'override' |  'final' ;
pure_specifier :  '=' Octal_literal ;

// A.9 Derived classes 	 [gram.derived] 
base_clause :  ':' base_specifier_list ;
base_specifier_list : (  base_specifier '...' ? ) ( ',' base_specifier '...' ? ) * ;
// § A.9 	 1225  c ISO/IEC 	 N4296

base_specifier :  attribute_specifier_seq ? base_type_specifier |  attribute_specifier_seq ? 'virtual' access_specifier ? base_type_specifier |  attribute_specifier_seq ? access_specifier 'virtual' ? base_type_specifier ;
class_or_decltype :  nested_name_specifier ? class_name |  decltype_specifier ;
base_type_specifier :  class_or_decltype ;
access_specifier :  'private' |  'protected' |  'public' ;

// A.10 Special member functions 	 [gram.special] 
conversion_function_id :  'operator' conversion_type_id ;
conversion_type_id :  type_specifier_seq conversion_declarator ? ;
conversion_declarator : (  ptr_operator ) * ( ) ;
ctor_initializer :  ':' mem_initializer_list ;
mem_initializer_list : (  mem_initializer '...' ? ) ( ',' mem_initializer '...' ? ) * ;
mem_initializer :  mem_initializer_id '(' expression_list ? ')' |  mem_initializer_id braced_init_list ;
mem_initializer_id :  class_or_decltype |  Identifier ;

// A.11 Overloading 	 [gram.over] 
operator_function_id :  'operator' operator ;
operator :  'new' | 'delete' | 'new' '[' ']' | 'delete' '[' ']' | '+' | '-' | '=' | '*' | '<' | '/' | '>' | '%' | '+=' | '~' | '!' | '^' | '-=' | '&' | '*=' | '|' | '/=' | '%=' | '^=' | '&=' | '|=' | '<<' | '>>' | '>>=' | '<<=' | '==' | '!=' | '<=' | '(' ')' | '>=' | '[' ']' | '&&' | '||' | '++' | '--' | ',' | '->*' | '->' ;
literal_operator_id :  'operator' String_literal Identifier |  'operator' User_defined_string_literal ;

// A.12 Templates 	 [gram.temp] 
template_declaration :  'template' '<' template_parameter_list '>' declaration ;
template_parameter_list : (  template_parameter ) ( ',' template_parameter ) * ;
// § A.12 	 1226  c ISO/IEC 	 N4296

template_parameter :  type_parameter |  parameter_declaration ;
type_parameter :  type_parameter_key '...' ? Identifier ? |  type_parameter_key Identifier ? '=' type_id |  'template' '<' template_parameter_list '>' type_parameter_key '...' ? |  Identifier ? |  'template' '<' template_parameter_list '>' type_parameter_key Identifier ? '=' id_expression ;
type_parameter_key :  'class' |  'typename' ;
simple_template_id :  template_name '<' template_argument_list ? '>' ;
template_id :  simple_template_id |  operator_function_id '<' template_argument_list ? '>' |  literal_operator_id '<' template_argument_list ? '>' ;
template_name :  Identifier ;
template_argument_list : (  template_argument '...' ? ) ( ',' template_argument '...' ? ) * ;
template_argument :  constant_expression |  type_id |  id_expression ;
typename_specifier :  'typename' nested_name_specifier Identifier |  'typename' nested_name_specifier 'template' ? |  simple_template_id ;
explicit_instantiation :  'extern' ? 'template' declaration ;
explicit_specialization :  'template' '<' '>' declaration ;

// A.13 Exception handling 	 [gram.except] 
try_block :  'try' compound_statement handler_seq ;
function_try_block :  'try' ctor_initializer ? compound_statement handler_seq ;
handler_seq : (  handler ) * ( ) ;
handler :  'catch' '(' exception_declaration ')' compound_statement ;
exception_declaration :  attribute_specifier_seq ? type_specifier_seq declarator |  attribute_specifier_seq ? type_specifier_seq abstract_declarator ? |  '...' ;
exception_specification :  dynamic_exception_specification |  noexcept_specification ;
dynamic_exception_specification :  'throw' '(' type_id_list ? ')' ;
// § A.13 	 1227  c ISO/IEC 	 N4296

type_id_list : (  type_id '...' ? ) ( ',' type_id '...' ? ) * ;
noexcept_specification :  'noexcept' '(' constant_expression ')' |  'noexcept' ;

// A.14 Preprocessing directives 	 [gram.cpp] 
//  preprocessing_file :  group ? ;
//  group : (  group_part ) ( group_part ) * ;
//  group_part :  if_section |  control_line |  text_line |  '#' non_directive ;
//  if_section :  if_group elif_groups ? else_group ? endif_line ;
//  if_group :  '#' 'if' constant_expression new_line group ? |  '#' 'ifdef' Identifier new_line group ? |  '#' 'ifndef' Identifier new_line group ? ;
//  elif_groups : (  elif_group ) ( elif_group ) * ;
//  elif_group :  '#' 'elif' constant_expression new_line group ? ;
//  else_group :  '#' 'else' new_line group ? ;
//  endif_line :  '#' 'endif' new_line ;
//  control_line :  '#' 'include' pp_tokens new_line |  '#' 'define' Identifier replacement_list new_line |  '#' 'define' Identifier lparen identifier_list ? ')' replacement_list new_line |  '#' 'define' Identifier lparen '...' ')' replacement_list new_line |  '#' 'define' Identifier lparen identifier_list ',' '...' ')' replacement_list new_line |  '#' 'undef' Identifier new_line |  '#' 'line' pp_tokens new_line |  '#' 'error' pp_tokens ? new_line |  '#' 'pragma' pp_tokens ? new_line |  '#' new_line ;
//  text_line :  pp_tokens ? new_line ;
//  non_directive :  pp_tokens new_line ;
//  lparen :  RESTRICTED_CHARS10 ;
// § A.14 	 1228  c ISO/IEC 	 N4296

identifier_list : (  Identifier ) ( ',' Identifier ) * ;
//  replacement_list :  pp_tokens ? ;
//  pp_tokens : (  preprocessing_token ) ( preprocessing_token ) * ;
//  new_line :  RESTRICTED_CHARS11 ;
// § A.14 	 1229  c ISO/IEC 	 N4296

keyword : 'alignas' | 'continue' | 'friend' | 'register' | 'true' 
                'alignof' | 'decltype' | 'goto' | 'reinterpret_cast' | 'try'
                'asm' | 'default' | 'if' | 'return' | 'typedef'
                'auto' | 'delete' | 'inline' | 'short' | 'typeid'
                'bool' | 'do' | 'int' | 'signed' | 'typename'
                'break' | 'double' | 'long' | 'sizeof' | 'union'
                'case' | 'dynamic_cast' | 'mutable' | 'static' | 'unsigned'
                'catch' | 'else' | 'namespace' | 'static_assert' | 'using'
                'char' | 'enum' | 'new' | 'static_cast' | 'virtual'
                'char16_t' | 'explicit' | 'noexcept' | 'struct' | 'void'
                'char32_t' | 'export' | 'nullptr' | 'switch' | 'volatile'
                'class' | 'extern' | 'operator' | 'template' | 'wchar_t'
                'const' | 'false' | 'private' | 'this' | 'while'
                'constexpr' | 'float' | 'protected' | 'thread_local'
                'const_cast' | 'for' | 'public' | 'throw'
                'and' | 'and_eq' | 'bitand' | 'bitor' | 'compl' | 'not'
                'not_eq' | 'or' | 'or_eq' | 'xor' | 'xor_eq' ;
punctuator : preprocessing_op_or_punc ;
WS : [\n\r\t ]+ -> channel(HIDDEN);
COMMENT : '//' ~[\n\r]* -> channel(HIDDEN);
ML_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
Prep : '#' ~[\n\r]* -> channel(HIDDEN);
