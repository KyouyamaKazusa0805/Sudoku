using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Provides methods for pointer handling.
/// </summary>
public static unsafe class PointerOperations
{
	/// <summary>
	/// To swap the two variables using pointers.
	/// </summary>
	/// <typeparam name="T">The type of the variable.</typeparam>
	/// <param name="left">The left variable.</param>
	/// <param name="right">The right variable.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(T* left, T* right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		var temp = *left;
		*left = *right;
		*right = temp;
	}

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
	public static T[] Slice<T>(T* ptr, int index, int length)
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
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <param name="index">The start index that you want to pick from.</param>
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
	public static int[] Slice(int* ptr, int length, int index, bool removeTrailingZeros)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		if (removeTrailingZeros)
		{
			var count = 0;
			var p = ptr + length - 1;
			for (var i = length - 1; i >= 0; i--, p--, count++)
			{
				if (*p != 0)
				{
					break;
				}
			}

			var result = new int[length - count - index];
			for (var i = index; i < length - count; i++)
			{
				result[i - index] = ptr[i];
			}

			return result;
		}

		return Slice(ptr, index, length);
	}
}
