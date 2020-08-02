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
		public int GeneratingTrial { get; }

		/// <inheritdoc/>
		public double Percentage => 0;

		/// <inheritdoc/>
		public readonly string GlobalizationString { get; }


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() =>
			GeneratingTrial == 1
				? Resources.GetValue("GeneratingProgressSingular")
				: $"{GeneratingTrial} {Resources.GetValue("GeneratingProgressPlural")}";
	}
}
