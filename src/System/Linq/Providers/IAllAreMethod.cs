namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>AllAre</c>.
/// </summary>
/// <inheritdoc/>
public interface IAllAreMethod<TSelf, TSource> : ICustomLinqMethod<TSelf, TSource>
	where TSelf :
		IAllAreMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <summary>
	/// Determine whether all elements are of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the target elements.</typeparam>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public virtual bool AllAre<T>() where T : TSource?
	{
		foreach (var element in this)
		{
			if (element is not T)
			{
				return false;
			}
		}
		return true;
	}
}
