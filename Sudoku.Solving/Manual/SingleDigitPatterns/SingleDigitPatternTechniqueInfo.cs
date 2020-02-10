using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.SingleDigitPatterns
{
	/// <summary>
	/// Provides a usage of single-digit pattern technique.
	/// </summary>
	public abstract class SingleDigitPatternTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit">The digit.</param>
		protected SingleDigitPatternTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit)
			: base(conclusions, views) => Digit = digit;


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; protected set; }
	}
}
