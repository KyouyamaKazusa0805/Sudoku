namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Petals">Indicates the petals used.</param>
internal abstract record DeathBlossomStep(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	AlmostLockedSet[] Petals
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string AlsesStr() => string.Join(R.EmitPunctuation(Punctuation.Comma), from als in Petals select als.ToString());

	[ResourceTextFormatter]
	internal string DigitStr() => DigitMaskFormatter.Format(DigitsMask);
}
