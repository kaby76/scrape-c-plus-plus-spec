
#if defined(XXX)
void bad1() {}
#endif

#define YYY 1
#if defined(YYY)
void ok2() {}
#endif

#define YYY 1
#if !defined(YYY)
void bad2() {}
#endif

#if !defined(XXX)
void ok3() {}
#endif

#if defined YYY
void ok4() {}
#endif
