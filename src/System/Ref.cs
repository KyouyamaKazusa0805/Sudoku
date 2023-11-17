using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace System;

/// <summary>
/// Represents a list of methods that can check for the concept "References" defined in C#.
/// </summary>
public static class Ref
{
	/// <summary>
	/// Swaps for two elements.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be swapped.</param>
	/// <param name="right">The second element to be swapped.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(scoped ref T left, scoped ref T right)
	{
		if (!MemoryLocationAreSame(in left, in right))
		{
			var temp = left;
			left = right;
			right = temp;
		}
	}

	/// <summary>
	/// Determines whether the current reference points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">The reference to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullReference<T>(scoped ref readonly T reference) => IsNullRef(in reference);

	/// <summary>
	/// Check whether two references point to a same memory location.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be checked.</param>
	/// <param name="right">The second element to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool MemoryLocationAreSame<T>(scoped ref readonly T left, scoped ref readonly T right)
		=> AreSame(in left, in right);

	/// <summary>
	/// Returns a reference that points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the element the reference points to if this reference were not <see langword="null"/>.
	/// </typeparam>
	/// <returns>A read-only reference that points to <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T MakeNullReference<T>() => ref NullRef<T>();

	/// <summary>
	/// Get the new array from the reference to the block memory start position, with the specified start index.
	/// </summary>
	/// <typeparam name="T">The type of the pointed element.</typeparam>
	/// <param name="memorySpan">The reference to the block memory start position.</param>
	/// <param name="start">The start index that you want to pick from.</param>
	/// <param name="count">The length of the array that the reference points to.</param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="memorySpan"/> is <see langword="null"/>.
	/// </exception>
	public static unsafe ReadOnlySpan<T> Slice<T>(scoped ref readonly T memorySpan, int start, int count)
	{
		ThrowIfNullRef(in memorySpan);

		var result = new T[count - start];
		for (var i = start; i < count; i++)
		{
			result[i - start] = AddByteOffset(ref AsRef(in memorySpan), sizeof(T) * i);
		}

		return result;
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
	public static unsafe ReadOnlySpan<int> Slice(int* ptr, int length, int index, bool removeTrailingZeros)
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

	/// <summary>
	/// Throws an <see cref="ArgumentNullRefException"/> if the argument points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">
	/// <para>The reference to the target element, or maybe a <see langword="null"/> reference.</para>
	/// <para><i>
	/// Please note that the argument requires a <see langword="ref"/> modifier, but it does not modify the referenced value
	/// of the argument. It is nearly equal to <see langword="in"/> modifier.
	/// However, the method will invoke <see cref="IsNullRef{T}(ref readonly T)"/>,
	/// where the only argument is passed by <see langword="ref"/>.
	/// Therefore, here the current method argument requires a modifier <see langword="ref"/> instead of <see langword="in"/>.
	/// </i></para>
	/// </param>
	/// <param name="paramName">
	/// The parameter name. <b>This argument needn't to be assigned because it will be replaced with a new value by compiler.</b>
	/// </param>
	/// <exception cref="ArgumentNullRefException">Throws if the argument is a <see langword="null"/> reference.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<T>(
		scoped ref readonly T reference,
		[ConstantExpected, CallerArgumentExpression(nameof(reference))] string? paramName = null
	)
	{
		if (IsNullReference(in reference))
		{
			throw new ArgumentNullRefException(nameof(reference));
		}
	}
}
