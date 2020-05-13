using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using UlType3 = Sudoku.Solving.Manual.Uniqueness.Loops.UlType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop</b> (UL) technique.
	/// </summary>
	public sealed class UlTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = { 0, 0, 0, 0, .1M, 0, .2M, 0, .3M, 0, .4M, 0, .5M, 0, .6M };


		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="detailData">The data of details.</param>
		public UlTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, UlDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <inheritdoc/>
		public override string Name =>
			$"Unique {(DetailData.Cells.Count == 4 ? "Rectangle" : "Loop")} Type {DetailData.Type}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				UlType3 data() => (UlType3)DetailData;
				return DetailData.Type switch
				{
					1 => 4.5M,
					2 => 4.6M,
					3 => (data().IsNaked ? 4.5M : 4.6M) + data().SubsetCells.Count * .1M,
					4 => 4.6M,
					_ => throw new NotSupportedException($"The specified {nameof(DetailData.Type)} is out of range.")
				} + DifficultyExtra[DetailData.Cells.Count];
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode
		{
			get
			{
				return DetailData.Type switch
				{
					1 => TechniqueCode.UlType1,
					2 => TechniqueCode.UlType2,
					3 => TechniqueCode.UlType3,
					4 => TechniqueCode.UlType4,
					_ => throw Throwing.ImpossibleCase
				};
			}
		}

		/// <summary>
		/// The data of the specified unique rectangle type.
		/// </summary>
		public UlDetailData DetailData { get; }

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo other) =>
			other is UlTechniqueInfo comparer && DetailData.Equals(comparer.DetailData);

		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
