union a {
	int b;
	float c;
};


union [[deprecated]] b
{
	int d;
};

int f()
{
	return 1;
}

main()
{
	return 0;
}
