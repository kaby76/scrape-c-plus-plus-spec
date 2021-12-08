#ifdef DEBUG
#define MYDEBUG(X) (X)
#else
#define MYDEBUG(X)
#endif

MYDEBUG({
	if (shit_happens) {
		cerr << "help!" << endl;
		....
	}
});
