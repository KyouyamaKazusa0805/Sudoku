namespace System.Algorithm;

/// <summary>
/// Provides all algorithm processing methods.
/// </summary>
public static unsafe class Sorting
{
	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <param name="startIndex">Indicates the start index.</param>
	/// <param name="endIndex">Indicates the end index.</param>
	public static void Sort<T>(this T[] @this, delegate*<T, T, int> comparer, int startIndex, int endIndex)
	{
		q(startIndex, endIndex, @this, comparer);


		static void q(int l, int r, T[] @this, delegate*<T, T, int> comparer)
		{
			if (l < r)
			{
				int i = l, j = r - 1;
				var middle = @this[(l + r) / 2];
				while (true)
				{
					while (i < r && comparer(@this[i], middle) < 0) { i++; }
					while (j > 0 && comparer(@this[j], middle) > 0) { j--; }
					if (i == j)
					{
						break;
					}

					var temp = @this[i];
					@this[i] = @this[j];
					@this[j] = temp;

					if (comparer(@this[i], @this[j]) == 0) { j--; }
				}

				q(l, i, @this, comparer);
				q(i + 1, r, @this, comparer);
			}
		}
	}


#pragma warning disable CS1658, CS1584
	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <remarks>
	/// If you want to use this method, please call the other method
	/// <see cref="Sort{T}(T[], delegate*{T, T, int}, int, int)"/> instead.
	/// </remarks>
	/// <seealso cref="Sort{T}(T[], delegate*{T, T, int}, int, int)"/>
	public static void Sort<T>(this T[] @this, delegate*<in T, in T, int> comparer)
#pragma warning restore CS1658, CS1584
	{
		q(0, @this.Length - 1, @this, comparer);


		static void q(int l, int r, T[] @this, delegate*<in T, in T, int> comparer)
		{
			if (l < r)
			{
				int i = l, j = r - 1;
				var middle = @this[(l + r) / 2];
				while (true)
				{
					while (i < r && comparer(@this[i], middle) < 0) { i++; }
					while (j > 0 && comparer(@this[j], middle) > 0) { j--; }
					if (i == j)
					{
						break;
					}

					var temp = @this[i];
					@this[i] = @this[j];
					@this[j] = temp;

					if (comparer(@this[i], @this[j]) == 0) { j--; }
				}

				q(l, i, @this, comparer);
				q(i + 1, r, @this, comparer);
			}
		}
	}
}
