using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides methods on pointers.
	/// </summary>
	/// <remarks>
	/// Different with other types, pointers can't be as <see langword="this"/> parameter.
	/// </remarks>
	public static unsafe class Pointer
	{
		/// <summary>
		/// To swap the two variables using pointers when the pointee is an <see langword="unmanaged"/> type.
		/// </summary>
		/// <typeparam name="TUnmanaged">The type of the variable.</typeparam>
		/// <param name="left">The left variable.</param>
		/// <param name="right">The right variable.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Swap<TUnmanaged>(TUnmanaged* left, TUnmanaged* right)
			where TUnmanaged : unmanaged
		{
			var temp = *left;
			*left = *right;
			*right = temp;
		}

		/// <summary>
		/// Get the length of the specified string which is represented by a <see cref="char"/>*.
		/// </summary>
		/// <param name="ptr">The pointer.</param>
		/// <returns>The total length.</returns>
		/// <remarks>
		/// In C#, this function is unsafe because the implementation of
		/// <see cref="string"/> types between C and C# is totally different.
		/// In C, <see cref="string"/> is like a <see cref="char"/>* or a
		/// <see cref="char"/>[], they ends with the terminator symbol <c>'\0'</c>.
		/// However, C# not.
		/// </remarks>
		public static int StringLengthOf(char* ptr)
		{
			int result = 0;
			for (char* p = ptr; *p != '\0'; p++, result++) ;
			return result;
		}

		/// <summary>
		/// Get the new array from the pointer, with the specified start index.
		/// </summary>
		/// <typeparam name="TUnmanaged">
		/// The type of the pointer element. Note that the type should be <see langword="unmanaged"/>
		/// in order to use pointer handling. Therefore, <see langword="managed"/> types shouldn't be allowed.
		/// </typeparam>
		/// <param name="ptr">The pointer.</param>
		/// <param name="length">The length of the array that pointer points to.</param>
		/// <param name="index">The start index that you want to pick from.</param>
		/// <returns>The array of elements.</returns>
		/// <remarks>
		/// For example, the pointer is the address of the first element in an array <c>{ 0, 1, 3, 6, 10 }</c>,
		/// if parameter <paramref name="index"/> is 2, the return array will be <c>{ 3, 6, 10 }</c>. Note that
		/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
		/// 5 elements in this case.
		/// </remarks>
		public static TUnmanaged[] GetArrayFromStart<TUnmanaged>(TUnmanaged* ptr, int length, int index)
			where TUnmanaged : unmanaged
		{
			var result = new TUnmanaged[length - index];
			for (int i = index; i < length; i++)
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
		/// the method will be same as <see cref="GetArrayFromStart{TUnmanaged}(TUnmanaged*, int, int)"/>.
		/// </param>
		/// <returns>The array of elements.</returns>
		/// <remarks>
		/// For example, the pointer is the address of the first element in an array <c>{ 0, 1, 3, 6, 10 }</c>,
		/// if parameter <paramref name="index"/> is 2, the return array will be <c>{ 3, 6, 10 }</c>. Note that
		/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
		/// 5 elements in this case.
		/// </remarks>
		/// <seealso cref="GetArrayFromStart{TUnmanaged}(TUnmanaged*, int, int)"/>
		public static int[] GetArrayFromStart(int* ptr, int length, int index, bool removeTrailingZeros)
		{
			if (removeTrailingZeros)
			{
				int count = 0;
				int* p = ptr + length - 1;
				for (int i = length - 1; i >= 0; i--, p--, count++)
				{
					if (*p != 0)
					{
						break;
					}
				}

				int[] result = new int[length - count - index];
				for (int i = index, iterationLength = length - count; i < iterationLength; i++)
				{
					result[i - index] = ptr[i];
				}

				return result;
			}
			else
			{
				return GetArrayFromStart(ptr, length, index);
			}
		}
	}
}
