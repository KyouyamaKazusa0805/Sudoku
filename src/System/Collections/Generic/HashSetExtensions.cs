namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="HashSet{T}"/>.
/// </summary>
/// <seealso cref="HashSet{T}"/>
public static class HashSetExtensions
{
	/// <summary>
	/// Try to convert a <see cref="HashSet{T}"/> into an array, without any conversions among internal values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The value to be converted.</param>
	/// <returns>An array converted.</returns>
	public static T[] ToArray<T>(this HashSet<T> @this)
	{
		var result = new T[@this.Count];
		var enumerator = @this.GetEnumerator();
		var i = 0;
		while (enumerator.MoveNext())
		{
			var currentRef = HashSetEntry<T>.EnumeratorEntry.GetCurrentFieldRef(ref enumerator);
			result[i++] = currentRef;
		}
		return result;
	}

	/// <summary>
	/// Try to convert a <see cref="HashSet{T}"/> into a <see cref="ReadOnlySpan{T}"/>,
	/// without any conversions among internal values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The value to be converted.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnlySpan<T>(this HashSet<T> @this) => @this.ToArray();
}

/// <summary>
/// Represents an entry to call internal fields on <see cref="HashSet{T}"/>.
/// </summary>
/// <typeparam name="T">The type of each element in <see cref="HashSet{T}"/>.</typeparam>
/// <seealso cref="HashSet{T}"/>
file static class HashSetEntry<T>
{
	/// <summary>
	/// Represents an entry to call internal fields on <see cref="HashSet{T}.Enumerator"/>.
	/// </summary>
	/// <seealso cref="HashSet{T}.Enumerator"/>
	public static class EnumeratorEntry
	{
		/// <summary>
		/// Try to fetch the internal field <c>_current</c> in type <see cref="HashSet{T}.Enumerator"/>.
		/// </summary>
		/// <param name="this">The set.</param>
		/// <returns>The reference to the internal field.</returns>
		/// <remarks>
		/// <include
		///     file="../../global-doc-comments.xml"
		///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
		/// <include
		///     file="../../global-doc-comments.xml"
		///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
		/// </remarks>
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.Enumerator_Current)]
		public static extern ref T GetCurrentFieldRef(ref HashSet<T>.Enumerator @this);
	}
}
