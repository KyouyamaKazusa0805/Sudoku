using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern</b> (QDP) technique.
	/// </summary>
	public abstract class QdpTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		public QdpTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern)
			: base(conclusions, views) => Pattern = pattern;


		/// <summary>
		/// Indicates the pattern.
		/// </summary>
		public Pattern Pattern { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.8M;

		/// <inheritdoc/>
		public abstract override TechniqueCode TechniqueCode { get; }

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
