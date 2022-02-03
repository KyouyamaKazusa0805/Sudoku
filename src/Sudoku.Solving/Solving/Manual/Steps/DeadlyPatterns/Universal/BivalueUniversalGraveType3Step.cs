using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Solving.Manual.Constants;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TrueCandidates">Indicates the true candidates.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsNaked">Indicates whether the specified subset is naked subset.</param>
public sealed record BivalueUniversalGraveType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	IReadOnlyList<int> TrueCandidates,
	short DigitsMask,
	in Cells Cells,
	bool IsNaked
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		base.Difficulty
			+ Size * .1M // Subset extra difficulty.
			+ (IsNaked ? 0 : .1M); // Hidden subset extra difficulty.

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)DigitsMask);

	[FormatItem]
	internal string TrueCandidatesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(TrueCandidates).ToString();
	}

	[FormatItem]
	internal string SubsetTypeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ResourceManager.Shared[IsNaked ? "NakedKeyword" : "HiddenKeyword"];
	}

	[FormatItem]
	internal string SizeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SubsetNames[Size].ToLower(null);
	}

	[FormatItem]
	internal string SizeStrZhCn
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ResourceManager.Shared[$"SubsetNames{Size}"];
	}

	[FormatItem]
	internal string ExtraDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}
