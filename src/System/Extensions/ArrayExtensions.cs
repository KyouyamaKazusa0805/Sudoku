namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Creates a new <see cref="Array"/> instance of type <typeparamref name="T"/>,
	/// with all elements in the current instance, except the element at the specified index.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="index">The desired index.</param>
	/// <returns>The target list.</returns>
	public static T[] CopyExcept<T>(this T[] @this, int index)
	{
		var result = new T[@this.Length - 1];
		for (var i = 0; i < index; i++)
		{
			result[i] = @this[i];
		}
		for (var i = index + 1; i < @this.Length; i++)
		{
			result[i - 1] = @this[i];
		}

		return result;
	}
}
