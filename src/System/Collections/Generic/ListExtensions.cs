namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Adds an object to the end of the <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	/// <param name="this">The current instance.</param>
	/// <param name="item">The object to be added to the end of the <see cref="List{T}"/>.</param>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='9' and @preview-value='4']/feature[@name='unsafe-accessor']"/>
	/// <para>
	/// This method passes with a reference to an object, which is unnecessary to be called by a reference-typed object,
	/// or a value-typed object whose memory size is less than a pointer. <b>Always measure the necessity of the usage.</b>
	/// </para>
	/// </remarks>
	/// <seealso cref="UnsafeAccessorAttribute"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef<T>(this List<T> @this, ref readonly T item)
	{
		Entry<T>.GetVersion(@this)++;
		var array = Entry<T>.GetItems(@this).AsSpan();
		var size = Entry<T>.GetSize(@this);
		if ((uint)size < (uint)array.Length)
		{
			Entry<T>.GetSize(@this)++;
			array[size] = item;
		}
		else
		{
			@this.AddWithResize(in item);
		}
	}

	/// <summary>
	/// Adds the elements of the specified <see cref="ReadOnlySpan{T}"/> to the end of the <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	/// <param name="this">The current instance.</param>
	/// <param name="items">The collection whose elements should be added to the end of the <see cref="List{T}"/>.</param>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='9' and @preview-value='4']/feature[@name='unsafe-accessor']"/>
	/// <para>
	/// This method passes with a <see cref="ReadOnlySpan{T}"/> instead of <see cref="IEnumerable{T}"/>,
	/// allowing iterating on read-only references of collection elements, which is unnecessary to be called
	/// by a reference-typed object, or a value-typed object whose memory size is less than a pointer.
	/// <b>Always measure the necessity of the usage.</b>
	/// </para>
	/// </remarks>
	/// <seealso cref="UnsafeAccessorAttribute"/>
	/// <seealso cref="ReadOnlySpan{T}"/>
	/// <seealso cref="IEnumerable{T}"/>
	public static void AddRangeRef<T>(this List<T> @this, params ReadOnlySpan<T> items)
	{
		foreach (ref readonly var item in items)
		{
			@this.AddRef(in item);
		}
	}

	/// <inheritdoc cref="List{T}.RemoveAt(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAt<T>(this List<T> @this, Index index) => @this.RemoveAt(index.GetOffset(@this.Count));

	/// <summary>
	/// Determines whether two sequences are equal by comparing the elements by using <see cref="IEquatable{T}.Equals(T)"/> for their type.
	/// </summary>
	/// <typeparam name="TEquatable">The type of each element.</typeparam>
	/// <param name="this">A <see cref="List{T}"/> to compare to <paramref name="other"/>.</param>
	/// <param name="other">A <see cref="List{T}"/> to compare to <paramref name="this"/>.</param>
	/// <returns>
	/// <see langword="true"/> if the two source sequences are of equal length and their corresponding elements are equal according
	/// to <see cref="IEquatable{T}.Equals(T)"/> for their type; otherwise, <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool SequenceEqual<TEquatable>(this List<TEquatable> @this, List<TEquatable> other)
		where TEquatable : IEquatable<TEquatable>
		=> @this.AsSpan().SequenceEqual(other.AsSpan());

	/// <inheritdoc cref="Enumerable.Sum(IEnumerable{int})"/>
	public static T Sum<T>(this List<T> @this) where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>
	{
		var result = T.AdditiveIdentity;
		foreach (ref readonly var element in @this.AsSpan())
		{
			result += element;
		}
		return result;
	}

	/// <summary>
	/// Gets a <see cref="ReadOnlySpan{T}"/> view over the data in a list.
	/// Items should not be added or removed from the <see cref="List{T}"/> while the <see cref="ReadOnlySpan{T}"/> is in use.
	/// </summary>
	/// <param name="this">The instance to be transformed.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance over the <see cref="List{T}"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(1)]
	public static ReadOnlySpan<T> AsSpan<T>(this List<T> @this) => CollectionsMarshal.AsSpan(@this);

	/// <summary>
	/// Try to create a <see cref="ReadOnlyMemory{T}"/> with values from the current <see cref="List{T}"/> object,
	/// without any copy operation.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list to create.</param>
	/// <returns>The created <see cref="ReadOnlyMemory{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(1)]
	public static ReadOnlyMemory<T> AsMemory<T>(this List<T> @this) => new(Entry<T>.GetItems(@this), 0, @this.Count);

	/// <summary>
	/// Add an item and resize the <see cref="List{T}"/> of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the target value to be added.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="item">An instance of type <typeparamref name="T"/> to be added.</param>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// </remarks>
	private static void AddWithResize<T>(this List<T> @this, ref readonly T item)
	{
		Debug.Assert(Entry<T>.GetSize(@this) == Entry<T>.GetItems(@this).Length);
		var size = Entry<T>.GetSize(@this);
		@this.Capacity = @this.GetNewCapacity(size + 1);
		Entry<T>.GetSize(@this) = size + 1;
		Entry<T>.GetItems(@this)[size] = item;
	}

	/// <summary>
	/// Try to get a new capacity value by the desired capacity to be set.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="capacity">The desired capacity to be set.</param>
	/// <returns>The result value to be set.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetNewCapacity<T>(this List<T> @this, int capacity)
	{
		Debug.Assert(Entry<T>.GetItems(@this).Length < capacity);
		return Entry<T>.GetItems(@this).Length == 0 ? 4 : Entry<T>.GetItems(@this).Length << 1;
	}
}

/// <summary>
/// Represents an entry to call internal fields on <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of each element in <see cref="List{T}"/>.</typeparam>
/// <seealso cref="List{T}"/>
file sealed class Entry<T>
{
	/// <summary>
	/// Try to fetch the internal field <c>_size</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
	/// </remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.List_Size)]
	public static extern ref int GetSize(List<T> @this);

	/// <summary>
	/// Try to fetch the internal field <c>_version</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
	/// </remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.List_Version)]
	public static extern ref int GetVersion(List<T> @this);

	/// <summary>
	/// Try to fetch the internal reference to the first <typeparamref name="T"/> in a <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list of <typeparamref name="T"/> instances.</param>
	/// <returns>The reference to the first <typeparamref name="T"/>.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
	/// </remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.List_Items)]
	public static extern ref T[] GetItems(List<T> @this);
}
