
#define FOO(a) a
#define FOO2(a,b,c) a; b; c;

FOO(main)
{
	FOO2(1, 2+2, 3*(5+8));
}
