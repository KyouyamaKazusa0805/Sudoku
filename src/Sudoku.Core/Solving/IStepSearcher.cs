namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher.
/// </summary>
public interface IStepSearcher
{
	/// <summary>
	/// Determines whether the current step searcher is separated one, which mean it can be created
	/// as many possible instances in a same step searchers pool.
	/// </summary>
	public sealed bool IsSeparated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<SeparatedStepSearcherAttribute>() is not null;
	}

	/// <summary>
	/// Determines whether the current step searcher is a direct one.
	/// </summary>
	/// <remarks>
	/// If you don't know what is a direct step searcher, please visit the property
	/// <see cref="StepSearcherOptionsAttribute.IsDirect"/> to learn more information.
	/// </remarks>
	/// <seealso cref="StepSearcherOptionsAttribute.IsDirect"/>
	public sealed bool IsDirect
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherOptionsAttribute>()?.IsDirect ?? false;
	}

	/// <summary>
	/// Determines whether we can adjust the ordering of the current step searcher
	/// as a customized configuration option before solving a puzzle.
	/// </summary>
	/// <remarks>
	/// If you don't know what is a direct step searcher, please visit the property
	/// <see cref="StepSearcherOptionsAttribute.IsOptionsFixed"/> to learn more information.
	/// </remarks>
	/// <seealso cref="StepSearcherOptionsAttribute.IsOptionsFixed"/>
	public sealed bool IsOptionsFixed
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherOptionsAttribute>()?.IsOptionsFixed ?? false;
	}

	/// <summary>
	/// Determines whether the current step searcher is deprecated.
	/// </summary>
	/// <remarks>
	/// If you don't know what is a direct step searcher, please visit the property
	/// <see cref="StepSearcherOptionsAttribute.IsDeprecated"/> to learn more information.
	/// </remarks>
	/// <seealso cref="StepSearcherOptionsAttribute.IsDeprecated"/>
	public sealed bool IsDeprecated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherOptionsAttribute>()?.IsDeprecated ?? false;
	}

	/// <summary>
	/// Determines whether the current step searcher is not supported for sukaku solving mode.
	/// </summary>
	public sealed bool IsNotSupportedForSukaku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().IsDefined(typeof(SukakuNotSupportedAttribute));
	}

	/// <summary>
	/// Determines whether the current step searcher is too slow,
	/// with being applied <see cref="AlgorithmTooSlowAttribute"/>.
	/// </summary>
	/// <seealso cref="AlgorithmTooSlowAttribute"/>
	public sealed bool IsConfiguredSlow
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().IsDefined(typeof(AlgorithmTooSlowAttribute));
	}

	/// <summary>
	/// Indicates the step searching options.
	/// </summary>
	public abstract SearcherInitializationOptions Options { get; set; }


	/// <summary>
	/// Accumulate all possible steps into the argument <paramref name="accumulator"/> if
	/// the argument <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or return the first found step if the argument <paramref name="onlyFindOne"/>
	/// is <see langword="true"/>.
	/// </summary>
	/// <param name="accumulator">
	/// <para>
	/// <para>The accumulator to store each step.</para>
	/// </para>
	/// <para>
	/// If <paramref name="onlyFindOne"/> is set to <see langword="true"/>,
	/// this argument will become useless because we only finding one step is okay,
	/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
	/// Therefore, this argument can be <see langword="null"/>
	/// (i.e. the expression <c><see langword="null"/>!</c>) when the argument
	/// <paramref name="onlyFindOne"/> is <see langword="true"/>.
	/// </para>
	/// </param>
	/// <param name="grid">The grid to search for techniques.</param>
	/// <param name="onlyFindOne">
	/// Indicates whether the method only searches for one <see cref="IStep"/> instance.
	/// </param>
	/// <returns>
	/// Returns the first found step. The nullability of the return value are as belows:
	/// <list type="bullet">
	/// <item>
	/// <see langword="null"/>:
	/// <list type="bullet">
	/// <item><c><paramref name="onlyFindOne"/> == <see langword="false"/></c>.</item>
	/// <item><c><paramref name="onlyFindOne"/> == <see langword="true"/></c>, but <b>nothing</b> found.</item>
	/// </list>
	/// </item>
	/// <item>
	/// Not <see langword="null"/>:
	/// <list type="bullet">
	/// <item>
	/// <c><paramref name="onlyFindOne"/> == <see langword="true"/></c>, and found <b>at least one step</b>.
	/// In this case the return value is the first found step.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </returns>
	public abstract IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne);
}
