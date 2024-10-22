namespace System.Linq;

/// <summary>
/// Represents a type that enumerates elements of type <typeparamref name="TSource"/> in a <see cref="ReadOnlySpan{T}"/>,
/// grouped by the specified key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TSource">The type of each element.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <param name="elements">Indicates the elements.</param>
/// <param name="key">Indicates the key that can compare each element.</param>
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[TypeImpl(TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators)]
public readonly partial struct SpanGrouping<TSource, TKey>([Field] TSource[] elements, [Property, HashCodeMember, StringMember] TKey key) :
	IGroupingDataProvider<SpanGrouping<TSource, TKey>, TKey, TSource>
	where TKey : notnull
{
	/// <summary>
	/// Indicates the length of the value.
	/// </summary>
	public int Length => _elements.Length;

	[HashCodeMember]
	private unsafe nint ElementsRawPointerValue => (nint)Unsafe.AsPointer(ref _elements[0]);

	[StringMember]
	private string FirstElementString => _elements[0]!.ToString()!;

	/// <summary>
	/// Creates a <see cref="ReadOnlySpan{T}"/> instance that is aligned as <see cref="_elements"/>.
	/// </summary>
	/// <seealso cref="_elements"/>
	private ReadOnlySpan<TSource> SourceSpan
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(_elements);
	}

	/// <inheritdoc/>
	ReadOnlySpan<TSource> IGroupingDataProvider<SpanGrouping<TSource, TKey>, TKey, TSource>.Elements => SourceSpan;


	/// <summary>
	/// Gets the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public ref readonly TSource this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _elements[index];
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(SpanGrouping<TSource, TKey> other)
		=> ReferenceEquals(_elements, other._elements) && Key.Equals(other.Key);

	/// <summary>
	/// Projects elements into a new form.
	/// </summary>
	/// <typeparam name="TResult">The type of each element in result collection.</typeparam>
	/// <param name="selector">The selector method that transform the object into new one.</param>
	/// <returns>A list of <typeparamref name="TResult"/> values.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<TSource, TResult> selector)
	{
		var result = new List<TResult>(Length);
		foreach (var element in SourceSpan)
		{
			result.AddRef(selector(element));
		}
		return result.AsSpan();
	}

	/// <summary>
	/// Filters the collection, only reserving elements satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition that checks for each element.</param>
	/// <returns>A list of <typeparamref name="TSource"/> elements satisfying the condition.</returns>
	public ReadOnlySpan<TSource> Where(Func<TSource, bool> predicate)
	{
		var result = new List<TSource>(Length);
		foreach (var element in SourceSpan)
		{
			if (predicate(element))
			{
				result.AddRef(in element);
			}
		}
		return result.AsSpan();
	}

	/// <summary>
	/// Casts the current object into a <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<TSource> AsSpan() => _elements;

	/// <summary>
	/// Creates an enumerator that can enumerate each element in the source collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AnonymousSpanEnumerator<TSource> GetEnumerator() => new(SourceSpan);

	/// <inheritdoc cref="ReadOnlySpan{T}.GetPinnableReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref readonly TSource GetPinnableReference() => ref _elements[0];

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<SpanGrouping<TSource, TKey>, TSource>.Select<TResult>(Func<TSource, TResult> selector)
		=> Select(selector).ToArray();

	/// <inheritdoc/>
	IEnumerable<TSource> IWhereMethod<SpanGrouping<TSource, TKey>, TSource>.Where(Func<TSource, bool> predicate)
		=> Where(predicate).ToArray();
}
