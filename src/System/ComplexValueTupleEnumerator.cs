using System.Collections;
using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Defines a complex value tuple enumerator.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <typeparam name="TRest">The type that encapsulate for a list of rest elements.</typeparam>
public ref struct ComplexValueTupleEnumerator<T, TRest> where TRest : struct
{
	/// <summary>
	/// Indicates the internal values to be iterated.
	/// </summary>
	private readonly ReadOnlySpan<T> _values;

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Initializes a <see cref="ComplexValueTupleEnumerator{T, TRest}"/> instance.
	/// </summary>
	/// <param name="tuple">The tuple.</param>
	public ComplexValueTupleEnumerator(ValueTuple<T, T, T, T, T, T, T, TRest> tuple)
	{
		_values = [tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, .. getExtraValues(tuple.Rest)];


		static ReadOnlySpan<T> getExtraValues<TRestTuple>(TRestTuple tuple) where TRestTuple : struct
		{
			switch (tuple)
			{
				case ValueTuple<T> a: { return [a.Item1]; }
				case ValueTuple<T, T> a: { return [a.Item1, a.Item2]; }
				case ValueTuple<T, T, T> a: { return [a.Item1, a.Item2, a.Item3]; }
				case ValueTuple<T, T, T, T> a: { return [a.Item1, a.Item2, a.Item3, a.Item4]; }
				case ValueTuple<T, T, T, T, T> a: { return [a.Item1, a.Item2, a.Item3, a.Item4, a.Item5]; }
				case ValueTuple<T, T, T, T, T, T> a: { return [a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6]; }
				case ValueTuple<T, T, T, T, T, T, T> a: { return [a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6, a.Item7]; }
				case ValueTuple<T, T, T, T, T, T, T, TRest> a:
				{
					return [a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6, a.Item7, .. getExtraValues(a.Rest)];
				}
				default: { throw new InvalidOperationException("The rest type is invalid."); }
			}
		}
	}


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public readonly ref readonly T Current => ref _values[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => _index++ < _values.Length;
}
