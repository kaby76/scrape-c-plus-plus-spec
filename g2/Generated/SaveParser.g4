parser grammar SaveParser;

options { tokenVocab=SaveLexer; }

// A.1 Keywords 	 [gram.key] 
// typedef_name :  identifier ;
// namespace_name :  original_namespace_name |  namespace_alias ;
original_namespace_name :  Identifier ;
preprocessing_token :  Header_name |  Identifier |  pp_number |  Character_literal |  User_defined_character_literal |  String_literal |  User_defined_string_literal |  preprocessing_op_or_punc | ~Newline;
// § A.2 	 1210  c ISO/IEC 	 N4296

token :  Identifier |  keyword |  literal |  operator |  punctuator ;
// § A.2 	 1211  c ISO/IEC

preprocessing_op_or_punc :  LeftBrace | RightBrace | LeftBracket | RightBracket
 | Pound | PoundPound
 | LeftParen | RightParen | LtColon | ColonGt | LtPer | PerGt
 | PerColon | PerColonPerColon | Semi | Colon | Ellipsis
 | KWNew | KWDelete | Question | Doublecolon | Dot | DotStar
 | Plus | Minus | Assign | Star | Less | Div | Greater
 | Mod | PlusAssign | Tilde | Not | Caret | MinusAssign
 | And | StarAssign | Or | DivAssign | ModAssign | XorAssign
 | AndAssign | OrAssign | LeftShift | RightShift
 | RightShiftAssign | LeftShiftAssign | Equal | NotEqual
 | LessEqual | KWAnd | KWOr | GreaterEqual | KWAndEq | KWOrEq
 | AndAnd | KWBitAnd | KWXor | OrOr | KWBitOr | KWXorEq
 | PlusPlus | KWCompl | MinusMinus | KWNot | Comma
 | KWNotEq | ArrowStar | Arrow ;

literal :  Integer_literal |  Character_literal |  Floating_literal |  String_literal |  boolean_literal |  pointer_literal |  User_defined_literal ;
boolean_literal :  KWFalse_ |  KWTrue_ ;
pointer_literal :  KWNullptr ;
// § A.2 	 1214  c ISO/IEC 	 N4296


// A.3 Basic concepts 	 [gram.basic] 
translation_unit :  declaration_seq ? EOF ;

// A.4 Expressions 	 [gram.expr] 
primary_expression :  literal |  KWThis |  LeftParen expression RightParen |  id_expression |  lambda_expression |  fold_expression ;
id_expression :  unqualified_id |  qualified_id ;
unqualified_id :  Identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  Minus class_name |  Minus decltype_specifier |  template_id ;
qualified_id :  nested_name_specifier KWTemplate ? unqualified_id ;
nested_name_specifier :  Doublecolon |  type_name Doublecolon |  namespace_name Doublecolon |  decltype_specifier Doublecolon |  nested_name_specifier Identifier Doublecolon |  nested_name_specifier KWTemplate ? simple_template_id Doublecolon ;
lambda_expression :  lambda_introducer lambda_declarator ? compound_statement ;
lambda_introducer :  LeftBracket lambda_capture ? RightBracket ;
lambda_capture :  capture_default |  capture_list |  capture_default Comma capture_list ;
capture_default :  And |  Assign ;
capture_list :  capture Ellipsis ? |  capture_list Comma capture Ellipsis ? ;
capture :  simple_capture |  init_capture ;
simple_capture :  Identifier |  And Identifier |  KWThis ;
// § A.4 	 1215  c ISO/IEC 	 N4296

