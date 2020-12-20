namespace System.Extensions
{
	/// <summary>
	/// Provides methods on pointers.
	/// </summary>
	/// <remarks>
	/// Different with other types, pointers can't be as <see langword="this"/> parameter.
	/// </remarks>
	[CLSCompliant(false)]
	public static unsafe class Pointer
	{
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
				for (int i = index; i < length - count; i++)
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
