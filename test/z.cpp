#define XX asdf

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
