namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>avoidable rectangle + hidden single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All AR cells.</param>
	/// <param name="BaseCell">The base cell.</param>
	/// <param name="TargetCell">The target cell.</param>
	/// <param name="TargetRegion">The target region.</param>
	/// <param name="AbsoluteOffset">The absolute offset.</param>
	public sealed record ArWithHiddenSingleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit1, int Digit2,
		int[] Cells, int BaseCell, int TargetCell, int TargetRegion, int AbsoluteOffset
	) : UrStepInfo(
		Conclusions, Views, TargetRegion switch
		{
			>= 0 and < 9 => Technique.ArHiddenSingleBlock,
			>= 9 and < 18 => Technique.ArHiddenSingleRow,
			>= 18 and < 27 => Technique.ArHiddenSingleColumn
		}, Digit1, Digit2, Cells, true, AbsoluteOffset
	)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.7M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

		[FormatItem]
		private string BaseCellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { BaseCell }.ToString();
		}

		[FormatItem]
		private string TargetCellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { TargetCell }.ToString();
		}

		[FormatItem]
		private string RegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(TargetRegion).ToString();
		}

		[FormatItem]
		private string Digit1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit1 + 1).ToString();
		}
	}
}
