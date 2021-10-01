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

#ifndef X
main4() { }
#endif

#ifndef Y
main5() { }
#endif

