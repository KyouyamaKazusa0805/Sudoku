#if DOUBLE_LAYERED_ASSUMPTION

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.Subsets;
using Grid = Sudoku.Data.SudokuGrid;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>forcing chains (+)</b> (<b>FCs (+)</b>) technique searcher.
	/// </summary>
	public sealed class FcPlusStepSearcher : FcStepSearcher
	{
		/// <summary>
		/// Indicates the advanced searchers.
		/// </summary>
		private IList<StepSearcher>? _advancedSearchers;


		/// <summary>
		/// Initializes an instance with the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		public FcPlusStepSearcher(int level) : base(false, true, true) => _level = level;


		/// <inheritdoc cref="SearchingProperties"/>
		public static new TechniqueProperties Properties { get; } = new(80, nameof(TechniqueCode.DynamicRegionFc))
		{
			OnlyEnableInAnalysis = true,
			DisplayLevel = 5
		};


		/// <inheritdoc/>
		protected override IEnumerable<Node> Advanced(in Grid grid, in Grid source, Set<Node> offNodes)
		{
			InitializeTechniquesIfNeed();

			var result = new List<Node>();
			for (int i = 0; i < _advancedSearchers.Count && result.Count == 0; i++)
			{
				var searcher = _advancedSearchers[i];
				var accumulator = new List<StepInfo>();
				searcher.GetAll(accumulator, grid);

				foreach (var info in accumulator)
				{
					var parents = ((IHasParentNodeInfo)info).GetRuleParents(source, grid);
					if (parents.Any())
					{
						// If no parent can be found, the rule probably already exists without the chain.
						// Therefore, it's useles to include it in the chain.
						var nested = info as ChainingStepInfo;
						foreach (var conclusion in info.Conclusions)
						{
							var toOff = new Node(conclusion.Cell, conclusion.Digit, false)
							{
								Cause = Cause.Advanced,
								//NestedHint = nested
							};

							foreach (var p in parents)
							{
								offNodes.TryGetValue(p, out var real);
								(toOff.Parents ??= new List<Node>()).Add(real);
							}

							result.Add(toOff);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Initalizes technique searchers if need.
		/// </summary>
		[MemberNotNull(nameof(_advancedSearchers))]
		private void InitializeTechniquesIfNeed()
		{
			if (_advancedSearchers is null)
			{
				_advancedSearchers = new List<StepSearcher>
				{
					new LcStepSearcher(),
					new SubsetStepSearcher(),
					new NormalFishStepSearcher()
				};

				switch (_level)
				{
					case >= 5: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcPlusStepSearcher(_level - 3));
						goto case 4;
					}
					case 4: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcStepSearcher(false, true, true));
						goto case 3;
					}
					case 3: /*fallthrough*/
					{
						_advancedSearchers.Add(new FcStepSearcher(false, true, false));
						goto case 2;
					}
					case 2:
					{
						_advancedSearchers.Add(new AicStepSearcher());
						break;
					}
				}
			}
		}
	}
}

#endif