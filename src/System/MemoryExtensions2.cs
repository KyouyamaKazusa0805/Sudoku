namespace System;

/// <summary>
/// Extends from type <see cref="MemoryExtensions"/>.
/// </summary>
/// <seealso cref="MemoryExtensions"/>
public static class MemoryExtensions2
{
	/// <inheritdoc cref="MemoryExtensions.AsSpan{T}(T[])"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[]? array) => new(array);
}
