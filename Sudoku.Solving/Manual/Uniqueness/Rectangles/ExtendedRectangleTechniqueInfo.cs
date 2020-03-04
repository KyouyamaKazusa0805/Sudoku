using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using XrType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.ExtendedRectangleType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of extended rectangle (XR) technique.
	/// </summary>
	public sealed class ExtendedRectangleTechniqueInfo : RectangleTechniqueInfo
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = new[]
		{
			0, 0, 0, 0, .1m, 0, .2m, 0, .3m, 0, .4m, 0, .5m, 0, .6m
		};


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="detailData">The detail data.</param>
		public ExtendedRectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IRectangleDetailData detailData)
			: base(conclusions, views, detailData)
		{
		}


		/// <summary>
		/// Indicates the size of the instance.
		/// </summary>
		public int Size => DetailData.Cells.Count >> 1;

		/// <inheritdoc/>
		public override string Name => $"Extended Rectangle Type {DetailData.Type}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 4.5m,
					2 => 4.6m,
					3 => (((XrType3)DetailData).IsNaked ? 4.5m : 4.6m) + ((XrType3)DetailData).SubsetCells.Count * .1m,
					4 => 4.6m,
					_ => throw new NotSupportedException($"The specified {nameof(DetailData.Type)} is out of range.")
				} + DifficultyExtra[DetailData.Cells.Count];
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