init_capture :  Identifier initializer |  And Identifier initializer ;
lambda_declarator :  LeftParen parameter_declaration_clause RightParen KWMutable ? exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;
fold_expression :  LeftParen cast_expression fold_operator Ellipsis RightParen |  LeftParen Ellipsis fold_operator cast_expression RightParen |  LeftParen cast_expression fold_operator Ellipsis fold_operator cast_expression RightParen ;
fold_operator :  Plus | Minus | Star | Div | Mod | Caret | And | Or | LeftShift | RightShift | PlusAssign | MinusAssign | StarAssign | DivAssign | ModAssign | XorAssign | AndAssign | OrAssign | LeftShiftAssign | RightShiftAssign | Assign | Equal | NotEqual | Less | Greater | LessEqual | GreaterEqual | AndAnd | OrOr | Comma | DotStar | ArrowStar ;
postfix_expression :  primary_expression |  postfix_expression LeftBracket expression RightBracket |  postfix_expression LeftBracket braced_init_list RightBracket |  postfix_expression LeftParen expression_list ? RightParen |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  postfix_expression Dot KWTemplate ?  id_expression |  postfix_expression Arrow KWTemplate ? id_expression |  postfix_expression Dot pseudo_destructor_name |  postfix_expression Arrow pseudo_destructor_name |  postfix_expression PlusPlus |  postfix_expression MinusMinus |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ;
expression_list :  initializer_list ;
pseudo_destructor_name :  nested_name_specifier ? type_name Doublecolon Minus type_name |  nested_name_specifier KWTemplate simple_template_id Doublecolon Minus type_name |  Minus type_name |  Minus decltype_specifier ;
unary_expression :  postfix_expression |  PlusPlus cast_expression |  MinusMinus cast_expression |  unary_operator cast_expression |  KWSizeof unary_expression |  KWSizeof LeftParen type_id RightParen |  KWSizeof Ellipsis LeftParen Identifier RightParen |  KWAlignof LeftParen type_id RightParen |  noexcept_expression |  new_expression |  delete_expression ;
// § A.4 	 1216  c ISO/IEC 	 N4296

unary_operator :  Star | And | Plus | Minus | Not | Tilde | KWNot | KWCompl ;
new_expression :  Doublecolon ? KWNew new_placement ? new_type_id new_initializer ? |  Doublecolon ? KWNew new_placement ? LeftParen type_id RightParen new_initializer ? ;
new_placement :  LeftParen expression_list RightParen ;
new_type_id :  type_specifier_seq new_declarator ? ;
new_declarator :  ptr_operator new_declarator ? |  noptr_new_declarator ;
noptr_new_declarator :  LeftBracket expression RightBracket attribute_specifier_seq ? |  noptr_new_declarator LeftBracket constant_expression RightBracket attribute_specifier_seq ? ;
new_initializer :  LeftParen expression_list ? RightParen |  braced_init_list ;
delete_expression :  Doublecolon ? KWDelete cast_expression |  Doublecolon ? KWDelete LeftBracket RightBracket cast_expression ;
noexcept_expression :  KWNoexcept LeftParen expression RightParen ;
cast_expression :  unary_expression |  LeftParen type_id RightParen cast_expression ;
pm_expression :  cast_expression |  pm_expression DotStar cast_expression |  pm_expression ArrowStar cast_expression ;
multiplicative_expression :  pm_expression |  multiplicative_expression Star pm_expression |  multiplicative_expression Div pm_expression |  multiplicative_expression Mod pm_expression ;
additive_expression :  multiplicative_expression |  additive_expression Plus multiplicative_expression |  additive_expression Minus multiplicative_expression ;
shift_expression :  additive_expression |  shift_expression LeftShift additive_expression |  shift_expression RightShift additive_expression ;
relational_expression :  shift_expression |  relational_expression Less shift_expression |  relational_expression Greater shift_expression |  relational_expression LessEqual shift_expression |  relational_expression GreaterEqual shift_expression ;
equality_expression :  relational_expression |  equality_expression Equal relational_expression |  equality_expression NotEqual relational_expression ;
// § A.4 	 1217  c ISO/IEC 	 N4296

and_expression :  equality_expression |  and_expression ( And | KWBitAnd ) equality_expression ;
exclusive_or_expression :  and_expression |  exclusive_or_expression ( Caret | KWXor ) and_expression ;
inclusive_or_expression :  exclusive_or_expression |  inclusive_or_expression ( Or | KWBitOr ) exclusive_or_expression ;
logical_and_expression :  inclusive_or_expression |  logical_and_expression ( AndAnd | KWAnd ) inclusive_or_expression ;
logical_or_expression :  logical_and_expression |  logical_or_expression ( OrOr | KWOr ) logical_and_expression ;
conditional_expression :  logical_or_expression |  logical_or_expression Question expression Colon assignment_expression ;
throw_expression :  KWThrow assignment_expression ? ;
assignment_expression :  conditional_expression |  logical_or_expression assignment_operator initializer_clause |  throw_expression ;
assignment_operator :  Assign | StarAssign | DivAssign | ModAssign | PlusAssign | MinusAssign | RightShiftAssign | LeftShiftAssign | AndAssign | XorAssign | OrAssign | KWAndEq | KWOrEq | KWXorEq ;
expression :  assignment_expression |  expression Comma assignment_expression ;
constant_expression :  conditional_expression ;

