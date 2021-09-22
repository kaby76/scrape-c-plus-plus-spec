#include <iostream>

class a {
	public:
		int  x;
		int operator +(class a & other)
		{
			return other.x + x;
		}
		int operator /=(class a & other)
		{
			return other.x / x;
		}
};

main()
{
	class a	     J;
	class a	     K;
	J.x = 1;
	K.x = 2;
	std::cout << J + K << std::endl;
	J /= K;
	return 0;
}
