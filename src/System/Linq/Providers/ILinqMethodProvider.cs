namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports a certain method group defined in <see cref="Enumerable"/>, well-known as LINQ methods.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TSource">The type of each element that the type supports for iteration.</typeparam>
/// <seealso cref="Enumerable"/>
public interface ILinqMethodProvider<TSelf, TSource> : IEnumerable<TSource> where TSelf : ILinqMethodProvider<TSelf, TSource>;
