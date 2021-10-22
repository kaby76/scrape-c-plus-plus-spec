
#define FOO(a) a
#define DOUBLE(x) (2*x)
#define FOO2(a,b,c) a + DOUBLE(b) + c

FOO(main)
{
    FOO2(1,2+2,3*(5+8));
}
