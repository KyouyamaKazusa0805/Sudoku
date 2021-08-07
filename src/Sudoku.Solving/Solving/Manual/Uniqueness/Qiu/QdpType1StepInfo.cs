using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 1</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="Candidate">The candidate.</param>
	public sealed record QdpType1StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Pattern, int Candidate
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.QdpType1;

		[FormatItem]
		private string CandidateStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates { Candidate }.ToString();
		}


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: Cells {PatternStr} will be a deadly pattern if {CandidateStr} is false => {ElimStr}";
	}
}
