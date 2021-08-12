namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>BUG + n</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Candidates">All candidates used.</param>
	public sealed record BugMultipleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<int> Candidates
	) : BugStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The table of extra difficulty values.
		/// </summary>
		private static readonly decimal[] ExtraDifficulty =
		{
			.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
			.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M
		};


		/// <inheritdoc/>
		public override string Name => $"{(string)TextResources.Current.Bug} + {Candidates.Count}";

		/// <inheritdoc/>
		public override string? Acronym => $"BUG + {Candidates.Count.ToString()}";

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M + ExtraDifficulty[Candidates.Count - 1];

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugMultiple;

		[FormatItem]
		private string CandidatesStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates(Candidates).ToString();
		}
	}
}
