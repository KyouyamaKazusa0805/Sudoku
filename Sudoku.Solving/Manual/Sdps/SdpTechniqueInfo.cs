using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>single-digit pattern</b> (SDP) technique.
	/// </summary>
	public abstract class SdpTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit">The digit.</param>
		protected SdpTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int digit)
			: base(conclusions, views) => Digit = digit;


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; protected set; }
	}
}
