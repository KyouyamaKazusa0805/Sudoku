namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet">INdicates the exocet pattern.</param>
/// <param name="DigitsMask">Indicates the mask that holds all possible digits used.</param>
/// <param name="Eliminations">Indicates all possible eliminations.</param>
internal abstract record ExocetStep(ViewList Views, scoped in Exocet Exocet, short DigitsMask, ImmutableArray<ExocetElimination> Eliminations) :
	Step(GatherConclusions(Eliminations), Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Exocet;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Exocet;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.HardlyEver;

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	[DebuggerHidden]
	private CellMap BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.BaseCellsMap;
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	[DebuggerHidden]
	private CellMap TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.TargetCellsMap;
	}


	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	/// <summary>
	/// Indicates the base map string.
	/// </summary>
	[ResourceTextFormatter]
	internal string BaseCellsStr() => BaseMap.ToString();

	/// <summary>
	/// Indicates the target map string.
	/// </summary>
	[ResourceTextFormatter]
	internal string TargetCellsStr() => TargetMap.ToString();


	/// <summary>
	/// Gather conclusions.
	/// </summary>
	/// <returns>The gathered result.</returns>
	private static ConclusionList GatherConclusions(ImmutableArray<ExocetElimination> eliminations)
	{
		var result = new List<Conclusion>();
		foreach (var eliminationInstance in eliminations)
		{
			result.AddRange(eliminationInstance.Conclusions);
		}

		return ImmutableArray.CreateRange(result);
	}
}
