using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of Borescoper's deadly pattern (BDP) technique.
	/// </summary>
	public sealed class BorescoperDeadlyPatternTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="detailData">The detail data.</param>
		public BorescoperDeadlyPatternTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			BorescoperDeadlyPatternDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <summary>
		/// The detail data of the technique.
		/// </summary>
		public BorescoperDeadlyPatternDetailData DetailData { get; }

		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => DetailData.Digits.Count;

		/// <inheritdoc/>
		public override string Name =>
			$"Borescoper's Deadly Pattern {Size} Digits (Type {DetailData.Type})";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 5.3m,
					2 => 5.4m,
					4 => 5.4m,
					_ => throw new NotSupportedException("Out of range.")
				} + (Size != 3 ? .1m : 0);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
