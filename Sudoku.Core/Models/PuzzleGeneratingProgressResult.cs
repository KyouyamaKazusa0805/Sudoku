#pragma warning disable CA1815

using Sudoku.Windows;

namespace Sudoku.Models
{
	/// <summary>
	/// Indicates a puzzle generating progress result.
	/// </summary>
	public struct PuzzleGeneratingProgressResult : IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified trial times and globalization string.
		/// </summary>
		/// <param name="generatingTrial">The number of the trial times.</param>
		/// <param name="globalizationString">The globalization string.</param>
		public PuzzleGeneratingProgressResult(int generatingTrial, string globalizationString) : this() =>
			(GeneratingTrial, GlobalizationString) = (generatingTrial, globalizationString);


		/// <summary>
		/// Indicates how many trials of the specified generator generates.
		/// </summary>
		public int GeneratingTrial { readonly get; set; }

		/// <inheritdoc/>
		public readonly double Percentage => 0;

		/// <inheritdoc/>
		public readonly string GlobalizationString { get; }


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() =>
			GeneratingTrial == 1
				? Resources.GetValue("GeneratingProgressSingular")
				: $"{GeneratingTrial} {Resources.GetValue("GeneratingProgressPlural")}";
	}
}
