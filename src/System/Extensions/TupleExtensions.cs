namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides with extension methods on <see cref="ITuple"/>.
/// </summary>
/// <seealso cref="ITuple"/>
public static class TupleExtensions
{
	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TupleEnumerator GetEnumerator(this ITuple @this) => new(@this);
}
