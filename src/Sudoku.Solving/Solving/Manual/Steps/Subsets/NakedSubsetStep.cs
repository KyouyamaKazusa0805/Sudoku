using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.Subsets;

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
	ImmutableArray<PresentationData> Views,
	int Region,
	in Cells Cells,
	short DigitsMask,
	bool? IsLocked
) : SubsetStep(Conclusions, Views, Region, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		Size switch { 2 => 3.0M, 3 => 3.6M, 4 => 5.0M } // Base difficulty.
		+ IsLocked switch
		{
			true => Size switch { 2 => -1.0M, 3 => -1.1M, },
			false => .1M,
			_ => 0
		}; // Locked difficulty.

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		(IsLocked, Size) switch
		{
			(IsLocked: true, Size: 2) => Technique.LockedPair,
			(IsLocked: false, Size: 2) => Technique.NakedPairPlus,
			(IsLocked: null, Size: 2) => Technique.NakedPair,
			(IsLocked: true, Size: 3) => Technique.LockedTriple,
			(IsLocked: false, Size: 3) => Technique.NakedTriplePlus,
			(IsLocked: null, Size: 3) => Technique.NakedTriple,
			(IsLocked: false, Size: 4) => Technique.NakedQuadruplePlus,
			(IsLocked: null, Size: 4) => Technique.NakedQuadruple
		};

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}

	[FormatItem]
	private string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExternalResourceManager.Shared[$"subsetNames{Size}"];
	}
}