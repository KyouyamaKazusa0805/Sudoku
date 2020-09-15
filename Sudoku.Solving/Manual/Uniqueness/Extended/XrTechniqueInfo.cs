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
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	public abstract record XrTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, GridMap Cells, short DigitsMask)
		: UniquenessTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		protected static readonly decimal[] DifficultyExtra = { 0, 0, 0, 0, .1M, 0, .2M, 0, .3M, 0, .4M, 0, .5M, 0, .6M };


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
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
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
