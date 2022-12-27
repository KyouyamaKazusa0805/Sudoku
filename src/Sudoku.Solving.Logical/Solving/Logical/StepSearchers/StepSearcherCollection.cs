namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Defines a collection that only stores for <see cref="IStepSearcher"/> instances.
/// </summary>
/// <seealso cref="IStepSearcher"/>
/// <completionlist cref="WellKnownStepSearcherCollections"/>
public sealed class StepSearcherCollection :
	IEnumerable<IStepSearcher>,
	IReadOnlyCollection<IStepSearcher>,
	IReadOnlyList<IStepSearcher>,
	ISlicable<StepSearcherCollection, IStepSearcher>
{
	/// <summary>
	/// The internal array of <see cref="IStepSearcher"/>s.
	/// </summary>
	private readonly IStepSearcher[] _stepSearchers;


	/// <summary>
	/// Initializes a <see cref="StepSearcherCollection"/> instance via the specified collection of <see cref="IStepSearcher"/>s.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private StepSearcherCollection(IStepSearcher[] stepSearchers) => _stepSearchers = stepSearchers;


	/// <inheritdoc/>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _stepSearchers.Length;
	}

	/// <inheritdoc/>
	int ISlicable<StepSearcherCollection, IStepSearcher>.Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Count;
	}


	/// <inheritdoc/>
	public IStepSearcher this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _stepSearchers[index];
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StepSearcherCollection Slice(int start, int count) => new(_stepSearchers[start..(start + count)]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<IStepSearcher> GetEnumerator() => ((IEnumerable<IStepSearcher>)_stepSearchers).GetEnumerator();

	/// <summary>
	/// Filters the collection, removing <see cref="IStepSearcher"/> instances that is not target type.
	/// </summary>
	/// <typeparam name="T">The type of the target step searcher.</typeparam>
	/// <returns>A list of <see cref="IStepSearcher"/>s that are of type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<T> OfType<T>() where T : class, IStepSearcher => _stepSearchers.OfType<T>();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <summary>
	/// Implicit cast from <see cref="IStepSearcher"/>[] to <see cref="StepSearcherCollection"/>.
	/// </summary>
	/// <param name="stepSearchers">The step searchers.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator StepSearcherCollection(IStepSearcher[] stepSearchers) => new(stepSearchers);
}
