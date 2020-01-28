using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of unique rectangle technique.
	/// </summary>
	public sealed class UniqueRectangleTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="detailData">The data of details.</param>
		public UniqueRectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			UniqueRectangleDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <inheritdoc/>
		public override string Name =>
			$"Unique Rectangle Type {DetailData.Type}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 4.5m,
					2 => 4.6m,
					3 => throw new NotImplementedException("I have no Type 3 detail data class."),
					4 => 4.6m,
					5 => 4.6m,
					6 => 4.7m,
					7 => 4.8m,
					8 => 4.9m,
					9 => 5m,
					_ => throw new NotSupportedException($"The specified {nameof(DetailData.Type)} is out of range.")
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;

		/// <summary>
		/// The data of the specified unique rectangle type.
		/// </summary>
		public UniqueRectangleDetailData DetailData { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
