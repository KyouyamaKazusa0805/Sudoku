using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace System
{
	/// <summary>
	/// Provides and encapsulates a array that only contains the <see langword="unmanaged"/>-typed elements.
	/// The function of this structure is same as <see cref="Array"/>, but this one is lighter, because
	/// this data structure is only used in stack, rather than managed heap.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	public readonly unsafe ref partial struct ValueArray<TUnmanaged> where TUnmanaged : unmanaged
	{
		/// <summary>
		/// The inner array.
		/// </summary>
		private readonly TUnmanaged* _array;


		/// <summary>
		/// Initializes an instance with the specified length.
		/// </summary>
		/// <param name="length">The length.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueArray(int length)
		{
			var arr = stackalloc TUnmanaged[length];
			_array = arr;
			Length = length;
		}

		/// <summary>
		/// Initializes an instance with the specified pointer that points to an array,
		/// and the length of that array.
		/// </summary>
		/// <param name="ptr">The pointer.</param>
		/// <param name="length">The length.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueArray(TUnmanaged* ptr, int length)
		{
			_array = ptr;
			Length = length;
		}


		/// <summary>
		/// Indicates the length of the collection.
		/// </summary>
		public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }


		/// <summary>
		/// Gets or sets the specified value at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The value at the specified position.</returns>
		/// <value>The specified value to set.</value>
		public TUnmanaged this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => _array[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _array[index] = value;
		}


		/// <inheritdoc/>
		/// <remarks>
		/// The method is always return <see langword="false"/> because the instance can't box to heap memory.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => false;

		/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
		public bool Equals(in ValueArray<TUnmanaged> other)
		{
			if (Length != other.Length)
			{
				return false;
			}

			int i = 0;
			for (TUnmanaged* pThis = _array, pOther = other._array; i < Length; i++, pThis++, pOther++)
			{
				if (!Unsafe.AreSame(ref *pThis, ref *pOther))
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var z = new HashCode();
			for (int i = 0; i < Length; i++)
			{
				z.Add(_array[i]);
			}

			return z.ToHashCode();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() =>
			typeof(TUnmanaged) == typeof(char)
			? new ReadOnlySpan<TUnmanaged>(_array, 0).ToString()
			: $"ValueArray`1({typeof(TUnmanaged).Name}), Length = {Length.ToString()}";

		/// <summary>
		/// Get the reference of the first element. This method is used in <see langword="fixed"/>
		/// statement.
		/// </summary>
		/// <returns>The reference of the first element.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref readonly TUnmanaged GetPinnableReference() => ref _array[0];

		/// <summary>
		/// Converts the current instance to an array.
		/// </summary>
		/// <returns>The array of type <typeparamref name="TUnmanaged"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TUnmanaged[] ToArray()
		{
			var result = new TUnmanaged[Length];
			fixed (TUnmanaged* pResult = result)
			{
				Unsafe.CopyBlock(pResult, _array, (uint)(sizeof(TUnmanaged) * Length));
			}

			return result;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new(_array, Length);

		/// <summary>
		/// Slice the current array.
		/// </summary>
		/// <param name="start">
		/// The start index of the array you want to slice. The return array <b>contains</b> the element
		/// at the current index.
		/// </param>
		/// <param name="end">
		/// The end index of the array you want to slice. The return array <b>doesn't contain</b>
		/// the element at the current index.
		/// </param>
		/// <returns>The return array.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueArray<TUnmanaged> Slice(int start, int end) => new(_array + start, Length - end + start);


		/// <summary>
		/// Make the current array move to one offset unit behind itself.
		/// </summary>
		/// <param name="array">(<see langword="in"/> parameter) The current array.</param>
		/// <returns>
		/// The result array that starts with the first element (of index <c>[0]</c>)
		/// from the old array.
		/// </returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueArray<TUnmanaged> operator ++(in ValueArray<TUnmanaged> array) =>
			new(array._array + 1, array.Length - 1);

		/// <summary>
		/// Make the current array move to one offset unit before itself.
		/// </summary>
		/// <param name="array">(<see langword="in"/> parameter) The current array.</param>
		/// <returns>
		/// The result array that starts with the element of the index <c>[-1]</c> from the old array.
		/// </returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueArray<TUnmanaged> operator --(in ValueArray<TUnmanaged> array) =>
			new(array._array - 1, array.Length + 1);

		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in ValueArray<TUnmanaged> left, in ValueArray<TUnmanaged> right) =>
			left.Equals(right);

		/// <inheritdoc cref="Operators.operator =="/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in ValueArray<TUnmanaged> array, TUnmanaged* ptr) =>
			array._array == ptr;

		/// <inheritdoc cref="Operators.operator =="/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TUnmanaged* ptr, in ValueArray<TUnmanaged> array) =>
			array._array == ptr;

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in ValueArray<TUnmanaged> left, in ValueArray<TUnmanaged> right) =>
			!(left == right);

		/// <inheritdoc cref="Operators.operator !="/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in ValueArray<TUnmanaged> array, TUnmanaged* ptr) =>
			array._array != ptr;

		/// <inheritdoc cref="Operators.operator !="/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TUnmanaged* ptr, in ValueArray<TUnmanaged> array) =>
			array._array != ptr;

		/// <summary>
		/// Make the current array move to specified number of offsets unit behind itself.
		/// </summary>
		/// <param name="array">(<see langword="in"/> parameter) The current array.</param>
		/// <param name="offset">The specified offset you want to move.</param>
		/// <returns>The result array.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueArray<TUnmanaged> operator +(in ValueArray<TUnmanaged> array, int offset) =>
			new(array._array + offset, array.Length - offset);

		/// <summary>
		/// Make the current array move to specified number of offsets unit before itself.
		/// </summary>
		/// <param name="array">(<see langword="in"/> parameter) The current array.</param>
		/// <param name="offset">The specified offset you want to move.</param>
		/// <returns>The result array.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueArray<TUnmanaged> operator -(in ValueArray<TUnmanaged> array, int offset) =>
			new(array._array - offset, array.Length + offset);


		/// <summary>
		/// Implicit cast from <see cref="ValueArray{TUnmanaged}"/> to <typeparamref name="TUnmanaged"/>*.
		/// </summary>
		/// <param name="this">(<see langword="in"/> parameter) The current instance.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TUnmanaged*(in ValueArray<TUnmanaged> @this) => @this._array;

		/// <summary>
		/// Explicit cast from <see cref="ValueArray{TUnmanaged}"/> to <see cref="Span{T}"/>.
		/// </summary>
		/// <param name="this">(<see langword="in"/> parameter) The current instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Span<TUnmanaged>(in ValueArray<TUnmanaged> @this) =>
			new(@this._array, @this.Length);

		/// <summary>
		/// Explicit cast from <see cref="ValueArray{TUnmanaged}"/> to <see cref="ReadOnlySpan{T}"/>.
		/// </summary>
		/// <param name="this">(<see langword="in"/> parameter) The current instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator ReadOnlySpan<TUnmanaged>(in ValueArray<TUnmanaged> @this) =>
			new(@this._array, @this.Length);

		/// <summary>
		/// Explicit cast from <see cref="ValueArray{TUnmanaged}"/> to <typeparamref name="TUnmanaged"/>[].
		/// </summary>
		/// <param name="this">(<see langword="in"/> parameter) The current instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator TUnmanaged[](in ValueArray<TUnmanaged> @this) => @this.ToArray();
	}
}
