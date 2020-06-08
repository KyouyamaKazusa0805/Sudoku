using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 4 technique.
	/// </summary>
	public sealed class XrType4TechniqueInfo : XrTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="extraMask">The extra mask.</param>
		/// <param name="conjugateRegion">The conjugate region.</param>
		public XrType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, short extraMask,
			GridMap conjugateRegion) : base(conclusions, views, cells, digits) =>
			(ExtraMask, ConjugateRegion) = (extraMask, conjugateRegion);


		/// <summary>
		/// Indicates the mask of digits that is the combination.
		/// </summary>
		public short ExtraMask { get; }

		/// <summary>
		/// The so-called conjugate region.
		/// </summary>
		public GridMap ConjugateRegion { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 4.6M + DifficultyExtra[Cells.Count];

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.XrType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			string combStr = new DigitCollection(ExtraMask.GetAllSets()).ToString();
			string conjRegion = new CellCollection(ConjugateRegion).ToString();
			return $"the conjugate region {conjRegion} of the extra digits {combStr}";
		}
	}
}
