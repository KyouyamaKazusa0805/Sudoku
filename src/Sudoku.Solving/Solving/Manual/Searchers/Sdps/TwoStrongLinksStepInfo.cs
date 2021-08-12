namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>two strong links</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseRegion">The base region.</param>
	/// <param name="TargetRegion">The target region.</param>
	public sealed record TwoStrongLinksStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit, int BaseRegion, int TargetRegion
	) : SdpStepInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => TechniqueCode switch
		{
			Technique.TurbotFish => 4.2M,
			Technique.Skyscraper => 4.0M,
			Technique.TwoStringKite => 4.1M
		};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override Technique TechniqueCode => (BaseKind: BaseRegion / 9, TargetKind: TargetRegion / 9) switch
		{
			(BaseKind: 0, _) or (_, TargetKind: 0) => Technique.TurbotFish,
			(BaseKind: 1, TargetKind: 1) or (BaseKind: 2, TargetKind: 2) => Technique.Skyscraper,
			(BaseKind: 1, TargetKind: 2) or (BaseKind: 2, TargetKind: 1) => Technique.TwoStringKite
		};

		[FormatItem]
		private string DigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit + 1).ToString();
		}

		[FormatItem]
		private string BaseRegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(BaseRegion).ToString();
		}

		[FormatItem]
		private string TargetRegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(TargetRegion).ToString();
		}
	}
}
