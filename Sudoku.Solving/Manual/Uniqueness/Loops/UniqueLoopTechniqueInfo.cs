using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using UlType3 = Sudoku.Solving.Manual.Uniqueness.Loops.UniqueLoopType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of unique loop technique.
	/// </summary>
	public sealed class UniqueLoopTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = new[]
		{
			0, 0, 0, 0, .1m, 0, .2m, 0, .3m, 0, .4m, 0, .5m, 0, .6m
		};


		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="detailData">The data of details.</param>
		public UniqueLoopTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			UniqueLoopDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <inheritdoc/>
		public override string Name => $"Unique Loop (Type {DetailData.Type})";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 4.5m,
					2 => 4.6m,
					3 => (((UlType3)DetailData).IsNaked ? 4.5m : 4.6m) + ((UlType3)DetailData).SubsetCells.Count * 0.1m,
					4 => 4.6m,
					_ => throw new NotSupportedException($"The specified {nameof(DetailData.Type)} is out of range.")
				} + DifficultyExtra[DetailData.Cells.Count];
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;

		/// <summary>
		/// The data of the specified unique rectangle type.
		/// </summary>
		public UniqueLoopDetailData DetailData { get; }

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo other)
		{
			return other is UniqueLoopTechniqueInfo comparer
				&& DetailData.Equals(comparer.DetailData);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
