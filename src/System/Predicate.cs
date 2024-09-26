namespace System;

/// <summary>
/// Provides a type that supports predicate combinations.
/// </summary>
public static class Predicate
{
	/// <summary>
	/// Negates a predicate.
	/// </summary>
	/// <typeparam name="T">The type of the instance to be negated.</typeparam>
	/// <param name="predicate">The predicate.</param>
	/// <returns>A new predicate instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Predicate<T> Not<T>(this Predicate<T> predicate) where T : allows ref struct => e => !predicate(e);

	/// <inheritdoc cref="Not{T}(Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, bool> Not<T>(this Func<T, bool> predicate) where T : allows ref struct => e => !predicate(e);

	/// <inheritdoc cref="Not{T}(Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, bool> Not<T, TResult>(this Func<T, TResult> predicate)
		where T : allows ref struct
		where TResult : ILogicalOperators<TResult>, allows ref struct
		=> e => !predicate(e);

	/// <summary>
	/// Make logical and between two <typeparamref name="T"/> instances
	/// after <paramref name="predicate1"/> and <paramref name="predicate2"/> executed.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="predicate1">The first predicate to be used.</param>
	/// <param name="predicate2">The second predicate to be used.</param>
	/// <returns>A new predicate instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Predicate<T> And<T>(this Predicate<T> predicate1, Predicate<T> predicate2) where T : allows ref struct
		=> e => predicate1(e) && predicate2(e);

	/// <inheritdoc cref="And{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, bool> And<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2) where T : allows ref struct
		=> e => predicate1(e) && predicate2(e);

	/// <inheritdoc cref="And{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, TResult> And<T, TResult>(this Func<T, TResult> predicate1, Func<T, TResult> predicate2)
		where T : allows ref struct
		where TResult : ILogicalOperators<TResult>, allows ref struct
		=> e => predicate1(e) && predicate2(e);

	/// <summary>
	/// Make logical or between two <typeparamref name="T"/> instances
	/// after <paramref name="predicate1"/> and <paramref name="predicate2"/> executed.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="predicate1">The first predicate to be used.</param>
	/// <param name="predicate2">The second predicate to be used.</param>
	/// <returns>A new predicate instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Predicate<T> Or<T>(this Predicate<T> predicate1, Predicate<T> predicate2) where T : allows ref struct
		=> e => predicate1(e) || predicate2(e);

	/// <inheritdoc cref="Or{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, bool> Or<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2) where T : allows ref struct
		=> e => predicate1(e) || predicate2(e);

	/// <inheritdoc cref="Or{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, TResult> Or<T, TResult>(this Func<T, TResult> predicate1, Func<T, TResult> predicate2)
		where T : allows ref struct
		where TResult : ILogicalOperators<TResult>, allows ref struct
		=> e => predicate1(e) || predicate2(e);

	/// <summary>
	/// Make logical or bitwise exclusive or between two <typeparamref name="T"/> instances
	/// after <paramref name="predicate1"/> and <paramref name="predicate2"/> executed.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="predicate1">The first predicate to be used.</param>
	/// <param name="predicate2">The second predicate to be used.</param>
	/// <returns>A new predicate instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Predicate<T> Xor<T>(this Predicate<T> predicate1, Predicate<T> predicate2) where T : allows ref struct
		=> e => predicate1(e) ^ predicate2(e);

	/// <inheritdoc cref="Xor{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, bool> Xor<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2) where T : allows ref struct
		=> e => predicate1(e) ^ predicate2(e);

	/// <inheritdoc cref="Xor{T}(Predicate{T}, Predicate{T})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, TResult> Xor<T, TResult>(this Func<T, TResult> predicate1, Func<T, TResult> predicate2)
		where T : allows ref struct
		where TResult : ILogicalOperators<TResult>, allows ref struct
		=> e => predicate1(e) ^ predicate2(e);
}
