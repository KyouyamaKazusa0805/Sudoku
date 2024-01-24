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
				var middle = @this[(l + r) >> 1];
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
}
