using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern locked type</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	/// <param name="Candidates">The candidates.</param>
	public sealed record QdpLockedTypeStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Pattern Pattern, IReadOnlyList<int> Candidates
	) : QdpStepInfo(Conclusions, Views, Pattern)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .2M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.LockedQdp;

		[FormatItem]
		private string CandidateStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates(Candidates).ToString();
		}

		[FormatItem]
		private string Quantifier
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };
		}

		[FormatItem]
		private string Number
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Candidates.Count == 1 ? " the" : $" {Candidates.Count.ToString()}";
		}

		[FormatItem]
		private string SingularOrPlural
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Candidates.Count == 1 ? "candidate" : "candidates";
		}

		[FormatItem]
		private string BeVerb
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Candidates.Count == 1 ? "is" : "are";
		}
	}
}
