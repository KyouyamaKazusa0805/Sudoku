namespace System;

/// <summary>
/// Represents a list of methods that creates <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/> instances.
/// </summary>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="IComparer{T}"/>
public static class ValueComparison
{
	/// <inheritdoc cref="Create{T}(EqualsHandler{T}, GetHashCodeHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> CreateByEqualityOperator<T>() where T : IEqualityOperators<T, T, bool>
		=> Create<T>(static (a, b) => a == b, static v => v.GetHashCode());

	/// <inheritdoc cref="Create{T}(EqualsHandler{T}, GetHashCodeHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, GetHashCodeFunc<T> getHashCode)
		=> Create(
			(scoped ref readonly T left, scoped ref readonly T right) => equals(left, right),
			([DisallowNull] scoped ref readonly T obj) => getHashCode(obj)
		);

	/// <summary>
	/// Creates an <see cref="IEqualityComparer{T}"/> instance via specified methods.
	/// </summary>
	/// <typeparam name="T">The type of the value to be compared.</typeparam>
	/// <param name="equals">The equals method handler.</param>
	/// <param name="getHashCode">The get hash code method handler.</param>
	/// <returns>An <see cref="IEqualityComparer{T}"/> value serving as comparing equality rules.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> Create<T>(EqualsHandler<T> equals, GetHashCodeHandler<T> getHashCode)
		=> new SpecializedEqualityComparer<T>(equals, getHashCode);

	/// <inheritdoc cref="Create{T}(CompareHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IComparer<T> Create<T>(Comparison<T> compare)
		=> Create((scoped ref readonly T left, scoped ref readonly T right) => compare(left, right));

	/// <summary>
	/// Creates an <see cref="IComparer{T}"/> instance via specified method.
	/// </summary>
	/// <typeparam name="T">The type of the value to be compared.</typeparam>
	/// <param name="compare">The compare method handler.</param>
	/// <returns>An <see cref="IComparer{T}"/> value serving as comparing rules.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IComparer<T> Create<T>(CompareHandler<T> compare) => new SpecializedComparer<T>(compare);

	/// <inheritdoc cref="Create{T}(EqualsHandler{T}, GetHashCodeHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe IEqualityComparer<T> CreateUnsafe<T>(
		delegate*<ref readonly T, ref readonly T, bool> equals,
		delegate*<ref readonly T, int> getHashCode
	) => new SpecializedEqualityComparer<T>(equals, getHashCode);

	/// <inheritdoc cref="Create{T}(EqualsHandler{T}, GetHashCodeHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe IEqualityComparer<T> CreateUnsafe<T>(delegate*<T*, T*, bool> equals, delegate*<T*, int> getHashCode)
		=> Create(
			(scoped ref readonly T left, scoped ref readonly T right) => { fixed (T* l = &left, r = &right) { return equals(l, r); } },
			([DisallowNull] scoped ref readonly T obj) => { fixed (T* v = &obj) { return getHashCode(v); } }
		);

	/// <inheritdoc cref="Create{T}(CompareHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe IComparer<T> CreateUnsafe<T>(delegate*<ref readonly T, ref readonly T, int> compare)
		=> new SpecializedComparer<T>(compare);

	/// <inheritdoc cref="Create{T}(CompareHandler{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe IComparer<T> CreateUnsafe<T>(delegate*<T*, T*, int> compare)
		=> Create((scoped ref readonly T left, scoped ref readonly T right) => { fixed (T* l = &left, r = &right) { return compare(l, r); } });
}
