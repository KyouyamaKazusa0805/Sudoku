namespace System;

/// <summary>
/// Represents a read-only collection of trunks of values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of each values.</typeparam>
/// <param name="trunks">Indicates the internal trunks.</param>
[CollectionBuilder(typeof(ReadOnlyTrunksBuilder), nameof(ReadOnlyTrunksBuilder.Create))]
public readonly partial struct ReadOnlyTrunks<T>([RecordParameter(DataMemberKinds.Field)] params TrunkNode<T>[] trunks) : IEnumerable<T>
{
	/// <summary>
	/// Indicates the number of elements of type <see cref="TrunkNode{T}"/> stored in this collection.
	/// </summary>
	/// <seealso cref="TrunkNode{T}"/>
	public int Length => _trunks.Length;


	/// <summary>
	/// Gets the value node at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>A <see cref="TrunkNode{T}"/> instance. You can use explicit cast to fetch the internal value.</returns>
	public ref readonly TrunkNode<T> this[int index] => ref _trunks[index];


	/// <inheritdoc cref="ReadOnlySpan{T}.GetPinnableReference"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly TrunkNode<T> GetPinnableReference() => ref _trunks[0];

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
		foreach (ref readonly var element in _trunks.AsReadOnlySpan())
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
		foreach (var element in _trunks)
		{
			switch (element.Type)
			{
				case TrunkNodeType.Value:
				{
					yield return element.Value!;
					break;
				}
				case TrunkNodeType.Array:
				{
					foreach (var e in (T[])element.ValueRef!)
					{
						yield return e;
					}
					break;
				}
				case TrunkNodeType.List:
				{
					foreach (var e in (List<T>)element.ValueRef!)
					{
						yield return e;
					}
					break;
				}
			}
		}
	}
}
