#if defined(_GLIBCXX_DEBUG) || defined(_GLIBCXX_PARALLEL)
namespace std
{
_GLIBCXX_BEGIN_NAMESPACE_VERSION
  namespace __cxx1998
  {
# if _GLIBCXX_USE_CXX11_ABI
  inline namespace __cxx11 __attribute__((__abi_tag__ ("cxx11"))) { }
# endif
  }

_GLIBCXX_END_NAMESPACE_VERSION

# ifdef _GLIBCXX_DEBUG
  inline namespace __debug { }
# endif

# ifdef _GLIBCXX_PARALLEL
  inline namespace __parallel { }
# endif
}

# if defined(_GLIBCXX_DEBUG) && defined(_GLIBCXX_PARALLEL)
#  error illegal use of multiple inlined namespaces
# endif

//# if __NO_INLINE__ && !__GXX_WEAK__
//#  warning currently using inlined namespace mode which may fail
//# endif
#endif

#if defined(_GLIBCXX_DEBUG)
# define _GLIBCXX_STD_C __cxx1998
# define _GLIBCXX_BEGIN_NAMESPACE_CONTAINER \
	 namespace _GLIBCXX_STD_C {
# define _GLIBCXX_END_NAMESPACE_CONTAINER }
#else
# define _GLIBCXX_STD_C std
# define _GLIBCXX_BEGIN_NAMESPACE_CONTAINER
# define _GLIBCXX_END_NAMESPACE_CONTAINER
#endif

#ifdef _GLIBCXX_PARALLEL
# define _GLIBCXX_STD_A __cxx1998
# define _GLIBCXX_BEGIN_NAMESPACE_ALGO \
	 namespace _GLIBCXX_STD_A {
# define _GLIBCXX_END_NAMESPACE_ALGO }
#else
# define _GLIBCXX_STD_A std
# define _GLIBCXX_BEGIN_NAMESPACE_ALGO
# define _GLIBCXX_END_NAMESPACE_ALGO
#endif

#undef _GLIBCXX_LONG_DOUBLE_COMPAT

#if __LONG_DOUBLE_128__

asdf
#endif
