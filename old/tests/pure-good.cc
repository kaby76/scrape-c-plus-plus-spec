class Dummy
{
	virtual void Process() = 0;
};
void Dummy::Process()
{} //compiles on both GCC and VS2005


class Foo {
	static const int foo = 1;
	static const int bar = 0;
};
