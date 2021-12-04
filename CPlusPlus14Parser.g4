
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

parser grammar CPlusPlus14Parser;

options { tokenVocab=CPlusPlus14Lexer; superClass=ParserBase; }
// typedef_name :  Identifier ;
// namespace_name :  original_namespace_name |  namespace_alias ;
// original_namespace_name :  Identifier ;
// namespace_alias :  Identifier ;
// class_name :  Identifier |  simple_template_id ;
// enum_name :  Identifier ;
// template_name :  Identifier ;
preprocessing_token :  FHeader_name |  Identifier |  pp_number |  Character_literal |  User_defined_character_literal |  String_literal |  User_defined_string_literal |  preprocessing_op_or_punc |  ~Newline ;
token :  Identifier |  keyword |  literal |  operator |  punctuator ;
pp_number : Floating_literal;
preprocessing_op_or_punc :  LeftBrace | RightBrace | LeftBracket | RightBracket | Pound | PoundPound | LeftParen | RightParen | LtColon | ColonGt | LtPer | PerGt | PerColon | PerColonPerColon | Semi | Colon | Ellipsis | KWNew | KWDelete | Question | Doublecolon | Dot | DotStar | Plus | Minus | Star | Div | Mod | Caret | And | Or | Tilde | Not | Assign | Less | Greater | PlusAssign | MinusAssign | StarAssign | DivAssign | ModAssign | XorAssign | AndAssign | OrAssign | LeftShift | Greater Greater | RightShiftAssign | LeftShiftAssign | Equal | NotEqual | LessEqual | GreaterEqual | AndAnd | OrOr | PlusPlus | MinusMinus | Comma | ArrowStar | Arrow | KWAnd | KWAndEq | KWBitAnd | KWBitOr | KWCompl | KWNot | KWNotEq | KWOr | KWOrEq | KWXor | KWXorEq ;
literal :  Integer_literal |  Character_literal |  Floating_literal |  String_literal |  boolean_literal |  pointer_literal |  User_defined_literal ;
boolean_literal :  KWFalse_ |  KWTrue_ ;
pointer_literal :  KWNullptr ;
translation_unit :  declaration_seq ? EOF ;
primary_expression :  literal |  KWThis |  LeftParen expression RightParen |  id_expression |  lambda_expression ;
id_expression :  unqualified_id |  qualified_id ;
unqualified_id :  Identifier |  operator_function_id |  conversion_function_id |  literal_operator_id |  Tilde |  class_name |  Tilde |  decltype_specifier |  template_id ;
qualified_id :  nested_name_specifier KWTemplate ? unqualified_id ;
nested_name_specifier : (  Doublecolon |  type_name Doublecolon |  namespace_name Doublecolon |  decltype_specifier Doublecolon ) ( Identifier Doublecolon | KWTemplate ? simple_template_id Doublecolon )* ;
lambda_expression :  lambda_introducer lambda_declarator ? compound_statement ;
lambda_introducer :  LeftBracket lambda_capture ? RightBracket ;
lambda_capture :  capture_default |  capture_list |  capture_default Comma capture_list ;
capture_default :  And |  Assign ;
capture_list : (  capture Ellipsis ? ) ( Comma capture Ellipsis ? )* ;
capture :  simple_capture |  init_capture ;
simple_capture :  Identifier |  And Identifier |  KWThis ;
init_capture :  Identifier initializer |  And Identifier initializer ;
lambda_declarator :  LeftParen parameter_declaration_clause RightParen KWMutable ? exception_specification ? attribute_specifier_seq ? trailing_return_type ? ;
postfix_expression : (  primary_expression |  simple_type_specifier LeftParen expression_list ? RightParen |  typename_specifier LeftParen expression_list ? RightParen |  simple_type_specifier braced_init_list |  typename_specifier braced_init_list |  KWDynamic_cast Less type_id Greater LeftParen expression RightParen |  KWStatic_cast Less type_id Greater LeftParen expression RightParen |  KWReinterpret_cast Less type_id Greater LeftParen expression RightParen |  KWConst_cast Less type_id Greater LeftParen expression RightParen |  KWTypeid_ LeftParen expression RightParen |  KWTypeid_ LeftParen type_id RightParen ) ( LeftBracket expression RightBracket | LeftBracket braced_init_list RightBracket | LeftParen expression_list ? RightParen | Dot KWTemplate ? id_expression | Arrow KWTemplate ? id_expression | Dot pseudo_destructor_name | Arrow pseudo_destructor_name | PlusPlus | MinusMinus )* ;
expression_list :  initializer_list ;
pseudo_destructor_name :  nested_name_specifier ? type_name Doublecolon Tilde  type_name |  nested_name_specifier KWTemplate simple_template_id Doublecolon Tilde  type_name |  nested_name_specifier ? Tilde  type_name |  Tilde |  decltype_specifier ;
unary_expression :  KWSizeof* (  postfix_expression |  PlusPlus cast_expression |  MinusMinus cast_expression |  unary_operator cast_expression |  KWSizeof LeftParen type_id RightParen |  KWSizeof Ellipsis LeftParen Identifier RightParen |  KWAlignof LeftParen type_id RightParen |  noexcept_expression |  new_expression |  delete_expression ) ;
unary_operator :  Star | And | Plus | Minus | Not | Tilde | KWNot | KWCompl ;
new_expression :  Doublecolon ? KWNew new_placement ? new_type_id new_initializer ? |  Doublecolon ? KWNew new_placement ? LeftParen type_id RightParen new_initializer ? ;
new_placement :  LeftParen expression_list RightParen ;
new_type_id :  type_specifier_seq new_declarator ? ;
new_declarator :  ptr_operator+  noptr_new_declarator ;
noptr_new_declarator : (  LeftBracket expression RightBracket attribute_specifier_seq ? ) ( LeftBracket constant_expression RightBracket attribute_specifier_seq ? )* ;
new_initializer :  LeftParen expression_list ? RightParen |  braced_init_list ;
delete_expression :  Doublecolon ? KWDelete cast_expression |  Doublecolon ? KWDelete LeftBracket RightBracket cast_expression ;
noexcept_expression :  KWNoexcept LeftParen expression RightParen ;
cast_expression : (  LeftParen type_id RightParen )*  unary_expression ;
pm_expression :  cast_expression ( DotStar cast_expression | ArrowStar cast_expression )* ;
multiplicative_expression :  pm_expression ( Star pm_expression | Div pm_expression | Mod pm_expression )* ;
additive_expression :  multiplicative_expression ( Plus multiplicative_expression | Minus multiplicative_expression )* ;
shift_expression :  additive_expression ( LeftShift additive_expression | Greater Greater additive_expression )* ;
relational_expression :  shift_expression ( Less shift_expression | Greater shift_expression | LessEqual shift_expression | GreaterEqual shift_expression )* ;
equality_expression :  relational_expression ( Equal relational_expression | NotEqual relational_expression )* ;
and_expression :  equality_expression ( ( And | KWBitAnd ) equality_expression )* ;
exclusive_or_expression :  and_expression ( ( Caret | KWXor ) and_expression )* ;
inclusive_or_expression :  exclusive_or_expression ( ( Or | KWBitOr ) exclusive_or_expression )* ;
logical_and_expression :  inclusive_or_expression ( ( AndAnd | KWAnd ) inclusive_or_expression )* ;
logical_or_expression :  logical_and_expression ( ( OrOr | KWOr ) logical_and_expression )* ;
conditional_expression :  logical_or_expression |  logical_or_expression Question expression Colon assignment_expression ;
assignment_expression :  conditional_expression |  logical_or_expression assignment_operator initializer_clause |  throw_expression ;
assignment_operator :  Assign | StarAssign | DivAssign | ModAssign | PlusAssign | MinusAssign | RightShiftAssign | LeftShiftAssign | AndAssign | XorAssign | OrAssign | KWAndEq | KWOrEq | KWXorEq ;
expression :  assignment_expression ( Comma assignment_expression )* ;
constant_expression_eof :  conditional_expression EOF ; constant_expression :  conditional_expression ;
statement :  labeled_statement |  attribute_specifier_seq ? expression_statement |  attribute_specifier_seq ? compound_statement |  attribute_specifier_seq ? selection_statement |  attribute_specifier_seq ? iteration_statement |  attribute_specifier_seq ? jump_statement |  declaration_statement |  attribute_specifier_seq ? try_block ;
labeled_statement :  attribute_specifier_seq ? Identifier Colon statement |  attribute_specifier_seq ? KWCase constant_expression Colon statement |  attribute_specifier_seq ? KWDefault Colon statement ;
expression_statement :  expression ? Semi ;
compound_statement :  LeftBrace statement_seq ? RightBrace ;
statement_seq :  statement+ ;
selection_statement :  KWIf LeftParen condition RightParen statement |  KWIf LeftParen condition RightParen statement KWElse statement |  KWSwitch LeftParen condition RightParen statement ;
condition :  expression |  attribute_specifier_seq ? decl_specifier_seq declarator Assign initializer_clause |  attribute_specifier_seq ? decl_specifier_seq declarator braced_init_list ;
iteration_statement :  KWWhile LeftParen condition RightParen statement |  KWDo statement KWWhile LeftParen expression RightParen Semi |  KWFor LeftParen for_init_statement condition ? Semi expression ? RightParen statement |  KWFor LeftParen for_range_declaration Colon for_range_initializer RightParen statement ;
for_init_statement :  expression_statement |  simple_declaration ;
for_range_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator ;
for_range_initializer :  expression |  braced_init_list ;
jump_statement :  KWBreak Semi |  KWContinue Semi |  KWReturn expression ? Semi |  KWReturn braced_init_list Semi |  KWGoto Identifier Semi ;
declaration_statement :  block_declaration ;
declaration_seq :  declaration+ ;
declaration :  block_declaration |  function_definition |  template_declaration |  explicit_instantiation |  explicit_specialization |  linkage_specification |  namespace_definition |  empty_declaration |  attribute_declaration ;
block_declaration :  simple_declaration |  asm_definition |  namespace_alias_definition |  using_declaration |  using_directive |  static_assert_declaration |  alias_declaration |  opaque_enum_declaration ;
alias_declaration :  KWUsing Identifier attribute_specifier_seq ? Assign type_id Semi ;
simple_declaration :  decl_specifier_seq ? init_declarator_list ? Semi |  attribute_specifier_seq decl_specifier_seq ? init_declarator_list Semi ;
static_assert_declaration :  KWStatic_assert LeftParen constant_expression Comma String_literal RightParen Semi ;
empty_declaration :  Semi ;
attribute_declaration :  attribute_specifier_seq Semi ;
decl_specifier :  storage_class_specifier |  type_specifier |  function_specifier |  KWFriend |  KWTypedef |  KWConstexpr ;
decl_specifier_seq :  decl_specifier* (  decl_specifier attribute_specifier_seq ? ) ;
storage_class_specifier :  KWRegister |  KWStatic |  KWThread_local |  KWExtern |  KWMutable ;
function_specifier :  KWInline |  KWVirtual |  KWExplicit ;
typedef_name :  Identifier ;
type_specifier :  trailing_type_specifier |  class_specifier |  enum_specifier ;
trailing_type_specifier :  simple_type_specifier |  elaborated_type_specifier |  typename_specifier |  cv_qualifier ;
type_specifier_seq :  type_specifier* (  type_specifier attribute_specifier_seq ? ) ;
trailing_type_specifier_seq :  trailing_type_specifier* (  trailing_type_specifier attribute_specifier_seq ? ) ;
simple_type_specifier :  nested_name_specifier ? type_name |  nested_name_specifier KWTemplate simple_template_id |  KWChar |  KWChar16 |  KWChar32 |  KWWchar |  KWBool |  KWShort |  KWInt |  KWLong |  KWSigned |  KWUnsigned |  KWFloat |  KWDouble |  KWVoid |  KWAuto |  decltype_specifier ;
type_name :  class_name |  enum_name |  typedef_name |  simple_template_id ;
decltype_specifier :  KWDecltype LeftParen expression RightParen |  KWDecltype LeftParen KWAuto RightParen ;
elaborated_type_specifier :  class_key attribute_specifier_seq ? nested_name_specifier ? Identifier |  class_key simple_template_id |  class_key nested_name_specifier KWTemplate ? simple_template_id |  KWEnum nested_name_specifier ? Identifier ;
enum_name :  Identifier ;
enum_specifier :  enum_head LeftBrace enumerator_list ? RightBrace |  enum_head LeftBrace enumerator_list Comma RightBrace ;
enum_head :  enum_key attribute_specifier_seq ? Identifier ? enum_base ? |  enum_key attribute_specifier_seq ? nested_name_specifier Identifier enum_base ? ;
opaque_enum_declaration :  enum_key attribute_specifier_seq ? Identifier enum_base ? Semi ;
enum_key :  KWEnum |  KWEnum KWClass |  KWEnum KWStruct ;
enum_base :  Colon type_specifier_seq ;
enumerator_list :  enumerator_definition ( Comma enumerator_definition )* ;
enumerator_definition :  enumerator |  enumerator Assign constant_expression ;
enumerator :  Identifier ;
namespace_name :  original_namespace_name |  namespace_alias ;
original_namespace_name :  Identifier ;
namespace_definition :  named_namespace_definition |  unnamed_namespace_definition ;
named_namespace_definition :  original_namespace_definition |  extension_namespace_definition ;
original_namespace_definition :  KWInline ? KWNamespace Identifier LeftBrace namespace_body RightBrace ;
extension_namespace_definition :  KWInline ? KWNamespace original_namespace_name LeftBrace namespace_body RightBrace ;
unnamed_namespace_definition :  KWInline ? KWNamespace LeftBrace namespace_body RightBrace ;
namespace_body :  declaration_seq ? ;
namespace_alias :  Identifier ;
namespace_alias_definition :  KWNamespace Identifier Assign qualified_namespace_specifier Semi ;
qualified_namespace_specifier :  nested_name_specifier ? namespace_name ;
using_declaration :  KWUsing KWTypename_ ? nested_name_specifier unqualified_id Semi |  KWUsing Doublecolon unqualified_id Semi ;
using_directive :  attribute_specifier_seq ? KWUsing KWNamespace nested_name_specifier ? namespace_name Semi ;
asm_definition :  KWAsm LeftParen String_literal RightParen Semi ;
linkage_specification :  KWExtern String_literal LeftBrace declaration_seq ? RightBrace |  KWExtern String_literal declaration ;
attribute_specifier_seq : attribute_specifier+ ;
attribute_specifier :  LeftBracket LeftBracket attribute_list RightBracket RightBracket |  alignment_specifier ;
alignment_specifier :  KWAlignas LeftParen type_id Ellipsis ? RightParen |  KWAlignas LeftParen constant_expression Ellipsis ? RightParen ;
attribute_list : (  attribute ? |  attribute Ellipsis ) ( Comma attribute ? | Comma attribute Ellipsis )* ;
attribute :  attribute_token attribute_argument_clause ? ;
attribute_token :  Identifier |  attribute_scoped_token ;
attribute_scoped_token :  attribute_namespace Doublecolon Identifier ;
attribute_namespace :  Identifier ;
attribute_argument_clause :  LeftParen balanced_token_seq RightParen ;
balanced_token_seq :  balanced_token ? balanced_token* ;
balanced_token :  LeftParen balanced_token_seq RightParen |  LeftBracket balanced_token_seq RightBracket |  LeftBrace balanced_token_seq RightBrace |  ~(LeftParen | RightParen | LeftBrace | RightBrace | LeftBracket | RightBracket)+ ;
init_declarator_list :  init_declarator ( Comma init_declarator )* ;
init_declarator :  declarator initializer ? ;
declarator :  ptr_declarator |  noptr_declarator parameters_and_qualifiers trailing_return_type ;
ptr_declarator :  ptr_operator*  noptr_declarator ;
noptr_declarator : (  declarator_id attribute_specifier_seq ? |  LeftParen ptr_declarator RightParen ) ( parameters_and_qualifiers | LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? )* ;
parameters_and_qualifiers :  LeftParen parameter_declaration_clause RightParen cv_qualifier_seq ?  ref_qualifier ? exception_specification ? attribute_specifier_seq ? ;
trailing_return_type :  Arrow trailing_type_specifier_seq abstract_declarator ? ;
ptr_operator :  Star attribute_specifier_seq ? cv_qualifier_seq ? |  And attribute_specifier_seq ? |  AndAnd attribute_specifier_seq ? |  nested_name_specifier Star attribute_specifier_seq ? cv_qualifier_seq ? ;
cv_qualifier_seq :  cv_qualifier+ ;
cv_qualifier :  KWConst |  KWVolatile ;
ref_qualifier :  And |  AndAnd ;
declarator_id :  Ellipsis ? id_expression ;
type_id :  type_specifier_seq abstract_declarator ? ;
abstract_declarator :  ptr_abstract_declarator |  noptr_abstract_declarator ? parameters_and_qualifiers trailing_return_type |  abstract_pack_declarator ;
ptr_abstract_declarator :  ptr_operator+  noptr_abstract_declarator ;
noptr_abstract_declarator : (  LeftParen ptr_abstract_declarator RightParen ) ( parameters_and_qualifiers | LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? )+ ;
abstract_pack_declarator :  ptr_operator*  noptr_abstract_pack_declarator ;
noptr_abstract_pack_declarator :  Ellipsis ( parameters_and_qualifiers | LeftBracket constant_expression ? RightBracket attribute_specifier_seq ? )* ;
parameter_declaration_clause :  parameter_declaration_list ? Ellipsis ? |  parameter_declaration_list Comma Ellipsis ;
parameter_declaration_list :  parameter_declaration ( Comma parameter_declaration )* ;
parameter_declaration :  attribute_specifier_seq ? decl_specifier_seq declarator |  attribute_specifier_seq ? decl_specifier_seq declarator Assign initializer_clause |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? |  attribute_specifier_seq ? decl_specifier_seq abstract_declarator ? Assign initializer_clause ;
function_definition :  attribute_specifier_seq ? decl_specifier_seq ? declarator virt_specifier_seq ? function_body ;
function_body :  ctor_initializer ? compound_statement |  function_try_block |  Assign KWDefault Semi |  Assign KWDelete Semi ;
initializer :  brace_or_equal_initializer |  LeftParen expression_list RightParen ;
brace_or_equal_initializer :  Assign initializer_clause |  braced_init_list ;
initializer_clause :  assignment_expression |  braced_init_list ;
initializer_list : (  initializer_clause Ellipsis ? ) ( Comma initializer_clause Ellipsis ? )* ;
braced_init_list :  LeftBrace initializer_list Comma ? RightBrace |  LeftBrace RightBrace ;
class_name :  Identifier |  simple_template_id ;
class_specifier :  class_head LeftBrace member_specification ? RightBrace ;
class_head :  class_key attribute_specifier_seq ? class_head_name class_virt_specifier ? base_clause ? |  class_key attribute_specifier_seq ? base_clause ? ;
class_head_name :  nested_name_specifier ? class_name ;
class_virt_specifier :  KWFinal ;
class_key :  KWClass |  KWStruct |  KWUnion ;
member_specification : (  member_declaration |  access_specifier Colon )+ ;
member_declaration :  attribute_specifier_seq ? decl_specifier_seq ? member_declarator_list ? Semi |  function_definition |  using_declaration |  static_assert_declaration |  template_declaration |  alias_declaration |  empty_declaration ;
member_declarator_list :  member_declarator ( Comma member_declarator )* ;
member_declarator :  declarator virt_specifier_seq ? pure_specifier ? |  declarator brace_or_equal_initializer ? |  Identifier ? attribute_specifier_seq ? Colon constant_expression ;
virt_specifier_seq :  virt_specifier+ ;
virt_specifier :  KWOverride |  KWFinal ;
pure_specifier :  Assign Octal_literal ;
base_clause :  Colon base_specifier_list ;
base_specifier_list : (  base_specifier Ellipsis ? ) ( Comma base_specifier Ellipsis ? )* ;
base_specifier :  attribute_specifier_seq ? base_type_specifier |  attribute_specifier_seq ? KWVirtual access_specifier ? base_type_specifier |  attribute_specifier_seq ? access_specifier KWVirtual ? base_type_specifier ;
class_or_decltype :  nested_name_specifier ? class_name |  decltype_specifier ;
base_type_specifier :  class_or_decltype ;
access_specifier :  KWPrivate |  KWProtected |  KWPublic ;
conversion_function_id :  KWOperator conversion_type_id ;
conversion_type_id :  type_specifier_seq conversion_declarator ? ;
conversion_declarator :  ptr_operator+ ;
ctor_initializer :  Colon mem_initializer_list ;
mem_initializer_list : (  mem_initializer Ellipsis ? Comma )* (  mem_initializer Ellipsis ? ) ;
mem_initializer :  mem_initializer_id LeftParen expression_list ? RightParen |  mem_initializer_id braced_init_list ;
mem_initializer_id :  class_or_decltype |  Identifier ;
operator_function_id :  KWOperator operator ;
operator :  KWNew | KWDelete | KWNew LeftBracket RightBracket | KWDelete LeftBracket RightBracket | Plus | Minus | Star | Div | Mod | Caret | And | Or | Tilde | Not | Assign | Less | Greater | PlusAssign | MinusAssign | StarAssign | DivAssign | ModAssign | XorAssign | AndAssign | OrAssign | LeftShift | Greater Greater | RightShiftAssign | LeftShiftAssign | Equal | NotEqual | LessEqual | GreaterEqual | AndAnd | OrOr | PlusPlus | MinusMinus | Comma | ArrowStar | Arrow | LeftParen RightParen | LeftBracket RightBracket ;
literal_operator_id :  KWOperator String_literal Identifier |  KWOperator User_defined_string_literal ;
template_declaration :  KWTemplate Less template_parameter_list Greater declaration ;
template_parameter_list :  template_parameter ( Comma template_parameter )* ;
template_parameter :  type_parameter |  parameter_declaration ;
type_parameter :  KWClass Ellipsis ? Identifier ? |  KWClass Identifier ? Assign type_id |  KWTypename_ Ellipsis ? Identifier ? |  KWTypename_ Identifier ? Assign type_id |  KWTemplate Less template_parameter_list Greater KWClass Ellipsis ? Identifier ? |  KWTemplate Less template_parameter_list Greater KWClass Identifier ? Assign id_expression ;
simple_template_id :  template_name Less template_argument_list ? Greater ;
template_id :  simple_template_id |  operator_function_id Less template_argument_list ? Greater |  literal_operator_id Less template_argument_list ? Greater ;
template_name :  Identifier ;
template_argument_list : (  template_argument Ellipsis ? ) ( Comma template_argument Ellipsis ? )* ;
template_argument :  constant_expression |  type_id |  id_expression ;
typename_specifier :  KWTypename_ nested_name_specifier Identifier |  KWTypename_ nested_name_specifier KWTemplate ? simple_template_id ;
explicit_instantiation :  KWExtern ? KWTemplate declaration ;
explicit_specialization :  KWTemplate Less Greater declaration ;
try_block :  KWTry compound_statement handler_seq ;
function_try_block :  KWTry ctor_initializer ? compound_statement handler_seq ;
handler_seq :  handler+ ;
handler :  KWCatch LeftParen exception_declaration RightParen compound_statement ;
exception_declaration :  attribute_specifier_seq ? type_specifier_seq declarator |  attribute_specifier_seq ? type_specifier_seq abstract_declarator ? |  Ellipsis ;
throw_expression :  KWThrow assignment_expression ? ;
exception_specification :  dynamic_exception_specification |  noexcept_specification ;
dynamic_exception_specification :  KWThrow LeftParen type_id_list ? RightParen ;
type_id_list : (  type_id Ellipsis ? ) ( Comma type_id Ellipsis ? )* ;
noexcept_specification :  KWNoexcept LeftParen constant_expression RightParen |  KWNoexcept ;
preprocessing_file :  group ? EOF ;
group :  group_part+ ;
group_part :  if_section |  control_line |  text_line ; //  |  Pound non_directive ;
if_section :  if_group elif_groups ? else_group ? endif_line ;
if_group :  Pound KWIf constant_expression new_line group ? |  Pound KWIfdef Identifier new_line group ? |  Pound KWIfndef Identifier new_line group ? ;
elif_groups :  elif_group+ ;
elif_group :  Pound KWElif constant_expression new_line group ? ;
else_group :  Pound KWElse new_line group ? ;
endif_line :  Pound KWEndif new_line ;
control_line :  Pound KWInclude pp_tokens new_line |  Pound KWDefine Identifier replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list ? RightParen replacement_list new_line |  Pound KWDefine Identifier lparen Ellipsis RightParen replacement_list new_line |  Pound KWDefine Identifier lparen identifier_list Comma Ellipsis RightParen replacement_list new_line |  Pound KWUndef Identifier new_line |  Pound KWLine pp_tokens new_line |  Pound KWError pp_tokens ? new_line |  Pound KWPragma pp_tokens ? new_line |  Pound new_line ;
text_line :  { InputStream.LA(1) != CPlusPlus14Lexer.Pound }? pp_tokens ? new_line ;
non_directive :  pp_tokens new_line ;
lparen :  LeftParen ;
identifier_list :  Identifier ( Comma Identifier )* ;
replacement_list :  pp_tokens ? ;
pp_tokens :  preprocessing_token+ ;
new_line :  Newline ;

