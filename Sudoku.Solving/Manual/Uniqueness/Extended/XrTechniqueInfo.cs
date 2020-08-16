using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) technique.
	/// </summary>
	public abstract class XrTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		protected static readonly decimal[] DifficultyExtra = { 0, 0, 0, 0, .1M, 0, .2M, 0, .3M, 0, .4M, 0, .5M, 0, .6M };

		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		public XrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			GridMap cells, short digits) : base(conclusions, views) =>
			(Cells, Digits) = (cells, digits);


		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public GridMap Cells { get; }

		/// <summary>
		/// Indicates all digits.
		/// </summary>
		public short Digits { get; }

		/// <summary>
		/// Indicates the size of the instance.
		/// </summary>
		public int Size => Cells.Count >> 1;

		/// <inheritdoc/>
		public abstract override TechniqueCode TechniqueCode { get; }

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: {digitsStr} in {cellsStr}{(additional is null ? string.Empty : $" with {additional}")} => " +
				$"{elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();
	}
}
