using System;
using System.Numerics;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides the algorithms about sudoku solving (using basic implementation of sudoku data structures).
	/// </summary>
	public static class Algorithms
	{
		/// <summary>
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <returns>The result list.</returns>
		public static short[] GetMaskSubsets(short value)
		{
			short[][] maskSubsets = new short[9][];
			for (int size = 1; size <= 9; size++)
			{
				maskSubsets[size - 1] = GetMaskSubsets(value, size);
			}

			short[] result = new short[9];
			for (int i = 0; i < 9; i++)
			{
				short[] target = maskSubsets[i];
				result[i] = CreateBitsInt16(target);
			}

			return result;
		}

		/// <summary>
		/// Get all mask combinations.
		/// </summary>
		/// <param name="value">The mask.</param>
		/// <param name="size">The size.</param>
		/// <returns>The result list.</returns>
		public static short[] GetMaskSubsets(short value, int size)
		{
			var listToIterate = value.GetAllSets().GetSubsets(size);
			short[] result = new short[listToIterate.Count];
			int index = 0;
			foreach (int[] target in listToIterate)
			{
				result[index++] = CreateBitsInt16(target);
			}

			return result;
		}

		/// <summary>
		/// Create a <see cref="short"/> value, whose set bits are specified in the parameter
		/// <paramref name="values"/>.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>The mask result.</returns>
		/// <remarks>
		/// For example, if the <paramref name="values"/> are <c>{ 3, 6 }</c>, the return value
		/// will be <c><![CDATA[1 << 3 | 1 << 6]]></c>.
		/// </remarks>
		private static short CreateBitsInt16(int[] values)
		{
			short result = 0;
			foreach (int value in values) result |= (short)(1 << value);

			return result;
		}

		/// <inheritdoc cref="CreateBitsInt16(int[])"/>
		private static short CreateBitsInt16(short[] values)
		{
			short result = 0;
			foreach (int value in values) result |= (short)(1 << value);

			return result;
		}
	}
}
