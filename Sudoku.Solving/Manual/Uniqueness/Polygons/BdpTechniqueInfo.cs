using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern</b> (BDP) technique.
	/// </summary>
	public abstract class BdpTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="map">The cells used.</param>
		protected BdpTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap map, short digitsMask)
			: base(conclusions, views) => (Map, DigitsMask) = (map, digitsMask);


		/// <summary>
		/// Indicates the cells used.
		/// </summary>
		public GridMap Map { get; }

		/// <summary>
		/// Indicates the digits used.
		/// </summary>
		public short DigitsMask { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;

		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public abstract override TechniqueCode TechniqueCode { get; }

		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
