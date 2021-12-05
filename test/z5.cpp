
#define XXX 1
#if XXX && 1
int ok1() {}
#endif

#define YYY 1
#if YYY && XXX
int ok2() {}
#endif

