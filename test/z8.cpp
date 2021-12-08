#  define __GNUC_PREREQ(maj, min) ((__GNUC__ << 16) + __GNUC_MINOR__ >= ((maj) << 16) + (min))
//#  define __GNUC_PREREQ(maj, min) 0
#define	__GNUC_PREREQ__(ma, mi)	__GNUC_PREREQ(ma, mi)

void foo()
{
	__GNUC_PREREQ__(4,1);
}