// A.5 Statements 	 [gram.stmt] 
statement :  labeled_statement |  attribute_specifier_seq ? expression_statement |  attribute_specifier_seq ? compound_statement |  attribute_specifier_seq ? selection_statement |  attribute_specifier_seq ? iteration_statement |  attribute_specifier_seq ? jump_statement |  declaration_statement |  attribute_specifier_seq ? try_block ;
labeled_statement :  attribute_specifier_seq ? Identifier Colon statement |  attribute_specifier_seq ? KWCase constant_expression Colon statement |  attribute_specifier_seq ? KWDefault Colon statement ;
expression_statement :  expression ? Semi ;
compound_statement :  LeftBrace statement_seq ? RightBrace ;
statement_seq :  statement |  statement_seq statement ;
// § A.5 	 1218  c ISO/IEC 	 N4296

selection_statement :  KWIf LeftParen condition RightParen statement |  KWIf LeftParen condition RightParen statement KWElse statement |  KWSwitch LeftParen condition RightParen statement ;
condition :  expression |  attribute_specifier_seq ? decl_specifier_seq declarator Assign initializer_clause |  attribute_specifier_seq ? decl_specifier_seq declarator braced_init_list ;
iteration_statement :  KWWhile LeftParen condition RightParen statement |  KWDo statement KWWhile LeftParen expression RightParen Semi |  KWFor LeftParen for_init_statement condition ? Semi expression ? RightParen statement |  KWFor LeftParen for_range_declaration Colon for_range_initializer RightParen statement ;
for_init_statement :  expression_statement |  simple_declaration ;
for_range_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator ;
for_range_initializer :  expression |  braced_init_list ;
jump_statement :  KWBreak Semi |  KWContinue Semi |  KWReturn expression ? Semi |  KWReturn braced_init_list Semi |  KWGoto Identifier Semi ;
declaration_statement :  block_declaration ;

// A.6 Declarations 	 [gram.dcl] 
declaration_seq :  declaration |  declaration_seq declaration ;
declaration :  block_declaration |  function_definition |  template_declaration |  explicit_instantiation |  explicit_specialization |  linkage_specification |  namespace_definition |  empty_declaration |  attribute_declaration ;
block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;
// § A.6 	 1219  c ISO/IEC 	 N4296

alias_declaration :  KWUsing Identifier attribute_specifier_seq ? Assign type_id Semi ;
simple_declaration :  decl_specifier_seq ? init_declarator_list ? Semi |  attribute_specifier_seq decl_specifier_seq ? init_declarator_list Semi ;
static_assert_declaration :  KWStatic_assert LeftParen constant_expression RightParen Semi |  KWStatic_assert LeftParen constant_expression Comma String_literal RightParen Semi ;
empty_declaration :  Semi ;
attribute_declaration :  attribute_specifier_seq Semi ;
decl_specifier :  storage_class_specifier |  type_specifier |  function_specifier |  KWFriend |  KWTypedef |  KWConstexpr ;
decl_specifier_seq :  decl_specifier attribute_specifier_seq ? |  decl_specifier decl_specifier_seq ;
storage_class_specifier :  KWRegister |  KWStatic |  KWThread_local |  KWExtern |  KWMutable ;
function_specifier :  KWInline |  KWVirtual |  KWExplicit ;
typedef_name :  Identifier ;
type_specifier :  trailing_type_specifier |  class_specifier |  enum_specifier ;
trailing_type_specifier :  simple_type_specifier |  elaborated_type_specifier |  typename_specifier |  cv_qualifier ;
type_specifier_seq :  type_specifier attribute_specifier_seq ? |  type_specifier type_specifier_seq ;
trailing_type_specifier_seq :  trailing_type_specifier attribute_specifier_seq ? |  trailing_type_specifier trailing_type_specifier_seq ;
// § A.6 	 1220  c ISO/IEC 	 N4296

