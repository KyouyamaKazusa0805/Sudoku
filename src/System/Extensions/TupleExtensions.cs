namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides with extension methods on <see cref="ITuple"/>.
/// </summary>
/// <seealso cref="ITuple"/>
public static class TupleExtensions
{
	/// <summary>
	/// Converts the <see cref="ITuple"/> instance into an array of objects.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The array of elements.</returns>
	public static object?[] ToArray(this ITuple @this)
	{
		var result = new object?[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = element;
		}

		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TupleEnumerator GetEnumerator(this ITuple @this) => new(@this);
}
