using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using ArType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.AvoidableRectangleType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of avoidable rectangle (AR) technique.
	/// </summary>
	public sealed class AvoidableRectangleTechniqueInfo : RectangleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="detailData">The data of details.</param>
		public AvoidableRectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			AvoidableRectangleDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <inheritdoc/>
		public override string Name => $"Avoidable Rectangle (Type {DetailData.Type})";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 4.5m,
					2 => 4.7m,
					3 => (((ArType3)DetailData).IsNaked ? 4.6m : 4.7m) + ((ArType3)DetailData).SubsetDigits.Count * .1m,
					_ => throw new Exception("Impossible case.")
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <summary>
		/// The data of the specified avoidable rectangle type.
		/// </summary>
		public override IRectangleDetailData DetailData { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {DetailData} => {elimStr}";
		}
	}
}
