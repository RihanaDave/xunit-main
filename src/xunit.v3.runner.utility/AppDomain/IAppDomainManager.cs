using System;
using System.Reflection;

namespace Xunit;

interface IAppDomainManager : IDisposable
{
	bool HasAppDomain { get; }

	TObject? CreateObject<TObject>(
		AssemblyName assemblyName,
		string typeName,
		params object?[]? args)
			where TObject : class;

#if NETFRAMEWORK
	TObject? CreateObjectFrom<TObject>(
		string assemblyLocation,
		string typeName,
		params object?[]? args)
			where TObject : class;
#endif
}