// Defs from "addin3".

keyword : KWAlignas | KWContinue | KWFriend | KWRegister | KWTrue_ | KWAlignof | KWDecltype | KWGoto | KWReinterpret_cast | KWTry | KWAsm | KWDefault | KWIf | KWReturn | KWTypedef | KWAuto | KWDelete | KWInline | KWShort | KWTypeid_ | KWBool | KWDo | KWInt | KWSigned | KWTypename_ | KWBreak | KWDouble | KWLong | KWSizeof | KWUnion | KWCase | KWDynamic_cast | KWMutable | KWStatic | KWUnsigned | KWCatch | KWElse | KWNamespace | KWStatic_assert | KWUsing | KWChar | KWEnum | KWNew | KWStatic_cast | KWVirtual | KWChar16 | KWExplicit | KWNoexcept | KWStruct | KWVoid | KWChar32 | KWExport | KWNullptr | KWSwitch | KWVolatile | KWClass | KWExtern | KWOperator | KWTemplate | KWWchar | KWConst | KWFalse_ | KWPrivate | KWThis | KWWhile | KWConstexpr | KWFloat | KWProtected | KWThread_local | KWConst_cast | KWFor | KWPublic | KWThrow | KWAnd | KWAndEq | KWBitAnd | KWBitOr | KWCompl | KWNot | KWNotEq | KWOr | KWOrEq | KWXor | KWXorEq ;
punctuator : preprocessing_op_or_punc ;
