namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports customized LINQ method defined in this repository.
/// </summary>
/// <inheritdoc/>
public interface ICustomLinqMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : ICustomLinqMethod<TSelf, TSource>, allows ref struct
	where TSource : allows ref struct;
