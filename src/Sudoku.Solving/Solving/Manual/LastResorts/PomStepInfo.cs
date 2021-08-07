using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>pattern overlay method</b> (POM) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record PomStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: LastResortStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override decimal Difficulty => 8.5M;

		/// <inheritdoc/>
		public override string? Acronym => "POM";

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Pom;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Pom;

		[FormatItem]
		private string DigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit + 1).ToString();
		}


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: Digit {DigitStr} => {ElimStr}";
	}
}