simple_type_specifier :  nested_name_specifier ? type_name |  nested_name_specifier KWTemplate simple_template_id |  KWChar |  KWChar16 |  KWChar32 |  KWWchar |  KWBool |  KWShort |  KWInt |  KWLong |  KWSigned |  KWUnsigned |  KWFloat |  KWDouble |  KWVoid |  KWAuto |  decltype_specifier ;
type_name :  class_name |  enum_name |  typedef_name |  simple_template_id ;
decltype_specifier :  KWDecltype LeftParen expression RightParen |  KWDecltype LeftParen KWAuto RightParen ;
elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? Identifier |  class_key simple_template_id |  class_key nested_name_specifier KWTemplate ? |  simple_template_id |  KWEnum nested_name_specifier ? Identifier ;
enum_name :  Identifier ;
enum_specifier :  enum_head LeftBrace enumerator_list ? RightBrace |  enum_head LeftBrace enumerator_list Comma RightBrace ;
enum_head :  enum_key attribute_specifier_seq ? Identifier ? enum_base ? |  enum_key attribute_specifier_seq ? |  nested_name_specifier Identifier |  enum_base ? ;
opaque_enum_declaration :  enum_key attribute_specifier_seq ? |  Identifier enum_base ? |  Semi ;
enum_key :  KWEnum |  KWEnum KWClass |  KWEnum KWStruct ;
enum_base :  Colon type_specifier_seq ;
enumerator_list :  enumerator_definition |  enumerator_list Comma enumerator_definition ;
enumerator_definition :  enumerator |  enumerator Assign constant_expression ;
// § A.6 	 1221  c ISO/IEC 	 N4296

enumerator :  Identifier attribute_specifier_seq ? ;
namespace_name :  Identifier |  namespace_alias ;
namespace_definition :  named_namespace_definition |  unnamed_namespace_definition nested_namespace_definition ;
named_namespace_definition :  KWInline ? KWNamespace attribute_specifier_seq ? Identifier LeftBrace namespace_body RightBrace ;
unnamed_namespace_definition :  KWInline ? KWNamespace attribute_specifier_seq ? LeftBrace namespace_body RightBrace ;
nested_namespace_definition :  KWNamespace enclosing_namespace_specifier Doublecolon Identifier LeftBrace namespace_body RightBrace ;
enclosing_namespace_specifier :  Identifier enclosing_namespace_specifier Doublecolon Identifier ;
namespace_body :  declaration_seq ? ;
namespace_alias :  Identifier ;
namespace_alias_definition :  KWNamespace Identifier Assign qualified_namespace_specifier Semi ;
qualified_namespace_specifier :  nested_name_specifier ? namespace_name ;
using_declaration :  KWUsing KWTypename_ ? nested_name_specifier unqualified_id Semi ;
using_directive :  attribute_specifier_seq ? KWUsing KWNamespace nested_name_specifier ? namespace_name Semi ;
asm_definition :  KWAsm LeftParen String_literal RightParen Semi ;
linkage_specification :  KWExtern String_literal LeftBrace declaration_seq ? RightBrace |  KWExtern String_literal declaration ;
attribute_specifier_seq :  attribute_specifier_seq attribute_specifier | attribute_specifier ;
attribute_specifier :  LeftBracket LeftBracket attribute_list RightBracket RightBracket |  alignment_specifier ;
alignment_specifier :  KWAlignas LeftParen type_id Ellipsis ? RightParen |  KWAlignas LeftParen constant_expression Ellipsis ? RightParen ;
attribute_list :  attribute ? |  attribute_list Comma attribute ? |  attribute Ellipsis |  attribute_list Comma attribute Ellipsis ;
attribute :  attribute_token attribute_argument_clause ? ;
attribute_token :  Identifier |  attribute_scoped_token ;
// § A.6 	 1222  c ISO/IEC 	 N4296

