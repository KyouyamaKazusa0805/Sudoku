using System.Collections.Generic;
using Sudoku.CodeGenerating;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	/// <summary>
	/// Provides a usage of <b>reverse bi-value universal grave</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">All cells used.</param>
	/// <param name="Digit1">Indicates the digit 1.</param>
	/// <param name="Digit2">Indicates the digit 2.</param>
	[AutoHashCode(nameof(Loop), nameof(Digit1), nameof(Digit2), nameof(TechniqueCode))]
	public abstract partial record ReverseBugStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop, int Digit1, int Digit2
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.ReverseBug;


		/// <inheritdoc/>
		public virtual bool Equals(ReverseBugStepInfo? obj) =>
			obj is not null && Loop == obj.Loop
			&& Digit1 == obj.Digit1 && Digit2 == obj.Digit2 && TechniqueCode == obj.TechniqueCode;
	}
}
