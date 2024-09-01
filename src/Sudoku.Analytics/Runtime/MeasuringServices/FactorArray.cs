namespace Sudoku.Runtime.MeasuringServices;

/// <summary>
/// Represents a read-only array of <see cref="Factor"/> values.
/// </summary>
/// <param name="_values">Indicates the values.</param>
/// <seealso cref="Factor"/>
[CollectionBuilder(typeof(FactorArray), nameof(Create))]
public readonly ref partial struct FactorArray(ReadOnlyMemory<Factor> _values) :
	IEnumerable<Factor>,
	ISliceMethod<FactorArray, Factor>,
	IToArrayMethod<FactorArray, Factor>,
	IReadOnlyList<Factor>,
	IReadOnlyCollection<Factor>
{
	/// <summary>
	/// Indicates the length of elements stored in this collection.
	/// </summary>
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _values.Length;
	}

	/// <summary>
	/// Indicates the sequence of <see cref="Factor"/> instances stored in the current collection.
	/// </summary>
	public ReadOnlySpan<Factor> Span => _values.Span;

	/// <inheritdoc/>
	int IReadOnlyCollection<Factor>.Count => Length;


	/// <summary>
	/// Gets the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The <see cref="Factor"/> instance at the specified index.</returns>
	public Factor this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _values.Span[index];
	}


	/// <summary>
	/// Checks for all elements in this collection, finding for the first element satisfying the specified condition;
	/// return <see langword="null"/> if no such elements in this collection.
	/// </summary>
	/// <param name="match">The condition to be checked.</param>
	/// <returns>The first found <see cref="Factor"/> instance.</returns>
	public Factor? FirstOrDefault(Func<Factor, bool> match)
	{
		foreach (var factor in _values)
		{
			if (match(factor))
			{
				return factor;
			}
		}
		return null;
	}

	/// <summary>
	/// Iterates on each element, executing the specified action.
	/// </summary>
	/// <param name="action">The action to be executed and applied to each element.</param>
	public void ForEach(Action<Factor> action)
	{
		foreach (var factor in _values)
		{
			action(factor);
		}
	}

	/// <summary>
	/// Calculates sum of difficulty of the current step.
	/// </summary>
	/// <param name="step">The step.</param>
	/// <returns>The sum value.</returns>
	public int Sum(Step step)
	{
		var result = 0;
		foreach (var element in this)
		{
			result += element.Formula(from pi in element.Parameters select pi.GetValue(step)!);
		}
		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_values.Span);

	/// <summary>
	/// Slices the collection via the specified index as the start, and the number of elements to be sliced.
	/// </summary>
	/// <param name="start">The index of the start element.</param>
	/// <param name="length">The number of elements to be sliced.</param>
	/// <returns>A <see cref="FactorArray"/> instance sliced.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FactorArray Slice(int start, int length) => new(_values[start..(start + length)]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Factor[] ToArray() => _values.ToArray();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Factor> IEnumerable<Factor>.GetEnumerator()
		=> ((IEnumerable<Factor>)ToArray()).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<Factor> ISliceMethod<FactorArray, Factor>.Slice(int start, int count)
		=> Slice(start, count).ToArray();


	/// <summary>
	/// Creates a <see cref="FactorArray"/> instance.
	/// </summary>
	/// <param name="factors">
	/// <para>The factors to be used as initial values.</para>
	/// <include file="../../global-doc-comments.xml" path="g/csharp11/feature[@name='scoped-keyword']"/>
	/// </param>
	/// <returns>A <see cref="FactorArray"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FactorArray Create(scoped ReadOnlySpan<Factor> factors)
		=> factors.IsEmpty ? new(ReadOnlyMemory<Factor>.Empty) : new(factors.ToArray());
}
