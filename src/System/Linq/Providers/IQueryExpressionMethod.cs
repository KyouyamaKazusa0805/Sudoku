namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method groups that will be target method invocation of query expressions.
/// </summary>
/// <inheritdoc/>
public interface IQueryExpressionMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IQueryExpressionMethod<TSelf, TSource>
{
	/// <inheritdoc/>
	static bool ILinqMethod<TSelf, TSource>.SupportsQuerySyntax => true;
}
