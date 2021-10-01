#define X 1
#if X
main() { }
#endif

#ifdef X
main2() { }
#endif

#ifdef Y
main3() { }
#endif
