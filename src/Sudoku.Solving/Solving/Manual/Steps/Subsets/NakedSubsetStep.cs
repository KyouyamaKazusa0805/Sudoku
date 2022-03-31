namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Region"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="IsLocked">
/// Indicates which locked type this subset is. The cases are as belows:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The subset is a locked subset.</description>
/// </item>
/// <item>
/// <term><see langword="true"/></term>
/// <description>The subset is a naked subset with at least one extra locked candidate.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>The subset is a normal naked subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
public sealed record NakedSubsetStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<View> Views,
	int Region,
	in Cells Cells,
	short DigitsMask,
	bool? IsLocked
) : SubsetStep(Conclusions, Views, Region, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		Size switch
		{
			2 => 3.0M,
			3 => 3.6M,
			4 => 5.0M,
			_ => throw new NotSupportedException("The specified size is not supported.")
		} // Base difficulty.
			+ IsLocked switch
			{
				true => Size switch
				{
					2 => -1.0M,
					3 => -1.1M,
					_ => throw new NotSupportedException("The specified size is not supported.")
				},
				false => .1M,
				_ => 0
			}; // Locked difficulty.

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		(IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(null, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(null, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple,
			_ => throw new InvalidOperationException("The current status is invalid.")
		};

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}

	[FormatItem]
	internal string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[$"SubsetNamesSize{Size}"]!;
	}
}
