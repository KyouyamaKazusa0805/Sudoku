namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides extension methods on <see cref="IStepSearcher"/> instances.
/// </summary>
/// <seealso cref="IStepSearcher"/>
public static class StepSearcherPoolingExtensions
{
	/// <summary>
	/// Try to fetch a valid <typeparamref name="TStepSearcher"/> instance via the specified pool.
	/// </summary>
	/// <typeparam name="TStepSearcher">The type of the step searcher you want to fetch.</typeparam>
	/// <param name="stepSearchersPool">The pool where all possible step searchers are stored.</param>
	/// <returns>
	/// The found step searcher instance. If the type is marked <see cref="SeparatedStepSearcherAttribute"/>,
	/// the method will return the first found instance.
	/// </returns>
	/// <seealso cref="SeparatedStepSearcherAttribute"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TStepSearcher? GetOfType<TStepSearcher>(this IStepSearcher[] stepSearchersPool)
		where TStepSearcher : class, IStepSearcher
		=> stepSearchersPool.OfType<TStepSearcher>().FirstOrDefault();

	/// <summary>
	/// Try to fetch a valid <typeparamref name="TStepSearcher"/> instances via the specified pool.
	/// </summary>
	/// <typeparam name="TStepSearcher">The type of the step searcher you want to fetch.</typeparam>
	/// <param name="stepSearchersPool">The pool where all possible step searchers are stored.</param>
	/// <param name="getAll">
	/// Indicates whether you want to fetch all possible instances at the same time.
	/// </param>
	/// <returns>
	/// The found step searcher instances. If the type is marked <see cref="SeparatedStepSearcherAttribute"/>,
	/// the method will return all possible instances, whose behavior is different with the other method
	/// <see cref="GetOfType{TStepSearcher}(IStepSearcher[])"/>.
	/// </returns>
	/// <seealso cref="SeparatedStepSearcherAttribute"/>
	/// <seealso cref="GetOfType{TStepSearcher}(IStepSearcher[])"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TStepSearcher[] GetOfType<TStepSearcher>(this IStepSearcher[] stepSearchersPool, bool getAll)
		=> getAll
			? stepSearchersPool.OfType<TStepSearcher>().ToArray()
			: stepSearchersPool.OfType<TStepSearcher>().FirstOrDefault() is { } r
				? new[] { r }
				: Array.Empty<TStepSearcher>();
}
