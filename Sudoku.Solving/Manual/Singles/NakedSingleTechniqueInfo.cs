using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>naked single</b> technique.
	/// </summary>
	public sealed class NakedSingleTechniqueInfo : SingleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public NakedSingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int cellOffset, int digit)
			: base(conclusions, views, cellOffset, digit)
		{ 
		}


		/// <inheritdoc/>
		public override string Name => "Naked single";

		/// <inheritdoc/>
		public override decimal Difficulty => 2.3M;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new CellCollection(stackalloc[] { Cell }).ToString();
			int value = Digit + 1;
			return $"{Name}: {cellStr} = {value}";
		}
	}
}
