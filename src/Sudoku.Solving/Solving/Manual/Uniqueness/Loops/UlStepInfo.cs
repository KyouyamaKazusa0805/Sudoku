using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.CodeGenerating;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

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
	[AutoHashCode(nameof(Type), nameof(Digit1), nameof(Digit2), nameof(Loop))]
	public abstract partial record UlStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, in Cells Loop
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty extra.
		/// </summary>
		private static readonly decimal[] ExtraDifficulty = { 0, 0, .1M, .2M, .3M, .4M, .5M, .6M };


		/// <summary>
		/// Indicates the type.
		/// </summary>
		public abstract int Type { get; }

		/// <inheritdoc/>
		public sealed override decimal Difficulty => BaseDifficulty + ExtraDifficulty[Loop.Count >> 1];

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override string? Acronym => "UL";

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"UlType{Type.ToString()}");

		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Ul;

		/// <summary>
		/// Indicates the loop string.
		/// </summary>
		[FormatItem]
		protected string LoopStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Loop.ToString();
		}

		/// <summary>
		/// Indicates the digit 1 string.
		/// </summary>
		[FormatItem]
		protected string Digit1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit1 + 1).ToString();
		}

		/// <summary>
		/// Indicates the digit 2 string.
		/// </summary>
		[FormatItem]
		protected string Digit2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit2 + 1).ToString();
		}

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Type switch
		{
			1 => 4.5M,
			2 => 4.6M,
			3 => 4.5M + ((UlType3StepInfo)this).SubsetCells.Count * .1M,
			4 => 4.6M
		};


		/// <inheritdoc/>
		public abstract override string ToString();

		/// <inheritdoc/>
		public virtual bool Equals(UlStepInfo? other) =>
			other is not null
			&& Type == other.Type && Loop == other.Loop
			&& (
				Digit1 == other.Digit1 && Digit2 == other.Digit2
				|| Digit1 == other.Digit2 && Digit2 == other.Digit1
			);
	}
}
