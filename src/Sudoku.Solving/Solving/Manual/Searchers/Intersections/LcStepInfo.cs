using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>locked candidates</b> (LC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="BaseSet">The base region.</param>
	/// <param name="CoverSet">The cover region.</param>
	public sealed record LcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit, int BaseSet, int CoverSet
	) : IntersectionStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseSet < 9 ? 2.6M : 2.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		/// <inheritdoc/>
		public override Technique TechniqueCode => BaseSet < 9 ? Technique.Pointing : Technique.Claiming;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Lc;

		[FormatItem]
		private string DigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit + 1).ToString();
		}

		[FormatItem]
		private string BaseSetStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(BaseSet).ToString();
		}

		[FormatItem]
		private string CoverSetStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(CoverSet).ToString();
		}
	}
}
