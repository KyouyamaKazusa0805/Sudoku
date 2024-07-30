namespace System;

/// <summary>
/// Represents with extension methods for value tuple type set.
/// </summary>
public static class ValueTupleExtensions
{
	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this (T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this scoped ref readonly (T, T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3, T4}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this scoped ref readonly (T, T, T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this scoped ref readonly (T, T, T, T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this scoped ref readonly (T, T, T, T, T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueTupleEnumerator<T> GetEnumerator<T>(this scoped ref readonly (T, T, T, T, T, T, T) @this) => new(@this);

	/// <summary>
	/// Gets an <see cref="ValueTupleEnumerator{T}"/> instance that can iterate for a pair of values
	/// via a value tuple <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/> of a uniform type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The uniform type that two instances defined in pair are.</typeparam>
	/// <typeparam name="TRest">The type that encapsulates a list of rest elements.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ComplexValueTupleEnumerator<T, TRest> GetEnumerator<T, TRest>(this scoped ref readonly ValueTuple<T, T, T, T, T, T, T, TRest> @this)
		where TRest : struct => new(@this);
}
