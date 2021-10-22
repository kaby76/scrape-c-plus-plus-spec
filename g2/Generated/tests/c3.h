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

#if defined _GLIBCXX_LONG_DOUBLE_COMPAT && defined __LONG_DOUBLE_128__
namespace std
{
  inline namespace __gnu_cxx_ldbl128 { }
}
# define _GLIBCXX_NAMESPACE_LDBL __gnu_cxx_ldbl128::
# define _GLIBCXX_BEGIN_NAMESPACE_LDBL namespace __gnu_cxx_ldbl128 {
# define _GLIBCXX_END_NAMESPACE_LDBL }
#else
# define _GLIBCXX_NAMESPACE_LDBL
# define _GLIBCXX_BEGIN_NAMESPACE_LDBL
# define _GLIBCXX_END_NAMESPACE_LDBL
#endif
#if _GLIBCXX_USE_CXX11_ABI
# define _GLIBCXX_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_NAMESPACE_CXX11
# define _GLIBCXX_BEGIN_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_BEGIN_NAMESPACE_CXX11
# define _GLIBCXX_END_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_END_NAMESPACE_CXX11
#else
# define _GLIBCXX_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_NAMESPACE_LDBL
# define _GLIBCXX_BEGIN_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_BEGIN_NAMESPACE_LDBL
# define _GLIBCXX_END_NAMESPACE_LDBL_OR_CXX11 _GLIBCXX_END_NAMESPACE_LDBL
#endif

#if defined(_GLIBCXX_DEBUG) && !defined(_GLIBCXX_ASSERTIONS)
# define _GLIBCXX_ASSERTIONS 1
#endif

#ifdef _GLIBCXX_ASSERTIONS
# undef _GLIBCXX_EXTERN_TEMPLATE
# define _GLIBCXX_EXTERN_TEMPLATE -1
#endif

#if defined(_GLIBCXX_ASSERTIONS) \
  || defined(_GLIBCXX_PARALLEL) || defined(_GLIBCXX_PARALLEL_ASSERTIONS)
namespace std
{
  extern "C++" inline void
  __replacement_assert(const char* __file, int __line,
		       const char* __function, const char* __condition)
  {
    __builtin_printf("%s:%d: %s: Assertion '%s' failed.\n", __file, __line,
		     __function, __condition);
    __builtin_abort();
  }
}
#define __glibcxx_assert_impl(_Condition)				 \
  do 									 \
  {							      		 \
    if (! (_Condition))                                                  \
      std::__replacement_assert(__FILE__, __LINE__, __PRETTY_FUNCTION__, \
				#_Condition);				 \
  } while (false)
#endif

#if defined(_GLIBCXX_ASSERTIONS)
# define __glibcxx_assert(_Condition) __glibcxx_assert_impl(_Condition)
#else
# define __glibcxx_assert(_Condition)
#endif

#ifndef _GLIBCXX_SYNCHRONIZATION_HAPPENS_BEFORE
# define  _GLIBCXX_SYNCHRONIZATION_HAPPENS_BEFORE(A)
#endif
#ifndef _GLIBCXX_SYNCHRONIZATION_HAPPENS_AFTER
# define  _GLIBCXX_SYNCHRONIZATION_HAPPENS_AFTER(A)
#endif

# define _GLIBCXX_BEGIN_EXTERN_C extern "C" {
# define _GLIBCXX_END_EXTERN_C }

# define _GLIBCXX_USE_ALLOCATOR_NEW 1

#else // !__cplusplus
# define _GLIBCXX_BEGIN_EXTERN_C
# define _GLIBCXX_END_EXTERN_C
#endif

//#include <bits/os_defines.h>

//#include <bits/cpu_defines.h>

#ifndef _GLIBCXX_PSEUDO_VISIBILITY
# define _GLIBCXX_PSEUDO_VISIBILITY(V)
#endif

#ifndef _GLIBCXX_WEAK_DEFINITION
# define _GLIBCXX_WEAK_DEFINITION
#endif

#ifndef _GLIBCXX_USE_WEAK_REF
# define _GLIBCXX_USE_WEAK_REF __GXX_WEAK__
#endif
