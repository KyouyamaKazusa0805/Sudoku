using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public sealed partial class ChainingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicats whether the searcher will search for nishio forcing chains.
		/// </summary>
		private readonly bool _withNishio;

		/// <summary>
		/// Indicating whether the searcher will search for multiple forcing chains.
		/// </summary>
		private readonly bool _withMultiple;

		/// <summary>
		/// Indicating whether the searcher will search for dynamic forcing chains.
		/// </summary>
		private readonly bool _withDynamic;

		/// <summary>
		/// The dynamic level. <c>0</c> is for no dynamic.
		/// </summary>
		private readonly int _level;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="withNishio">
		/// A <see cref="bool"/> value indicating whether the searcher will search for nishio forcing chains.
		/// </param>
		/// <param name="withMultiple">
		/// A <see cref="bool"/> value indicating whether the searcher will search for multiple forcing chains.
		/// </param>
		/// <param name="withDynamic">
		/// A <see cref="bool"/> value indicating whether the searcher will search for dynamic forcing chains.
		/// </param>
		/// <param name="level">The dynamic level. <c>0</c> is for no dynamic.</param>
		public ChainingTechniqueSearcher(bool withNishio, bool withMultiple, bool withDynamic, int level) =>
			(_withNishio, _withMultiple, _withDynamic, _level) = (withNishio, withMultiple, withDynamic, level);


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
		}
	}
}
