namespace System;

/// <summary>
/// Creates <see cref="IEqualityComparer{T}"/> instances.
/// </summary>
/// <seealso cref="IEqualityComparer{T}"/>
public static unsafe class EqualityComparing
{
	/// <inheritdoc cref="Create{T}(FuncRefReadOnly{T, T, bool}, FuncRefReadOnly{T, int})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> CreateByEqualityOperator<T>() where T : IEqualityOperators<T, T, bool>
		=> Create<T>(static (a, b) => a == b, static v => v.GetHashCode());

	/// <inheritdoc cref="Create{T}(FuncRefReadOnly{T, T, bool}, FuncRefReadOnly{T, int})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
		=> Create((ref readonly T left, ref readonly T right) => equals(left, right), (ref readonly T obj) => getHashCode(obj));

	/// <summary>
	/// Creates an <see cref="IEqualityComparer{T}"/> instance via specified methods.
	/// </summary>
	/// <typeparam name="T">The type of the value to be compared.</typeparam>
	/// <param name="equals">The equals method handler.</param>
	/// <param name="getHashCode">The get hash code method handler.</param>
	/// <returns>An <see cref="IEqualityComparer{T}"/> value serving as comparing equality rules.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> Create<T>(FuncRefReadOnly<T, T, bool> equals, FuncRefReadOnly<T, int> getHashCode)
		=> new SpecializedEqualityComparer<T>(equals, getHashCode);

	/// <inheritdoc cref="Create{T}(FuncRefReadOnly{T, T, bool}, FuncRefReadOnly{T, int})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEqualityComparer<T> CreateUnsafe<T>(delegate*<ref readonly T, ref readonly T, bool> equals, delegate*<ref readonly T, int> getHashCode)
		=> Create(
			(ref readonly T left, ref readonly T right) => equals(in left, in right),
			(ref readonly T obj) => getHashCode(in obj)
		);
}
