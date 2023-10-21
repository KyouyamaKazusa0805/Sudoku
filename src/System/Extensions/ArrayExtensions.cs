namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>, especially for one-dimensional array.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
#pragma warning disable CS1658, CS1584
	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <remarks>
	/// If you want to use this method, please call the other method <see cref="Sort{T}(T[], delegate*{T, T, int}, int, int)"/> instead.
	/// </remarks>
	/// <seealso cref="Sort{T}(T[], delegate*{T, T, int}, int, int)"/>
#pragma warning restore CS1658, CS1584
	public static unsafe void Sort<T>(this T[] @this, delegate*<ref readonly T, ref readonly T, int> comparer)
	{
		q(0, @this.Length - 1, @this, comparer);


		static void q(Offset l, Offset r, T[] @this, delegate*<ref readonly T, ref readonly T, int> comparer)
		{
			if (l < r)
			{
				Offset i = l, j = r - 1;
				var middle = @this[(l + r) / 2];
				while (true)
				{
					while (i < r && comparer(in @this[i], in middle) < 0) { i++; }
					while (j > 0 && comparer(in @this[j], in middle) > 0) { j--; }
					if (i == j)
					{
						break;
					}

					(@this[i], @this[j]) = (@this[j], @this[i]);

					if (comparer(in @this[i], in @this[j]) == 0) { j--; }
				}

				q(l, i, @this, comparer);
				q(i + 1, r, @this, comparer);
			}
		}
	}

	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <param name="startIndex">Indicates the start index.</param>
	/// <param name="endIndex">Indicates the end index.</param>
	public static unsafe void Sort<T>(this T[] @this, delegate*<T, T, int> comparer, int startIndex, int endIndex)
	{
		q(startIndex, endIndex, @this, comparer);


		static void q(Offset l, Offset r, T[] @this, delegate*<T, T, int> comparer)
		{
			if (l < r)
			{
				Offset i = l, j = r - 1;
				var middle = @this[(l + r) / 2];
				while (true)
				{
					while (i < r && comparer(@this[i], middle) < 0) { i++; }
					while (j > 0 && comparer(@this[j], middle) > 0) { j--; }
					if (i == j)
					{
						break;
					}

					(@this[i], @this[j]) = (@this[j], @this[i]);

					if (comparer(@this[i], @this[j]) == 0) { j--; }
				}

				q(l, i, @this, comparer);
				q(i + 1, r, @this, comparer);
			}
		}
	}
}
