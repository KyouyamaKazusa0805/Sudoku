using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using BdpType3 = Sudoku.Solving.Manual.Uniqueness.Polygons.BdpType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern</b> (BDP) technique.
	/// </summary>
	public sealed class BdpTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="detailData">The detail data.</param>
		public BdpTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, BdpDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <summary>
		/// The detail data of the technique.
		/// </summary>
		public BdpDetailData DetailData { get; }

		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => DetailData.Digits.Count;

		/// <inheritdoc/>
		public override string Name => $"Borescoper's Deadly Pattern {Size} Digits (Type {DetailData.Type})";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				BdpType3 d() => (BdpType3)DetailData;

				return DetailData.Type switch
				{
					1 => 5.3M,
					2 => 5.4M,
					3 => (d().IsNaked ? 5.3M : 5.4M) + d().Digits.Count * .1M,
					4 => 5.4M,
					_ => throw new NotSupportedException("Out of range.")
				} + (Size != 3 ? .1M : 0);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			DetailData.Type switch
			{
				1 => TechniqueCode.BdpType1,
				2 => TechniqueCode.BdpType2,
				3 => TechniqueCode.BdpType3,
				4 => TechniqueCode.BdpType4,
				_ => throw Throwing.ImpossibleCase
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
