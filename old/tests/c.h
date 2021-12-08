#ifndef _GLIBCXX_CXX_CONFIG_H

#define _GLIBCXX_CXX_CONFIG_H 1
#define _GLIBCXX_RELEASE 10
#define __GLIBCXX__ 20200723

#ifndef _GLIBCXX_PURE
# define _GLIBCXX_PURE __attribute__ ((__pure__))
#endif

#ifndef _GLIBCXX_CONST
# define _GLIBCXX_CONST __attribute__ ((__const__))
#endif

#ifndef _GLIBCXX_NORETURN
# define _GLIBCXX_NORETURN __attribute__ ((__noreturn__))
#endif

#ifndef _GLIBCXX_NOTHROW
# ifndef __cplusplus
#  define _GLIBCXX_NOTHROW __attribute__((__nothrow__))
# endif
#endif

# define _GLIBCXX_HAVE_ATTRIBUTE_VISIBILITY 0

#if _GLIBCXX_HAVE_ATTRIBUTE_VISIBILITY
# define _GLIBCXX_VISIBILITY(V) __attribute__ ((__visibility__ (#V)))
#else
# define _GLIBCXX_VISIBILITY(V) _GLIBCXX_PSEUDO_VISIBILITY(V)
#endif

#ifndef _GLIBCXX_USE_DEPRECATED
# define _GLIBCXX_USE_DEPRECATED 1
#endif

#if defined(__DEPRECATED) && (__cplusplus >= 201103L)
# define _GLIBCXX_DEPRECATED __attribute__ ((__deprecated__))
#else
# define _GLIBCXX_DEPRECATED
#endif

#endif
