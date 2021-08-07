using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern</b> (QDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pattern">The pattern.</param>
	public abstract record QdpStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Pattern
	) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.8M;

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public abstract override Technique TechniqueCode { get; }

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

		/// <summary>
		/// Indicates the pattern string.
		/// </summary>
		[FormatItem]
		protected string PatternStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Pattern.FullMap.ToString();
		}


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
