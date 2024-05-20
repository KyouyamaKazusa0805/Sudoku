namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Has</c>.
/// </summary>
/// <inheritdoc/>
public interface IHasMethod<TSelf, TSource> : ICustomLinqMethod<TSelf, TSource> where TSelf : IHasMethod<TSelf, TSource>
{
	/// <summary>
	/// Determine whether the collection contains an element of the specified type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the constraint to be checked.</typeparam>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public virtual bool Has<T>() where T : TSource
	{
		foreach (var element in this)
		{
			if (element is T)
			{
				return true;
			}
		}
		return false;
	}
}
