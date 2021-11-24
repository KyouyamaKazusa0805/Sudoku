namespace Sudoku.Solving.Manual.Steps.LastResorts;

/// <summary>
/// Provides with a step that is a <b>Brute Force</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public sealed record BruteForceStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views
) : LastResortStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 20.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BruteForce;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Bf;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Always;

	[FormatItem]
	private string AssignmentStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new ConclusionCollection(Conclusions).ToString();
	}
}
