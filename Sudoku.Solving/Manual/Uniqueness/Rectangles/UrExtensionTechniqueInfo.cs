using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle extension</b> (UR+) technique.
	/// </summary>
	public sealed class UrExtensionTechniqueInfo : RectangleTechniqueInfo
	{
		/// <inheritdoc/>
		public UrExtensionTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IRectangleDetailData detailData)
			: base(conclusions, views, detailData)
		{
		}


		/// <inheritdoc/>
		public override string Name => Convert(DetailData).Name;

		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + .1M * Convert(DetailData).ConjugatePairs.Count;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}

		/// <summary>
		/// Convert the specified detail data to UR+ data.
		/// </summary>
		/// <param name="rectangleDetailData">The data to convert.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UrExtensionDetailData Convert(IRectangleDetailData rectangleDetailData) =>
			(UrExtensionDetailData)rectangleDetailData;
	}
}
