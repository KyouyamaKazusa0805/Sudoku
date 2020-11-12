using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop</b> (UL) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	public abstract record UlTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit1, int Digit2,
		in GridMap Loop) : UniquenessTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = { 0, 0, .1M, .2M, .3M, .4M, .5M, .6M };


		/// <summary>
		/// Indicates the type.
		/// </summary>
		public abstract int Type { get; }

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
		public sealed override string Name => base.Name;

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

		/// <inheritdoc/>
		public virtual bool Equals(UlTechniqueInfo? other) =>
			other is not null
			&& (Type, Loop) == (other.Type, other.Loop)
			&& (
			(Digit1, Digit2) == (other.Digit1, other.Digit2)
			|| (Digit1, Digit2) == (other.Digit2, other.Digit1));

		/// <inheritdoc/>
		public override int GetHashCode() => HashCode.Combine(Type, Loop, Digit1, Digit2);
	}
}
