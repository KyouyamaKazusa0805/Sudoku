namespace System;

/// <summary>
/// Represents a list of methods that can check for the concept "References" defined in C#.
/// </summary>
public static class Ref
{
	/// <summary>
	/// Swaps for two elements.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be swapped.</param>
	/// <param name="right">The second element to be swapped.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(scoped ref T left, scoped ref T right)
	{
		if (!MemoryLocationAreSame(in left, in right))
		{
			var temp = left;
			left = right;
			right = temp;
		}
	}

	/// <summary>
	/// Simply invokes the method <see cref="As{TFrom, TTo}(ref TFrom)"/>, but with target generic type being fixed type <see cref="byte"/>.
	/// </summary>
	/// <typeparam name="T">The base type that is converted from.</typeparam>
	/// <param name="ref">
	/// The reference to the value. Generally speaking the value should be a <see langword="ref readonly"/> parameter, but C# disallows it,
	/// using <see langword="ref readonly"/> as a combined parameter modifier.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref byte AsByteRef<T>(ref T @ref) => ref As<T, byte>(ref @ref);

	/// <inheritdoc cref="AsByteRef{T}(ref T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly byte AsReadOnlyByteRef<T>(ref readonly T @ref) => ref As<T, byte>(ref AsRef(in @ref));

	/// <summary>
	/// Determines whether the current reference points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">The reference to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullReference<T>(scoped ref readonly T reference) => IsNullRef(in reference);

	/// <summary>
	/// Check whether two references point to a same memory location.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be checked.</param>
	/// <param name="right">The second element to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool MemoryLocationAreSame<T>(scoped ref readonly T left, scoped ref readonly T right) => AreSame(in left, in right);

	/// <summary>
	/// Try to fetch the reference to the first character to a <see cref="Utf8String"/>, with read-only state.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The reference to the first character, with read-only state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref Utf8Char GetRefFirstChar(Utf8String @this) => ref @this[0];

	/// <summary>
	/// Try to fetch the reference to the first character to a <see cref="string"/>, with read-only state.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The reference to the first character, with read-only state.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly char GetRefFirstChar(string @this) => ref @this.AsSpan()[0];

	/// <summary>
	/// Returns a reference that points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the element the reference points to if this reference were not <see langword="null"/>.
	/// </typeparam>
	/// <returns>A read-only reference that points to <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T MakeNullReference<T>() => ref NullRef<T>();

	/// <summary>
	/// Re-interpret the read-only reference to non-read-only reference.
	/// </summary>
	/// <typeparam name="T">The type of the referenced item.</typeparam>
	/// <param name="ref">The read-only reference.</param>
	/// <returns>The non-read-only reference.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T AsMutableRef<T>(ref readonly T @ref) => ref AsRef(in @ref);

	/// <summary>
	/// Get the new array from the reference to the block memory start position, with the specified start index.
	/// </summary>
	/// <typeparam name="T">The type of the pointed element.</typeparam>
	/// <param name="memorySpan">The reference to the block memory start position.</param>
	/// <param name="start">The start index that you want to pick from.</param>
	/// <param name="count">The length of the array that the reference points to.</param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="memorySpan"/> is <see langword="null"/>.
	/// </exception>
	public static ReadOnlySpan<T> Slice<T>(scoped ref readonly T memorySpan, int start, int count)
	{
		ThrowIfNullRef(in memorySpan);

		var result = new T[count - start];
		for (var i = start; i < count; i++)
		{
			result[i - start] = Add(ref AsRef(in memorySpan), i);
		}

		return result;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullRefException"/> if the argument points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">
	/// <para>The reference to the target element, or maybe a <see langword="null"/> reference.</para>
	/// <para><i>
	/// Please note that the argument requires a <see langword="ref"/> modifier, but it does not modify the referenced value
	/// of the argument. It is nearly equal to <see langword="in"/> modifier.
	/// However, the method will invoke <see cref="IsNullRef{T}(ref readonly T)"/>,
	/// where the only argument is passed by <see langword="ref"/>.
	/// Therefore, here the current method argument requires a modifier <see langword="ref"/> instead of <see langword="in"/>.
	/// </i></para>
	/// </param>
	/// <param name="paramName">
	/// <para>The parameter name.</para>
	/// <include file="../../global-doc-comments.xml" path="g/csharp10/feature[@name='caller-argument-expression']" />
	/// </param>
	/// <exception cref="ArgumentNullRefException">Throws if the argument is a <see langword="null"/> reference.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<T>(
		scoped ref readonly T reference,
		[ConstantExpected, CallerArgumentExpression(nameof(reference))] string? paramName = null
	)
	{
		if (IsNullReference(in reference))
		{
			throw new ArgumentNullRefException(nameof(reference));
		}
	}
}
