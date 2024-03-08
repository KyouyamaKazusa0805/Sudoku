namespace System.Text;

/// <summary>
/// Provides extension methods on <see cref="StringBuilder"/>.
/// </summary>
/// <seealso cref="StringBuilder"/>
public static class StringBuilderExtensions
{
	/// <summary>
	/// Remove all characters behind the character whose index is specified.
	/// </summary>
	/// <param name="this">The instance to remove characters.</param>
	/// <param name="startIndex">The start index.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder RemoveFrom(this StringBuilder @this, int startIndex) => @this.Remove(startIndex, @this.Length - startIndex);

	/// <summary>
	/// Remove all characters behind the character whose index is specified.
	/// </summary>
	/// <param name="this">The instance to remove characters.</param>
	/// <param name="startIndex">The start index.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder RemoveFrom(this StringBuilder @this, Index startIndex)
		=> @this.Remove(startIndex.GetOffset(@this.Length), startIndex.Value);

	/// <summary>
	/// Appends a list of elements of type <typeparamref name="T"/> into the <see cref="StringBuilder"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The <see cref="StringBuilder"/> instance.</param>
	/// <param name="elements">The elements to be appended.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder AppendRange<T>(this StringBuilder @this, scoped ReadOnlySpan<T> elements) where T : notnull
		=> @this.AppendRange(elements, static element => element.ToString()!);

	/// <summary>
	/// Appends a list of elements of type <typeparamref name="T"/> into the <see cref="StringBuilder"/> instance,
	/// using the specified converter to convert each element into <see cref="string"/> value.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The <see cref="StringBuilder"/> instance.</param>
	/// <param name="elements">The elements to be appended.</param>
	/// <param name="stringConverter">The converter method.</param>
	/// <param name="builderAppender">
	/// The appender method for the builder instance, telling the handler which appending operation will be handled.
	/// By default, the method is equivalent to lambda:
	/// <code>
	/// <see langword="static"/> (<see cref="StringBuilder"/> sb, <typeparamref name="T"/> v) => sb.Append(v)
	/// </code>
	/// </param>
	/// <returns>The reference of the current instance.</returns>
	public static StringBuilder AppendRange<T>(
		this StringBuilder @this,
		scoped ReadOnlySpan<T> elements,
		Func<T, string> stringConverter,
		StringBuilderAppender<string>? builderAppender = null
	) where T : notnull
	{
		builderAppender ??= static (sb, v) => sb.Append(v);
		foreach (var element in elements)
		{
			builderAppender(@this, stringConverter(element));
		}
		return @this;
	}

	/// <summary>
	/// Appends a list of elements of type <typeparamref name="T"/>, specified as the reference to the first element in the collection,
	/// into the <see cref="StringBuilder"/> instance, using the specified converter to convert each element into <see cref="string"/> value.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The <see cref="StringBuilder"/> instance.</param>
	/// <param name="collectionRef">The reference to the first element in the collection.</param>
	/// <param name="collectionLength">The length of the collection.</param>
	/// <param name="stringConverter">The converter method.</param>
	/// <param name="separator">The separator character.</param>
	/// <returns>The reference of the current instance.</returns>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="collectionRef"/> is <see langword="ref null"/>.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="stringConverter"/> is <see langword="null"/>.
	/// </exception>
	public static unsafe StringBuilder AppendRangeWithSeparator<T>(
		this StringBuilder @this,
		scoped ref readonly T collectionRef,
		int collectionLength,
		delegate*<ref readonly T, string> stringConverter,
		string separator
	) where T : notnull
	{
		Ref.ThrowIfNullRef(in collectionRef);
		ArgumentNullException.ThrowIfNull(stringConverter);

		for (var i = 0; i < collectionLength; i++)
		{
			@this
				.Append(stringConverter(in Unsafe.Add(ref Ref.AsMutableRef(in collectionRef), i)))
				.Append(separator);
		}

		return @this.RemoveFrom(^separator.Length);
	}
}
