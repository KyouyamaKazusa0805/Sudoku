namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>, especially for one-dimensional array.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Same as for-each method <see cref="Array.ForEach{T}(T[], Action{T})"/>, but iterating on references to corresponding elements.
	/// </summary>
	/// <typeparam name="T">The type of each element in this array.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="callback">The callback method to handle for each reference to each element.</param>
	/// <seealso cref="Array.ForEach{T}(T[], Action{T})"/>
	public static void ForEachRef<T>(this T[] @this, ActionRef<T> callback)
	{
		foreach (ref var element in @this.AsSpan())
		{
			callback(ref element);
		}
	}

	/// <inheritdoc cref="ForEachRef{T}(T[], ActionRef{T})"/>
	public static unsafe void ForEachRefUnsafe<T>(this T[] @this, delegate*<ref T, void> callback)
	{
		foreach (ref var element in @this.AsSpan())
		{
			callback(ref element);
		}
	}

	/// <summary>
	/// Initializes an array, using the specified method to initialize each element.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="array">The array.</param>
	/// <param name="initializer">The initializer callback method.</param>
	public static void InitializeArray<T>(this T?[] array, ArrayInitializer<T> initializer)
	{
		foreach (ref var element in array.AsSpan())
		{
			initializer(ref element);
		}
	}

	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	public static unsafe void SortUnsafe<T>(this T[] @this, delegate*<ref readonly T, ref readonly T, int> comparer)
	{
		quickSort(0, @this.Length - 1, @this, comparer);


		static void quickSort(int l, int r, T[] @this, delegate*<ref readonly T, ref readonly T, int> comparer)
		{
			if (l < r)
			{
				int i = l, j = r - 1;
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

				quickSort(l, i, @this, comparer);
				quickSort(i + 1, r, @this, comparer);
			}
		}
	}

	/// <inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/>.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReverseIterator<T> EnumerateReversely<T>(this T[] @this) => new(@this);

	/// <summary>
	/// Creates a <see cref="ArrayPairIterator{T, TFirst, TSecond}"/> instance that iterates on each element of pair elements.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <typeparam name="TFirst">The first element returned.</typeparam>
	/// <typeparam name="TSecond">The second element returned.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>An enumerable collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayPairIterator<T, TFirst, TSecond> EnumerateAsPair<T, TFirst, TSecond>(this T[] @this)
		where T : notnull where TFirst : notnull, T where TSecond : notnull, T => new(@this);

	/// <inheritdoc cref="ReadOnlySpanExtensions.RandomSelectOne{T}(ReadOnlySpan{T}, Random?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T RandomSelectOne<T>(this T[] @this, Random? random = null)
		=> ref @this[(random ?? Random.Shared).Next(0, @this.Length)];

	/// <summary>
	/// Sort the specified array by quick sort.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="comparer">The method to compare two elements.</param>
	/// <param name="startIndex">Indicates the start index.</param>
	/// <param name="endIndex">Indicates the end index.</param>
	internal static unsafe void SortUnsafe<T>(this T[] @this, delegate*<T, T, int> comparer, int startIndex, int endIndex)
	{
		quickSort(startIndex, endIndex, @this, comparer);


		static void quickSort(int l, int r, T[] @this, delegate*<T, T, int> comparer)
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

					(@this[i], @this[j]) = (@this[j], @this[i]);

					if (comparer(@this[i], @this[j]) == 0) { j--; }
				}

				quickSort(l, i, @this, comparer);
				quickSort(i + 1, r, @this, comparer);
			}
		}
	}
}
