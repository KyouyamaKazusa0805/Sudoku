namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Provides a usage of <b>death blossom</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Pivot">The pivot cell.</param>
/// <param name="Petals">All ALSes used.</param>
public sealed record class DbStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int Pivot, IReadOnlyDictionary<int, Als> Petals
) : AlsStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 8.0M + Petals.Count * .1M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.DeathBlossom;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	[FormatItem]
	private string PivotStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { Pivot }.ToString();
	}

	[FormatItem]
	private unsafe string PetalsStr
	{
		get
		{
			const string separator = ", ";

			var sb = new ValueStringBuilder(stackalloc char[50]);
			sb.AppendRange(Petals, &converter, separator);
			return sb.ToString();


			static string converter(KeyValuePair<int, Als> pair) => $"{pair.Key - 1} - {pair.Value}";
		}
	}
}
