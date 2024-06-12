namespace System;

/// <summary>
/// Represents a list of methods that can check for the concept "References" defined in C#.
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public static class @ref
{
	/// <summary>
	/// Swaps for two elements.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be swapped.</param>
	/// <param name="right">The second element to be swapped.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(scoped ref T left, scoped ref T right)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
		if (!AreSameRef(in left, in right))
		{
			var temp = left;
			left = right;
			right = temp;
		}
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
	/// However, the method will invoke <see cref="Unsafe.IsNullRef{T}(ref readonly T)"/>,
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
		[CallerArgumentExpression(nameof(reference))] string paramName = null!
	)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
		if (IsNullRef(in reference))
		{
			throw new ArgumentNullRefException(nameof(reference));
		}
	}

	/// <summary>
	/// Simply invokes the method <see cref="Unsafe.As{TFrom, TTo}(ref TFrom)"/>, but with target generic type being fixed type <see cref="byte"/>.
	/// </summary>
	/// <typeparam name="T">The base type that is converted from.</typeparam>
	/// <param name="ref">
	/// The reference to the value. Generally speaking the value should be a <see langword="ref readonly"/> parameter, but C# disallows it,
	/// using <see langword="ref readonly"/> as a combined parameter modifier.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref byte ByteRef<T>(ref T @ref) => ref Unsafe.As<T, byte>(ref @ref);

	/// <inheritdoc cref="ByteRef{T}(ref T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly byte ReadOnlyByteRef<T>(scoped ref readonly T @ref)
		=> ref Unsafe.As<T, byte>(ref AsMutableRef(in @ref));

	/// <summary>
	/// Determines whether the current reference points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">The reference to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullRef<T>(scoped ref readonly T reference)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
#if NET9_0_OR_GREATER
		unsafe
		{
			return ToPointer(in reference) == null;
		}
#else
		return Unsafe.IsNullRef(in reference);
#endif
	}

	/// <summary>
	/// Check whether two references point to a same memory location.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be checked.</param>
	/// <param name="right">The second element to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AreSameRef<T>(scoped ref readonly T left, scoped ref readonly T right)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
#if NET9_0_OR_GREATER
		unsafe
		{
			return ToPointer(in left) == ToPointer(in right);
		}
#else
		Unsafe.AreSame(in left, in right);
#endif
	}

	/// <summary>
	/// Re-interpret the read-only reference to non-read-only reference.
	/// </summary>
	/// <typeparam name="T">The type of the referenced item.</typeparam>
	/// <param name="ref">The read-only reference.</param>
	/// <returns>The non-read-only reference.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T AsMutableRef<T>(scoped ref readonly T @ref)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
#if NET9_0_OR_GREATER
	{
		unsafe
		{
			return ref *ToPointer(in @ref);
		}
	}
#else
		=> ref Unsafe.AsRef(in @ref);
#endif

	/// <summary>
	/// Advances the pointer to an element after the specified number of block memory elements.
	/// </summary>
	/// <typeparam name="T">The type of the element in block memory.</typeparam>
	/// <param name="ref">The reference to be advanced.</param>
	/// <param name="length">The length that the pointer moves.</param>
	/// <returns>The target reference to the specified element.</returns>
	/// <remarks>
	/// Pass negative value into parameter <paramref name="length"/> if you want to move previously,
	/// which is equivalent to method call <see cref="Unsafe.Subtract{T}(ref T, int)"/>
	/// </remarks>
	/// <seealso cref="Unsafe.Subtract{T}(ref T, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T Add<T>(ref T @ref, int length)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
#if NET9_0_OR_GREATER
	{
		unsafe
		{
			return ref ToPointer(in @ref)[length];
		}
	}
#else
		=> ref Unsafe.Add(ref @ref, length);
#endif

	/// <summary>
	/// Converts the managed pointer into unmanaged one,
	/// meaning it converts <see langword="ref readonly"/> <typeparamref name="T"/> to <typeparamref name="T"/>*.
	/// </summary>
	/// <typeparam name="T">The type of the value that pointer points to.</typeparam>
	/// <param name="ref">The reference to be converted.</param>
	/// <returns>The unmanaged pointer as the result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe T* ToPointer<T>(ref readonly T @ref)
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
	{
#if NET9_0_OR_GREATER
		unsafe
		{
			fixed (T* pRef = &@ref)
			{
				return pRef;
			}
		}
#else
		return (T*)Unsafe.AsPointer(ref AsMutableRef(in @ref));
#endif
	}

	/// <summary>
	/// Returns a reference that points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the element the reference points to if this reference were not <see langword="null"/>.
	/// </typeparam>
	/// <returns>A read-only reference that points to <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T NullRef<T>()
#if NET9_0_OR_GREATER
		where T : allows ref struct
#endif
#if NET9_0_OR_GREATER
	{
		unsafe
		{
			return ref *(T*)0;
		}
	}
#else
		=> ref Unsafe.NullRef<T>();
#endif

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
			result[i - start] = Add(ref AsMutableRef(in memorySpan), i);
		}
		return result;
	}
}
