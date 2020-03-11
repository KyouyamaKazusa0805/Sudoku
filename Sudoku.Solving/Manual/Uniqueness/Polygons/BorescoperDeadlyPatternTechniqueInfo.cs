using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using BdpType3 = Sudoku.Solving.Manual.Uniqueness.Polygons.BorescoperDeadlyPatternType3DetailData;

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
					1 => 5.3M,
					2 => 5.4M,
					3 => (((BdpType3)DetailData).IsNaked ? 5.3M : 5.4M) + ((BdpType3)DetailData).Digits.Count * .1M,
					4 => 5.4M,
					_ => throw new NotSupportedException("Out of range.")
				} + (Size != 3 ? .1M : 0);
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