attribute_scoped_token :  attribute_namespace Doublecolon Identifier ;
attribute_namespace :  Identifier ;
attribute_argument_clause :  LeftParen balanced_token_seq RightParen ;
balanced_token_seq :  balanced_token ? |  balanced_token_seq balanced_token ;
balanced_token :  LeftParen balanced_token_seq RightParen |  LeftBracket balanced_token_seq RightBracket |  LeftBrace balanced_token_seq RightBrace |  ~( LeftParen | RightParen | LeftBrace | RightBrace | LeftBracket | RightBracket )+ ;

// A.7 Declarators 	 [gram.decl] 
init_declarator_list :  init_declarator |  init_declarator_list Comma init_declarator ;
init_declarator :  declarator initializer ? ;
declarator :  ptr_declarator |  noptr_declarator parameters_and_qualifiers trailing_return_type ;
ptr_declarator :  noptr_declarator |  ptr_operator ptr_declarator ;
noptr_declarator :  declarator_id attribute_specifier_seq ? |  noptr_declarator parameters_and_qualifiers |  noptr_declarator LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? |  LeftParen ptr_declarator RightParen ;
parameters_and_qualifiers :  LeftParen parameter_declaration_clause RightParen cv_qualifier_seq ? ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;
trailing_return_type :  Arrow trailing_type_specifier_seq abstract_declarator ? ;
ptr_operator :  Star attribute_specifier_seq ? cv_qualifier_seq ? |  And attribute_specifier_seq ? |  AndAnd attribute_specifier_seq ? |  nested_name_specifier Star attribute_specifier_seq ? cv_qualifier_seq ? ;
cv_qualifier_seq :  cv_qualifier cv_qualifier_seq ? ;
cv_qualifier :  KWConst |  KWVolatile ;
ref_qualifier :  And |  AndAnd ;
declarator_id :  Ellipsis ? id_expression ;
// § A.7 	 1223  c ISO/IEC 	 N4296

type_id :  type_specifier_seq abstract_declarator ? ;
abstract_declarator :  ptr_abstract_declarator |  noptr_abstract_declarator ? parameters_and_qualifiers trailing_return_type |  abstract_pack_declarator ;
ptr_abstract_declarator :  noptr_abstract_declarator |  ptr_operator ptr_abstract_declarator ? ;
noptr_abstract_declarator : noptr_abstract_declarator parameters_and_qualifiers | noptr_abstract_declarator LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? | LeftParen ptr_abstract_declarator RightParen | parameters_and_qualifiers | LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? ;
abstract_pack_declarator :  noptr_abstract_pack_declarator |  ptr_operator abstract_pack_declarator ;
noptr_abstract_pack_declarator :  noptr_abstract_pack_declarator parameters_and_qualifiers |  noptr_abstract_pack_declarator LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? |  Ellipsis ;
parameter_declaration_clause :  parameter_declaration_list ? Ellipsis ? |  parameter_declaration_list Comma Ellipsis ;
parameter_declaration_list :  parameter_declaration |  parameter_declaration_list Comma parameter_declaration ;
parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator Assign initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? Assign initializer_clause ;
function_definition :  attribute_specifier_seq ? decl_specifier_seq ? declarator virt_specifier_seq ? function_body ;
function_body :  ctor_initializer ? compound_statement |  function_try_block |  Assign KWDefault Semi |  Assign KWDelete Semi ;
initializer :  brace_or_equal_initializer |  LeftParen expression_list RightParen ;
brace_or_equal_initializer :  Assign initializer_clause |  braced_init_list ;
initializer_clause :  assignment_expression |  braced_init_list ;
initializer_list :  initializer_clause Ellipsis ? |  initializer_list Comma initializer_clause Ellipsis ? ;
braced_init_list :  LeftBrace initializer_list Comma ? RightBrace |  LeftBrace RightBrace ;
// § A.7 	 1224  c ISO/IEC 	 N4296


