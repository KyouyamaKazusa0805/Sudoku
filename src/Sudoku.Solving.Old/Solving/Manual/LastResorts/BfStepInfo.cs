namespace Sudoku.Solving.Manual.LastResorts;

/// <summary>
/// Provides a usage of <b>brute force</b> (<b>BF</b>) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public sealed record BfStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: LastResortStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 20.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BruteForce;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Bf;

	[FormatItem]
	private string AssignmentStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new ConclusionCollection(Conclusions).ToString();
	}
}
