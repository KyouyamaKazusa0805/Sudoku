namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Sum</c> and <c>SumBy</c>.
/// </summary>
/// <inheritdoc/>
public interface ISumProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : ISumProvider<TSelf, TSource>
	where TSource : IAdditiveIdentity<TSource, TSource>, IAdditionOperators<TSource, TSource, TSource>
{
	/// <inheritdoc/>
	public virtual TSource Sum()
	{
		var result = TSource.AdditiveIdentity;
		foreach (var element in this)
		{
			result += element;
		}
		return result;
	}
}
