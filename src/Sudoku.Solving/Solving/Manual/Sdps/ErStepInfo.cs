using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>empty rectangle</b> (<b>ER</b>) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="Block">The block that the empty rectangle lies in.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	public sealed record ErStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit, int Block, in ConjugatePair ConjugatePair
	) : SdpStepInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.EmptyRectangle;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.EmptyRectangle;

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
			get => new RegionCollection(Block).ToString();
		}

		[FormatItem]
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}
	}
}
