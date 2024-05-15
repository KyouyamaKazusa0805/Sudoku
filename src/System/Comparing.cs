namespace System;

/// <summary>
/// Create <see cref="IComparer{T}"/> instances.
/// </summary>
/// <seealso cref="IComparer{T}"/>
public static unsafe class Comparing
{
	/// <inheritdoc cref="Create{T}(FuncRefReadOnly{T, T, int})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IComparer<T> Create<T>(Comparison<T> compare)
		=> Create((ref readonly T left, ref readonly T right) => compare(left, right));

	/// <summary>
	/// Creates an <see cref="IComparer{T}"/> instance via specified method.
	/// </summary>
	/// <typeparam name="T">The type of the value to be compared.</typeparam>
	/// <param name="compare">The compare method handler.</param>
	/// <returns>An <see cref="IComparer{T}"/> value serving as comparing rules.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IComparer<T> Create<T>(FuncRefReadOnly<T, T, int> compare) => new SpecializedComparer<T>(compare);

	/// <inheritdoc cref="Create{T}(FuncRefReadOnly{T, T, int})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IComparer<T> CreateUnsafe<T>(delegate*<ref readonly T, ref readonly T, int> compare) => new SpecializedComparer<T>(compare);
}
