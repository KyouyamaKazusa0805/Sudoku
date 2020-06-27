using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	public abstract class ChainingTechniqueInfo : TechniqueInfo, IEquatable<ChainingTechniqueInfo?>
	{
		/// <inheritdoc/>
		protected ChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views) : base(conclusions, views)
		{
		}


		/// <inheritdoc/>
		public bool Equals(ChainingTechniqueInfo? other)
		{
			throw new NotImplementedException();
		}
	}
}
