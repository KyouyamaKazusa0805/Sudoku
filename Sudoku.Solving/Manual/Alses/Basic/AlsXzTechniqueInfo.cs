using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XZ rule</b>
	/// or <b>extended subset principle</b> technique.
	/// </summary>
	public sealed class AlsXzTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="als1">The ALS 1 used.</param>
		/// <param name="als2">The ALS 2 used.</param>
		/// <param name="commonDigitMask">The common digit mask.</param>
		public AlsXzTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Als als1,
			Als als2, short commonDigitMask) : base(conclusions, views) =>
			(Als1, Als2, CommonDigitMask) = (als1, als2, commonDigitMask);


		/// <summary>
		/// The ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// The ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// The common digit.
		/// </summary>
		public short CommonDigitMask { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				return Als1.IsBivalueCell || Als2.IsBivalueCell
					? "Extended Subset Principle"
					: "Almost Locked Sets XZ rule";
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string xStr = new DigitCollection(CommonDigitMask.GetAllSets()).ToString();
			return $"{Name}: ALS 1: {Als1}, ALS 2: {Als2}, x = {xStr} => {elimStr}";
		}
	}
}