// A.8 Classes 	 [gram.class] 
class_name :  Identifier |  simple_template_id ;
class_specifier :  class_head LeftBrace member_specification ? RightBrace ;
class_head :  class_key attribute_specifier_seq ? class_head_name class_virt_specifier ? base_clause ? |  class_key attribute_specifier_seq ? base_clause ? ;
class_head_name :  nested_name_specifier ? class_name ;
class_virt_specifier :  KWFinal ;
class_key :  KWClass |  KWStruct |  KWUnion ;
member_specification :  member_declaration member_specification ? |  access_specifier Colon member_specification ? ;
member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? Semi |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  alias_declaration |  empty_declaration ;
member_declarator_list :  member_declarator |  member_declarator_list Comma member_declarator ;
member_declarator :  declarator virt_specifier_seq ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  Identifier ? attribute_specifier_seq ? Colon constant_expression ;
virt_specifier_seq :  virt_specifier |  virt_specifier_seq virt_specifier ;
virt_specifier :  KWOverride |  KWFinal ;
pure_specifier :  Assign Octal_literal ;

// A.9 Derived classes 	 [gram.derived] 
base_clause :  Colon base_specifier_list ;
base_specifier_list :  base_specifier Ellipsis ? |  base_specifier_list Comma base_specifier Ellipsis ? ;
// § A.9 	 1225  c ISO/IEC 	 N4296

base_specifier :  attribute_specifier_seq ? base_type_specifier |  attribute_specifier_seq ? KWVirtual access_specifier ? base_type_specifier |  attribute_specifier_seq ? access_specifier KWVirtual ? base_type_specifier ;
class_or_decltype :  nested_name_specifier ? class_name |  decltype_specifier ;
base_type_specifier :  class_or_decltype ;
access_specifier :  KWPrivate |  KWProtected |  KWPublic ;

// A.10 Special member functions 	 [gram.special] 
conversion_function_id :  KWOperator conversion_type_id ;
conversion_type_id :  type_specifier_seq conversion_declarator ? ;
conversion_declarator :  ptr_operator conversion_declarator ? ;
ctor_initializer :  Colon mem_initializer_list ;
mem_initializer_list :  mem_initializer Ellipsis ? |  mem_initializer_list Comma mem_initializer Ellipsis ? ;
mem_initializer :  mem_initializer_id LeftParen expression_list ? RightParen |  mem_initializer_id braced_init_list ;
mem_initializer_id :  class_or_decltype |  Identifier ;

// A.11 Overloading 	 [gram.over] 
operator_function_id :  KWOperator operator ;
operator :  KWNew | KWDelete | KWNew LeftBracket RightBracket | KWDelete LeftBracket RightBracket | Plus | Minus | Assign | Star | Less | Div | Greater | Mod | PlusAssign | Tilde | Not | Caret | MinusAssign | And | StarAssign | Or | DivAssign | ModAssign | XorAssign | AndAssign | OrAssign | LeftShift | RightShift | RightShiftAssign | LeftShiftAssign | Equal | NotEqual | LessEqual | LeftParen RightParen | GreaterEqual | LeftBracket RightBracket | AndAnd | OrOr | PlusPlus | MinusMinus | Comma | ArrowStar | Arrow ;
literal_operator_id :  KWOperator String_literal Identifier |  KWOperator User_defined_string_literal ;

// A.12 Templates 	 [gram.temp] 
template_declaration :  KWTemplate Less template_parameter_list Greater declaration ;
template_parameter_list :  template_parameter |  template_parameter_list Comma template_parameter ;
// § A.12 	 1226  c ISO/IEC 	 N4296

template_parameter :  type_parameter |  parameter_declaration ;
type_parameter :  type_parameter_key Ellipsis ? Identifier ? |  type_parameter_key Identifier ? Assign type_id |  KWTemplate Less template_parameter_list Greater type_parameter_key Ellipsis ? |  Identifier ? |  KWTemplate Less template_parameter_list Greater type_parameter_key Identifier ? Assign id_expression ;
type_parameter_key :  KWClass |  KWTypename_ ;
simple_template_id :  template_name Less template_argument_list ? Greater ;
template_id :  simple_template_id |  operator_function_id Less template_argument_list ? Greater |  literal_operator_id Less template_argument_list ? Greater ;
template_name :  Identifier ;
template_argument_list :  template_argument Ellipsis ? |  template_argument_list Comma template_argument Ellipsis ? ;
template_argument :  constant_expression |  type_id |  id_expression ;
typename_specifier :  KWTypename_ nested_name_specifier Identifier |  KWTypename_ nested_name_specifier KWTemplate ? |  simple_template_id ;
explicit_instantiation :  KWExtern ? KWTemplate declaration ;
explicit_specialization :  KWTemplate Less Greater declaration ;

