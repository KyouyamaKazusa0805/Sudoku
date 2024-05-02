namespace Sudoku.Measuring;

/// <summary>
/// Represents a read-only collection of <see cref="Factor"/> instances.
/// </summary>
/// <param name="factors">Indicates the factors inside the data structure.</param>
/// <seealso cref="Factor"/>
[CollectionBuilder(typeof(FactorCollection), nameof(Create))]
public sealed partial class FactorCollection(ReadOnlyMemory<Factor> factors) : IEnumerable<Factor>, IReadOnlyList<Factor>, IReadOnlyCollection<Factor>
{
	/// <summary>
	/// Represents the empty instance.
	/// </summary>
	public static readonly FactorCollection Empty = new(ReadOnlyMemory<Factor>.Empty);


	/// <summary>
	/// Indicates the length of elements stored in this collection.
	/// </summary>
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => factors.Length;
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
		get => factors.Span[index];
	}


	/// <summary>
	/// Checks for all elements in this collection, finding for the first element satisfying the specified condition;
	/// return <see langword="null"/> if no such elements in this colletion.
	/// </summary>
	/// <param name="match">The condition to be checked.</param>
	/// <returns>The first found <see cref="Factor"/> instance.</returns>
	public Factor? FirstOrDefault(Func<Factor, bool> match)
	{
		foreach (var factor in factors)
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
		foreach (var factor in factors)
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

	/// <summary>
	/// Calculates sum of difficulty of the current step, using the specified value converter.
	/// </summary>
	/// <typeparam name="T">The type of the value converter.</typeparam>
	/// <param name="step">The step.</param>
	/// <param name="valueConverter">The value converter to be used.</param>
	/// <returns>The sum value of type <typeparamref name="T"/>.</returns>
	public T Sum<T>(Step step, Func<int, T> valueConverter) where T : unmanaged, INumber<T>
	{
		var result = default(T);
		foreach (var element in this)
		{
			result += valueConverter(element.Formula(step) ?? 0);
		}
		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(factors.Span);

	/// <summary>
	/// Slices the collection via the specified index as the start, and the number of elements to be sliced.
	/// </summary>
	/// <param name="start">The index of the start element.</param>
	/// <param name="length">The number of elements to be sliced.</param>
	/// <returns>A <see cref="FactorCollection"/> instance sliced.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FactorCollection Slice(int start, int length) => new(factors[start..(start + length)]);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Factor>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Factor> IEnumerable<Factor>.GetEnumerator()
	{
		foreach (var element in factors.ToArray())
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
	public static FactorCollection Create(ReadOnlySpan<Factor> factors) => factors.IsEmpty ? Empty : new(factors.ToArray());
}
