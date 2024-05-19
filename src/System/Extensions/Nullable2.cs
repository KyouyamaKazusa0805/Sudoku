namespace System;

/// <summary>
/// Represents extension methods on generic-typed instance.
/// </summary>
public static class Nullable2
{
	/// <summary>
	/// Unwraps the value and returns its non-null representation,
	/// or throws an <see cref="ArgumentNullException"/> reporting the argument has no value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="this">The instance to be unwrapped.</param>
	/// <returns>The unwrapped value.</returns>
	/// <exception cref="ArgumentNullException">Throws when the argument doesn't have a value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unwrap<T>(this T? @this) where T : class => @this ?? throw new ArgumentNullException(nameof(@this));

	/// <inheritdoc cref="Unwrap{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Unwrap<T>(this T? @this) where T : struct => @this ?? throw new ArgumentNullException(nameof(@this));

	/// <summary>
	/// Unwraps the reference to a value and returns its non-null representation,
	/// or throws an <see cref="ArgumentNullRefException"/> reporting the reference references <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="this">The instance to be unwrapped.</param>
	/// <returns>The unwrapped value.</returns>
	/// <exception cref="ArgumentNullRefException">Throws when the argument doesn't have a value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T UnwrapRef<T>(ref readonly T? @this) where T : class
	{
		@ref.ThrowIfNullRef(in @this);
		return ref @this!;
	}

	/// <summary>
	/// Unwraps the reference to a value and returns its non-null representation,
	/// or throws an <see cref="ArgumentNullRefException"/> reporting the reference references <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="this">The instance to be unwrapped.</param>
	/// <returns>The unwrapped value.</returns>
	/// <exception cref="ArgumentNullRefException">Throws when the argument doesn't have a value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T UnwrapRef<T>(ref readonly T? @this) where T : struct
	{
		@ref.ThrowIfNullRef(in @this);
		return ref Nullable.GetValueRefOrDefaultRef(in @this);
	}
}
