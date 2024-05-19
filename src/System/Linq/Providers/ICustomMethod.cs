namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports customized LINQ method defined in this repository.
/// </summary>
/// <inheritdoc/>
public interface ICustomMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource> where TSelf : ICustomMethod<TSelf, TSource>;
