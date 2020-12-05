using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Grid = Sudoku.Data.SudokuGrid;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>forcing chains (+)</b> (<b>FCs (+)</b>) technique searcher.
	/// </summary>
	public sealed class FcPlusTechniqueSearcher : FcTechniqueSearcher
	{
		/// <summary>
		/// Indicates the advanced searchers.
		/// </summary>
		private IList<TechniqueSearcher>? _advancedSearchers;


		/// <summary>
		/// Initializes an instance with the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		public FcPlusTechniqueSearcher(int level) : base(false, true, true) => _level = level;


		/// <inheritdoc cref="SearchingProperties"/>
		public static new TechniqueProperties Properties { get; } = new(80, nameof(TechniqueCode.DynamicRegionFc)) { DisplayLevel = 5 };


		/// <inheritdoc/>
		protected override IEnumerable<Node> Advanced(in Grid grid, in Grid source, Set<Node> offNodes)
		{
			IniitalizeTechniquesIfNeed();

			var result = new List<Node>();
			for (int i = 0; i < _advancedSearchers.Count && result.Count == 0; i++)
			{
				var searcher = _advancedSearchers[i];

				// TODO: Get all nodes.
			}
		}

		/// <summary>
		/// Initalizes technique searchers if need.
		/// </summary>
		[MemberNotNull(nameof(_advancedSearchers))]
		private void IniitalizeTechniquesIfNeed()
		{
			if (_advancedSearchers is null)
			{
				_advancedSearchers = new List<TechniqueSearcher>
				{
					new LcTechniqueSearcher(),
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new UrTechniqueSearcher(true, true)
				};

				switch (_level)
				{
					case >= 5: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcPlusTechniqueSearcher(_level - 3));
						goto case 4;
					}
					case 4: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcTechniqueSearcher(false, true, true));
						goto case 3;
					}
					case 3: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcTechniqueSearcher(false, true, false));
						goto case 2;
					}
					case 2:
					{
						_advancedSearchers.Add(new AicTechniqueSearcher());
						break;
					}
				}
			}
		}
	}
}
