namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + guardian</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="GuardianDigit">The guardian digit.</param>
	/// <param name="GuardianCells">The guardians.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public sealed record UrWithGuardianStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, int[] Cells, int GuardianDigit, in Cells GuardianCells, int AbsoluteOffset
	) : UrStepInfo(Conclusions, Views, Technique.UrGuardian, Digit1, Digit2, Cells, false, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + .1M * (GuardianCells.Count >> 1);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

		[FormatItem]
		private string GuardianDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (GuardianDigit + 1).ToString();
		}

		[FormatItem]
		private string GuardianCellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GuardianCells.ToString();
		}
	}
}
