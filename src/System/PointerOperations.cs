namespace System;

/// <summary>
/// Represents a list of methods operating with pointers.
/// </summary>
public static class PointerOperations
{
	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <typeparam name="T">The type of the pointer element.</typeparam>
	/// <param name="ptr">The pointer.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>[0, 1, 3, 6, 10]</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>[3, 6, 10]</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	public static unsafe ReadOnlySpan<T> Slice<T>(T* ptr, int index, int length)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = new T[length - index];
		for (var i = index; i < length; i++)
		{
			result[i - index] = ptr[i];
		}
		return result;
	}

	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <param name="removeTrailingZeros">
	/// Indicates whether the method will remove the trailing zeros. If <see langword="false"/>,
	/// the method will be same as <see cref="Slice{T}(T*, int, int)"/>.
	/// </param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>[0, 1, 3, 6, 10]</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>[3, 6, 10]</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	/// <seealso cref="Slice{T}(T*, int, int)"/>
	public static unsafe ReadOnlySpan<T> Slice<T>(T* ptr, int index, int length, bool removeTrailingZeros)
		where T : IBinaryInteger<T>
	{
		ArgumentNullException.ThrowIfNull(ptr);

		if (removeTrailingZeros)
		{
			var count = 0;
			var p = ptr + length - 1;
			for (var i = length - 1; i >= 0; i--, p--, count++)
			{
				if (*p != T.Zero)
				{
					break;
				}
			}

			var result = new T[length - count - index];
			for (var i = index; i < length - count; i++)
			{
				result[i - index] = ptr[i];
			}
			return result;
		}
		return Slice(ptr, index, length);
	}
}
