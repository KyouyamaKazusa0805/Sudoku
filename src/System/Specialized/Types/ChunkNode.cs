namespace System;

/// <summary>
/// Represents a chunk node. The type will be used by <see cref="ReadOnlyChunk{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of each values.</typeparam>
/// <seealso cref="ReadOnlyChunk{T}"/>
[StructLayout(LayoutKind.Explicit)]
[CollectionBuilder(typeof(ChunkNodeBuilder), nameof(ChunkNodeBuilder.Create))]
[LargeStructure]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
public readonly unsafe partial struct ChunkNode<T> : IEnumerable<T>
{
	/// <summary>
	/// Represents an empty instance.
	/// </summary>
	public static readonly ChunkNode<T> Empty = new(Array.Empty<T>());


	/// <summary>
	/// Initializes a <see cref="ChunkNode{T}"/> instance via the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChunkNode(T value) => (Type, Value, Length) = (ChunkNodeType.Value, value, 1);

	/// <summary>
	/// Initializes a <see cref="ChunkNode{T}"/> instance via the array.
	/// </summary>
	/// <param name="array">The array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChunkNode(T[] array) => (Type, ValueRef, Length) = (ChunkNodeType.Array, array, array.Length);

	/// <summary>
	/// Initializes a <see cref="ChunkNode{T}"/> instance via the list.
	/// </summary>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChunkNode(List<T> list) => (Type, ValueRef, Length) = (ChunkNodeType.List, list, list.Count);


	/// <summary>
	/// Indicates the pointer that points to the values.
	/// The value can be used if <see cref="Type"/> is not <see cref="ChunkNodeType.Value"/>.
	/// </summary>
	[field: FieldOffset(8)]
	public object? ValueRef { get; }

	/// <summary>
	/// Indicates the sequence length of the chunk.
	/// By default, the value is 1 if <see cref="Type"/> is <see cref="ChunkNodeType.Value"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	[field: FieldOffset(4)]
	public int Length { get; }

	/// <summary>
	/// Indicates the type of the chunk.
	/// </summary>
	[field: FieldOffset(0)]
	public ChunkNodeType Type { get; }

	/// <summary>
	/// Indicates the value. The value can be used if <see cref="Type"/> is <see cref="ChunkNodeType.Value"/>.
	/// </summary>
	[field: FieldOffset(8)]
	public T? Value { get; }

	/// <summary>
	/// Indicates whether the node represents for a single value.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Value))]
	[MemberNotNullWhen(false, nameof(ValueRef))]
	internal bool IsSingleValue => Type == ChunkNodeType.Value;


	/// <summary>
	/// Gets the value at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	/// <exception cref="NotSupportedException">Throws when the property is <see cref="ChunkNodeType.Value"/>.</exception>
	public ref readonly T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			switch (Type)
			{
				case ChunkNodeType.Value:
				{
					throw new NotSupportedException(
						$"""
						The sequence only contains one element, 
						and configured type is '{nameof(ChunkNodeType)}.{nameof(ChunkNodeType.Value)}', 
						meaning you cannot use indexer on such cases.
						""".RemoveLineEndings()
					);
				}
				case ChunkNodeType.Array:
				{
					Debug.Assert(!IsSingleValue);
					return ref ((T[])ValueRef)[index];
				}
				case ChunkNodeType.List:
				{
					Debug.Assert(!IsSingleValue);
					return ref ((List<T>)ValueRef).AsReadOnlySpan()[index];
				}
				default:
				{
					throw new InvalidOperationException();
				}
			}
		}
	}


	/// <summary>
	/// Compares the reference of the internal collection value from two values,
	/// and return a value indicating whether they reference to a same memory location.
	/// </summary>
	/// <param name="other">The other value to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReferenceEquals(scoped ref readonly ChunkNode<T> other)
	{
		if (Type != other.Type)
		{
			return false;
		}

		switch (Type)
		{
			case ChunkNodeType.Array:
			{
				Debug.Assert(!IsSingleValue);
				Debug.Assert(!other.IsSingleValue);
				return ReferenceEquals((T[])ValueRef, (T[])other.ValueRef);
			}
			case ChunkNodeType.List:
			{
				Debug.Assert(!IsSingleValue);
				Debug.Assert(!other.IsSingleValue);
				return ReferenceEquals((List<T>)ValueRef, (List<T>)other.ValueRef);
			}
			default:
			{
				return false;
			}
		}
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(in this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		if (Type == ChunkNodeType.Value)
		{
			Debug.Assert(IsSingleValue);
			yield return Value;
			yield break;
		}

		for (var i = 0; i < Length; i++)
		{
			yield return this[i];
		}
	}


	/// <summary>
	/// Explict cast from <see cref="ChunkNode{T}"/> to <typeparamref name="T"/>[].
	/// </summary>
	/// <param name="value">The value.</param>
	/// <exception cref="InvalidCastException">Throws when the value is not an array.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator T[](scoped in ChunkNode<T> value)
		=> value.Type == ChunkNodeType.Array ? (T[])value.ValueRef! : throw new InvalidCastException();

	/// <summary>
	/// Explict cast from <see cref="ChunkNode{T}"/> to <typeparamref name="T"/>[].
	/// </summary>
	/// <param name="value">The value.</param>
	/// <exception cref="InvalidCastException">Throws when the value is not a list.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator List<T>(scoped in ChunkNode<T> value)
		=> value.Type == ChunkNodeType.Array ? (List<T>)value.ValueRef! : throw new InvalidCastException();

	/// <summary>
	/// Implicit cast from <typeparamref name="T"/>[] to a <see cref="ChunkNode{T}"/>.
	/// </summary>
	/// <param name="array">The array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ChunkNode<T>(T[] array) => new(array);

	/// <summary>
	/// Implicit cast from <see cref="List{T}"/> of <typeparamref name="T"/> to a <see cref="ChunkNode{T}"/>.
	/// </summary>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ChunkNode<T>(List<T> list) => new(list);
}
