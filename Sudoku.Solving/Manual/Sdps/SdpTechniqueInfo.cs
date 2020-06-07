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
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit">The digit.</param>
		protected SdpTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int digit)
			: base(conclusions, views) => Digit = digit;


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; protected set; }
	}
}
