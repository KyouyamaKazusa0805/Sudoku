using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

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
		/// <param name="conjugatePair">The conjugate pair.</param>
		public XrType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			GridMap cells, short digits, ConjugatePair conjugatePair) : base(conclusions, views, cells, digits) =>
			ConjugatePair = conjugatePair;


		/// <summary>
		/// The conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 4.6M + DifficultyExtra[Cells.Count];

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.XrType4;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected override string? GetAdditional() => $"the conjugate pair {ConjugatePair}";
	}
}
