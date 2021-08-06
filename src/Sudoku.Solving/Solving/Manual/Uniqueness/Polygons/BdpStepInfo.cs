using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	public abstract record BdpStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Map, short DigitsMask
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

		/// <inheritdoc/>
		public abstract override Technique TechniqueCode { get; }

		[FormatItem]
		private protected string DigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(DigitsMask).ToString();
		}

		[FormatItem]
		private protected string CellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Map.ToString();
		}

		[FormatItem]
		private protected string ElimStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ConclusionCollection(Conclusions).ToString();
		}


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
