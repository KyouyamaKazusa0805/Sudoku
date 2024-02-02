namespace System;

/// <summary>
/// Represents a read-only collection of chunks of values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of each values.</typeparam>
/// <param name="chunks">Indicates the internal chunks.</param>
[CollectionBuilder(typeof(ReadOnlyChunksBuilder), nameof(ReadOnlyChunksBuilder.Create))]
public readonly partial struct ReadOnlyChunk<T>([PrimaryConstructorParameter(MemberKinds.Field)] params ChunkNode<T>[] chunks) : IEnumerable<T>
{
	/// <summary>
	/// Indicates the number of elements of type <see cref="ChunkNode{T}"/> stored in this collection.
	/// </summary>
	/// <seealso cref="ChunkNode{T}"/>
	public int Length => _chunks.Length;

	/// <summary>
	/// Indicates the span of values.
	/// </summary>
	public ReadOnlySpan<ChunkNode<T>> Span => _chunks.AsReadOnlySpan();


	/// <summary>
	/// Gets the value node at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>A <see cref="ChunkNode{T}"/> instance. You can use explicit cast to fetch the internal value.</returns>
	public ref readonly ChunkNode<T> this[int index] => ref _chunks[index];


	/// <inheritdoc cref="ReadOnlySpan{T}.GetPinnableReference"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly ChunkNode<T> GetPinnableReference() => ref _chunks[0];

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Expands the collection to a <see cref="ReadOnlySpan{T}"/> of <typeparamref name="T"/> instances.
	/// </summary>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> that stores the expanded <typeparamref name="T"/> elements.</returns>
	public ReadOnlySpan<T> AsSpan()
	{
		var valuesCount = 0;
		foreach (ref readonly var element in Span)
		{
			valuesCount += element.Length;
		}

		var result = new T[valuesCount];
		var i = 0;
		foreach (var element in this)
		{
			result[i++] = element;
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		foreach (var chunk in _chunks)
		{
			switch (chunk.Type)
			{
				case ChunkNodeType.Value:
				{
					Debug.Assert(chunk.IsSingleValue);
					yield return chunk.Value;
					break;
				}
				case ChunkNodeType.Array:
				{
					Debug.Assert(!chunk.IsSingleValue);
					foreach (var element in (T[])chunk.ValueRef)
					{
						yield return element;
					}
					break;
				}
				case ChunkNodeType.List:
				{
					Debug.Assert(!chunk.IsSingleValue);
					foreach (var element in (List<T>)chunk.ValueRef)
					{
						yield return element;
					}
					break;
				}
			}
		}
	}
}
