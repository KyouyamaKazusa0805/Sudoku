namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <seealso cref="ReadOnlySpan{T}"/>
	public static class ReadOnlySpanEx
	{
		/// <summary>
		/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
		/// clause.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <typeparam name="TResult">The result type.</typeparam>
		/// <param name="this">(<see langword="this in"/> parameter) The list.</param>
		/// <param name="selector">The selector that is used for conversion.</param>
		/// <returns>The array of target result elements.</returns>
		/// <example>
		/// For example:
		/// <code>
		/// <see langword="int"/>[] selection = <see langword="from"/> digit <see langword="in"/> 17.GetAllSets() <see langword="select"/> digit + 1;
		/// </code>
		/// </example>
		public static TResult[] Select<T, TResult>(this in ReadOnlySpan<T> @this, Func<T, TResult> selector)
		{
			var result = new TResult[@this.Length];
			int i = 0;
			foreach (var element in @this)
			{
				result[i++] = selector(element);
			}

			return result;
		}
	}
}
