namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports customized LINQ method defined in this repository.
/// </summary>
/// <inheritdoc/>
public interface ICustomLinqMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		ICustomLinqMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
	;
