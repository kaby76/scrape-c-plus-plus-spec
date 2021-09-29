#define XX asdf

#ifdef XX
int foo1() {}
#endif

#ifndef XX
int foo2() {}
#endif

union a {
	int b;
	float c;
};

union b : a
{
	int d;
}
int f()
{
	return 1;
}

main()
{
	return 0;
}
