namespace System;

/// <summary>
/// Represents a type that holds the <see langword="null"/>-checking operation.
/// </summary>
public static class Nullability
{
	/// <summary>
	/// Checks whether the specified pointer value is not <see langword="null"/>. Otherwise,
	/// an <see cref="ArgumentNullException"/> will be thrown.
	/// </summary>
	/// <param name="pointer">The pointer value.</param>
	/// <param name="argName">The argument name.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the <paramref name="argName"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void ThrowIfNull(void* pointer, [CallerArgumentExpression("pointer")] string? argName = null)
	{
		if (pointer == null)
		{
			throw new ArgumentNullException(argName);
		}
	}

	/// <summary>
	/// Checks whether the specified reference value is not <see langword="null"/>. Otherwise,
	/// an <see cref="ArgumentNullException"/> will be thrown.
	/// </summary>
	/// <typeparam name="TStruct">The type of the real instance.</typeparam>
	/// <param name="ref">The pointer value.</param>
	/// <param name="argName">The argument name.</param>
	/// <exception cref="ArgumentNullException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<TStruct>(in TStruct @ref, [CallerArgumentExpression("ref")] string? argName = null)
		where TStruct : struct
	{
		if (Unsafe.IsNullRef(ref Unsafe.AsRef(@ref)))
		{
			throw new ArgumentNullException(argName);
		}
	}
}
