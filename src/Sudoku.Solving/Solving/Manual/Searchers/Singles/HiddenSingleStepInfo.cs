using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a using of <b>hidden single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Region">The region.</param>
	/// <param name="EnableAndIsLastDigit">Indicates whether the current technique is a last digit.</param>
	public sealed record HiddenSingleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Cell, int Digit, int Region, bool EnableAndIsLastDigit
	) : SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => EnableAndIsLastDigit ? 1.1M : Region < 9 ? 1.2M : 1.5M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => EnableAndIsLastDigit
			? Technique.LastDigit
			: (Technique)((int)Technique.HiddenSingleBlock + (int)Region.ToLabel());

		/// <inheritdoc/>
		public override string Format => EnableAndIsLastDigit
			? TextResources.Current.Format_HiddenSingleStepInfo_1
			: TextResources.Current.Format_HiddenSingleStepInfo_2;

		[FormatItem]
		private string CellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { Cell }.ToString();
		}

		[FormatItem]
		private string DigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit + 1).ToString();
		}

		[FormatItem]
		private string RegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(Region).ToString();
		}
	}
}
