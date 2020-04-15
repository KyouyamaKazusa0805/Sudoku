using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using ArType3 = Sudoku.Solving.Manual.Uniqueness.Rectangles.ArType3DetailData;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	[Obsolete]
	public sealed class ArTechniqueInfo : RectangleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="detailData">The data of details.</param>
		public ArTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IRectangleDetailData detailData)
			: base(conclusions, views, detailData)
		{ 
		}


		/// <inheritdoc/>
		public override string Name => $"Avoidable Rectangle (Type {DetailData.Type})";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return DetailData.Type switch
				{
					1 => 4.5M,
					2 => 4.7M,
					3 => (((ArType3)DetailData).IsNaked ? 4.6M : 4.7M) + ((ArType3)DetailData).SubsetDigits.Count * .1M,
					_ => throw Throwing.ImpossibleCase
				};
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
