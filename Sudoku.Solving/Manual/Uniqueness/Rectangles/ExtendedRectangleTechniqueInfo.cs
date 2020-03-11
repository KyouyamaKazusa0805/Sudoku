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
			0, 0, 0, 0, .1M, 0, .2M, 0, .3M, 0, .4M, 0, .5M, 0, .6M
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
					1 => 4.5M,
					2 => 4.6M,
					3 => (((XrType3)DetailData).IsNaked ? 4.5M : 4.6M) + ((XrType3)DetailData).SubsetCells.Count * .1M,
					4 => 4.6M,
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