// A.13 Exception handling 	 [gram.except] 
try_block :  KWTry compound_statement handler_seq ;
function_try_block :  KWTry ctor_initializer ? compound_statement handler_seq ;
handler_seq :  handler handler_seq ? ;
handler :  KWCatch LeftParen exception_declaration RightParen compound_statement ;
exception_declaration :  attribute_specifier_seq ? type_specifier_seq declarator |  attribute_specifier_seq ? type_specifier_seq abstract_declarator ? |  Ellipsis ;
exception_specification :  dynamic_exception_specification |  noexcept_specification ;
dynamic_exception_specification :  KWThrow LeftParen type_id_list ? RightParen ;
// § A.13 	 1227  c ISO/IEC 	 N4296

type_id_list :  type_id Ellipsis ? |  type_id_list Comma type_id Ellipsis ? ;
noexcept_specification :  KWNoexcept LeftParen constant_expression RightParen |  KWNoexcept ;

// A.14 Preprocessing directives 	 [gram.cpp] 
preprocessing_file :  group ? EOF ;
group :  group_part+? ;
group_part :  if_section |  control_line |  text_line |  PPPound non_directive ;
if_section :  if_group elif_groups ? else_group ? endif_line ;
if_group :  Pound KWIf constant_expression new_line group ? |  Pound KWIfdef Identifier new_line group ? |  Pound KWIfndef Identifier new_line group ? ;
elif_groups :  elif_group |  elif_groups elif_group ;
elif_group :  Pound KWElif constant_expression new_line group ? ;
else_group :  Pound KWElse new_line group ? ;
endif_line :  Pound KWEndif new_line ;
control_line :  Pound KWInclude pp_tokens new_line |  Pound KWDefine Identifier replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list ? RightParen replacement_list new_line |  Pound KWDefine Identifier lparen Ellipsis RightParen replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list Comma Ellipsis RightParen replacement_list new_line |  Pound KWUndef Identifier new_line |  Pound KWLine pp_tokens new_line |  Pound KWError pp_tokens ? new_line |  Pound KWPragma pp_tokens ? new_line |  Pound new_line ;
text_line :  { InputStream.LA(1) != SaveLexer.Pound }? pp_tokens ? new_line ;
non_directive :  pp_tokens new_line ;
lparen :  LeftParen;
// § A.14 	 1228  c ISO/IEC 	 N4296

identifier_list :  Identifier |  identifier_list Comma Identifier ;
replacement_list :  pp_tokens ? ;
pp_tokens :  preprocessing_token+?;
new_line :  Newline;
pp_number : Floating_literal;

// § A.14 	 1229  c ISO/IEC 	 N4296

keyword : KWAlignas | KWContinue | KWFriend | KWRegister | KWTrue_ 
                KWAlignof | KWDecltype | KWGoto | KWReinterpret_cast | KWTry
                KWAsm | KWDefault | KWIf | KWReturn | KWTypedef
                KWAuto | KWDelete | KWInline | KWShort | KWTypeid_
                KWBool | KWDo | KWInt | KWSigned | KWTypename_
                KWBreak | KWDouble | KWLong | KWSizeof | KWUnion
                KWCase | KWDynamic_cast | KWMutable | KWStatic | KWUnsigned
                KWCatch | KWElse | KWNamespace | KWStatic_assert | KWUsing
                KWChar | KWEnum | KWNew | KWStatic_cast | KWVirtual
                KWChar16 | KWExplicit | KWNoexcept | KWStruct | KWVoid
                KWChar32 | KWExport | KWNullptr | KWSwitch | KWVolatile
                KWClass | KWExtern | KWOperator | KWTemplate | KWWchar
                KWConst | KWFalse_ | KWPrivate | KWThis | KWWhile
                KWConstexpr | KWFloat | KWProtected | KWThread_local
                KWConst_cast | KWFor | KWPublic | KWThrow
                KWAnd | KWAndEq | KWBitAnd | KWBitOr | KWCompl | KWNot
                KWNotEq | KWOr | KWOrEq | KWXor | KWXorEq ;
punctuator : preprocessing_op_or_punc ;

