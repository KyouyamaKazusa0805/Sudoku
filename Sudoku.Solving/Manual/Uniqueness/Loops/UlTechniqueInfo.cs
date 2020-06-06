using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop</b> (UL) technique.
	/// </summary>
	public abstract class UlTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = { 0, 0, .1M, .2M, .3M, .4M, .5M, .6M };


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		public UlTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int d1, int d2, GridMap loop)
			: base(conclusions, views) => (Digit1, Digit2, Loop) = (d1, d2, loop);


		/// <summary>
		/// Indicates the digit 1.
		/// </summary>
		public int Digit1 { get; }

		/// <summary>
		/// Indicates the digit 2.
		/// </summary>
		public int Digit2 { get; }

		/// <summary>
		/// Indicates the loop.
		/// </summary>
		public GridMap Loop { get; }

		/// <summary>
		/// Indicates the type.
		/// </summary>
		public abstract int Type { get; }

		/// <inheritdoc/>
		public sealed override string Name => $"Unique Loop Type {Type}";

		/// <inheritdoc/>
		public sealed override decimal Difficulty =>
			Type switch
			{
				1 => 4.5M,
				2 => 4.6M,
				3 => 4.5M + ((UlType3TechniqueInfo)this).SubsetCells.Count * .1M,
				4 => 4.6M,
				_ => throw Throwings.ImpossibleCase
			} + DifficultyExtra[Loop.Count >> 1];

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override TechniqueCode TechniqueCode =>
			Type switch
			{
				1 => TechniqueCode.UlType1,
				2 => TechniqueCode.UlType2,
				3 => TechniqueCode.UlType3,
				4 => TechniqueCode.UlType4,
				_ => throw Throwings.ImpossibleCase
			};


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
