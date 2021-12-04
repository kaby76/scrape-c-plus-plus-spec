
#ifdef XXX
int hi() {}
#endif

#define YYY 1
#ifdef YYY
int thisshouldbethere() {}
#endif

#if YYY
int alsoshouldbethere() {}
#endif

#if !YYY
int shouldnotbethere() {}
#endif
