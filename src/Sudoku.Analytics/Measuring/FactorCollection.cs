namespace Sudoku.Measuring;

/// <summary>
/// Represents a read-only collection of <see cref="Factor"/> instances.
/// </summary>
/// <seealso cref="Factor"/>
[CollectionBuilder(typeof(FactorCollection), nameof(Create))]
public sealed partial class FactorCollection : IEnumerable<Factor>, IReadOnlyList<Factor>, IReadOnlyCollection<Factor>
{
	/// <summary>
	/// Represents the empty instance.
	/// </summary>
	public static readonly FactorCollection Empty = new(ReadOnlyMemory<Factor>.Empty);


	/// <summary>
	/// Indicates the factors.
	/// </summary>
	private readonly ReadOnlyMemory<Factor> _factors;


	/// <summary>
	/// Initializes a <see cref="FactorCollection"/> instance via initial elements.
	/// </summary>
	/// <param name="factors">The factors.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private FactorCollection(ReadOnlyMemory<Factor> factors) => _factors = factors;


	/// <summary>
	/// Indicates the length of elements stored in this collection.
	/// </summary>
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _factors.Length;
	}

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
		get => _factors.Span[index];
	}


	/// <summary>
	/// Checks for all elements in this collection, finding for the first element satisfying the specified condition;
	/// return <see langword="null"/> if no such elements in this colletion.
	/// </summary>
	/// <param name="match">The condition to be checked.</param>
	/// <returns>The first found <see cref="Factor"/> instance.</returns>
	public Factor? FirstOrDefault(Func<Factor, bool> match)
	{
		foreach (var factor in _factors)
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
		foreach (var factor in _factors)
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
			result += element.Formula(step) ?? 0;
		}
		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_factors.Span);

	/// <summary>
	/// Slices the collection via the specified index as the start, and the number of elements to be sliced.
	/// </summary>
	/// <param name="start">The index of the start element.</param>
	/// <param name="length">The number of elements to be sliced.</param>
	/// <returns>A <see cref="FactorCollection"/> instance sliced.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FactorCollection Slice(int start, int length) => new(_factors[start..(start + length)]);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Factor>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Factor> IEnumerable<Factor>.GetEnumerator()
	{
		foreach (var element in _factors.ToArray())
		{
			yield return element;
		}
	}


	/// <summary>
	/// Creates a <see cref="FactorCollection"/> instance.
	/// </summary>
	/// <param name="factors">The factors to be used as initial values.</param>
	/// <returns>A <see cref="FactorCollection"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FactorCollection Create(scoped ReadOnlySpan<Factor> factors) => factors.IsEmpty ? Empty : new(factors.ToArray());
}
