namespace System;

/// <summary>
/// Represents a trunk node. The type will be used by <see cref="ReadOnlyTrunks{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of each values.</typeparam>
/// <seealso cref="ReadOnlyTrunks{T}"/>
[StructLayout(LayoutKind.Explicit)]
[CollectionBuilder(typeof(TrunkNodeBuilder), nameof(TrunkNodeBuilder.Create))]
[LargeStructure]
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
[SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
public readonly unsafe partial struct TrunkNode<T> : IEnumerable<T>
{
	/// <summary>
	/// Represents an empty instance.
	/// </summary>
	public static readonly TrunkNode<T> Empty = new(Array.Empty<T>());


	/// <summary>
	/// Initializes a <see cref="TrunkNode{T}"/> instance via the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TrunkNode(T value) => (Type, Value, Length) = (TrunkNodeType.Value, value, 1);

	/// <summary>
	/// Initializes a <see cref="TrunkNode{T}"/> instance via the array.
	/// </summary>
	/// <param name="array">The array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TrunkNode(T[] array) => (Type, ValueRef, Length) = (TrunkNodeType.Array, array, array.Length);

	/// <summary>
	/// Initializes a <see cref="TrunkNode{T}"/> instance via the list.
	/// </summary>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TrunkNode(List<T> list) => (Type, ValueRef, Length) = (TrunkNodeType.List, list, list.Count);


	/// <summary>
	/// Indicates the pointer that points to the values.
	/// The value can be used if <see cref="Type"/> is not <see cref="TrunkNodeType.Value"/>.
	/// </summary>
	[field: FieldOffset(8)]
	public object? ValueRef { get; }

	/// <summary>
	/// Indicates the sequence length of the trunk.
	/// By default, the value is 1 if <see cref="Type"/> is <see cref="TrunkNodeType.Value"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	[field: FieldOffset(4)]
	public int Length { get; }

	/// <summary>
	/// Indicates the type of the trunk.
	/// </summary>
	[field: FieldOffset(0)]
	public TrunkNodeType Type { get; }

	/// <summary>
	/// Indicates the value. The value can be used if <see cref="Type"/> is <see cref="TrunkNodeType.Value"/>.
	/// </summary>
	[field: FieldOffset(8)]
	public T? Value { get; }

	/// <summary>
	/// Indicates whether the node represents for a single value.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Value))]
	[MemberNotNullWhen(false, nameof(ValueRef))]
	internal bool IsSingleValue => Type == TrunkNodeType.Value;


	/// <summary>
	/// Gets the value at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	/// <exception cref="NotSupportedException">Throws when the property is <see cref="TrunkNodeType.Value"/>.</exception>
	public ref readonly T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			switch (Type)
			{
				case TrunkNodeType.Value:
				{
					throw new NotSupportedException(
						$"""
						The sequence only contains one element, 
						and configured type is '{nameof(TrunkNodeType)}.{nameof(TrunkNodeType.Value)}', 
						meaning you cannot use indexer on such cases.
						""".RemoveLineEndings()
					);
				}
				case TrunkNodeType.Array:
				{
					Debug.Assert(!IsSingleValue);
					return ref ((T[])ValueRef)[index];
				}
				case TrunkNodeType.List:
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
	public bool ReferenceEquals(scoped ref readonly TrunkNode<T> other)
	{
		if (Type != other.Type)
		{
			return false;
		}

		switch (Type)
		{
			case TrunkNodeType.Array:
			{
				Debug.Assert(!IsSingleValue);
				Debug.Assert(!other.IsSingleValue);
				return ReferenceEquals((T[])ValueRef, (T[])other.ValueRef);
			}
			case TrunkNodeType.List:
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
		if (Type == TrunkNodeType.Value)
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
	/// Explict cast from <see cref="TrunkNode{T}"/> to <typeparamref name="T"/>[].
	/// </summary>
	/// <param name="value">The value.</param>
	/// <exception cref="InvalidCastException">Throws when the value is not an array.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator T[](scoped in TrunkNode<T> value)
		=> value.Type == TrunkNodeType.Array ? (T[])value.ValueRef! : throw new InvalidCastException();

	/// <summary>
	/// Explict cast from <see cref="TrunkNode{T}"/> to <typeparamref name="T"/>[].
	/// </summary>
	/// <param name="value">The value.</param>
	/// <exception cref="InvalidCastException">Throws when the value is not a list.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator List<T>(scoped in TrunkNode<T> value)
		=> value.Type == TrunkNodeType.Array ? (List<T>)value.ValueRef! : throw new InvalidCastException();

	/// <summary>
	/// Implicit cast from <typeparamref name="T"/>[] to a <see cref="TrunkNode{T}"/>.
	/// </summary>
	/// <param name="array">The array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TrunkNode<T>(T[] array) => new(array);

	/// <summary>
	/// Implicit cast from <see cref="List{T}"/> of <typeparamref name="T"/> to a <see cref="TrunkNode{T}"/>.
	/// </summary>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TrunkNode<T>(List<T> list) => new(list);
}
