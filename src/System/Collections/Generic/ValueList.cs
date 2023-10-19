#pragma warning disable IDE0032
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using System.Text;

namespace System.Collections.Generic;

/// <summary>
/// Defines a value-type sequence list, using unmanaged pointer types to describe the sequence memory.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="capacity">Indicates the length of the list.</param>
/// <remarks>
/// We recommend you use this type like:
/// <code><![CDATA[
/// static int[] Example()
/// {
///     using scoped var list = new ValueList<int>(10);
///     list.Add(3);
///     list.Add(6);
///     return [.. list];
/// }
/// ]]></code>
/// </remarks>
[StructLayout(LayoutKind.Auto)]
[CollectionBuilder(typeof(ValueListCreator), nameof(ValueListCreator.Create))]
[ToString(ToStringBehavior.CallOverload)]
public unsafe ref partial struct ValueList<T>([DataMember(MemberKinds.Field)] byte capacity) where T : notnull
{
	/// <summary>
	/// Indicates the current length.
	/// </summary>
	private byte _length = 0;

	/// <summary>
	/// Indicates the pointer that points to the first element.
	/// </summary>
	private T?* _startPtr = (T?*)NativeMemory.Alloc((nuint)sizeof(T) * capacity);


	/// <summary>
	/// Initializes a <see cref="ValueList{T}"/> instance via the default capacity 255.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueList() : this(byte.MaxValue)
	{
	}


	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	public readonly byte Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _length;
	}

	/// <summary>
	/// Indicates the length of the list. The property is same as <see cref="Count"/>, but the property is used
	/// by slicing and list patterns.
	/// </summary>
	/// <seealso cref="Count"/>
	public readonly int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _length;
	}


	/// <summary>
	/// Gets the element from the current list, or sets the element to the current list,
	/// with the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public readonly ref T this[byte index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _startPtr[index];
	}

	/// <inheritdoc cref="this[byte]"/>
	public readonly ref T this[Index index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _startPtr[index.GetOffset(_length)];
	}


	/// <summary>
	/// Adds the element to the current list.
	/// </summary>
	/// <param name="element">The element.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T element)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(_length, _capacity);

		_startPtr[_length++] = element;
	}

	/// <summary>
	/// Try to add the element into the current list, and return <see langword="false"/> if the collection is full.
	/// </summary>
	/// <param name="element">The element.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is successful.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryAdd(T element)
	{
		if (_length >= _capacity)
		{
			return false;
		}

		_startPtr[_length++] = element;
		return true;
	}

	/// <summary>
	/// Removes the last element from the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove() => _length--;

	/// <summary>
	/// Removes all elements in this collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => _length = 0;

	/// <summary>
	/// To dispose the current list.
	/// </summary>
	/// <remarks><i>
	/// This method should be called when the constructor <see cref="ValueList{T}(byte)"/> is called.
	/// </i></remarks>
	/// <seealso cref="ValueList{T}(byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		NativeMemory.Free(_startPtr);
		_startPtr = null;
	}

	/// <summary>
	/// Determines whether the specified element is in the current collection
	/// using the specified equality comparing method to define whether two instances are considered equal.
	/// </summary>
	/// <param name="instance">The instance to be determined.</param>
	/// <param name="predicate">A method that defines whether two instances are considered equal.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public readonly bool Contains(T instance, delegate*<T, T, bool> predicate)
	{
		foreach (var element in this)
		{
			if (predicate(element, instance))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Returns a string that represents the current object with the custom format string.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The string that represents the current object.</returns>
	/// <exception cref="FormatException">Throws when the specified format is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
	{
		return format switch
		{
			null or "L" or "l" => $"ValueList<{typeof(T).Name}> {{ Count = {_length}, Capacity = {_capacity} }}",
			"C" or "c" => toContentString(in this),
			"S" or "s" => $"ValueList<{typeof(T).Name}> {{ Size = {sizeof(T) * _length} }}",
			_ => throw new FormatException("The specified format doesn't support.")
		};


		static string toContentString(scoped ref readonly ValueList<T> @this)
		{
			const string separator = ", ";
			scoped var sb = new StringHandler();
			foreach (var element in @this)
			{
				sb.Append(element.ToString()!);
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return $"[{sb.ToStringAndClear()}]";
		}
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(in this);

	/// <summary>
	/// Converts the current instance into an array of type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>The array of elements of type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly T?[] ToArray()
	{
		var result = new T?[_length];
		Unsafe.CopyBlock(ref Unsafe.As<T?, byte>(ref result[0]), in Unsafe.As<T?, byte>(ref _startPtr[0]), (uint)(sizeof(T) * _length));
		return result;
	}
}
