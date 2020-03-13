using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Solving.Utils.RegionUtils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an (grouped) alternating inference chain (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This technique searcher may use the basic searching way to find all AICs.
	/// For example, this searcher will try to search for all strong inferences firstly,
	/// and then search a weak inference that the candidate is in the same region or cell
	/// with a node in the strong inference in order to link them.
	/// </para>
	/// <para>
	/// Note that AIC may be static chains, which means that the searcher may just use
	/// static analysis is fine, which is different with dynamic chains.
	/// </para>
	/// </remarks>
	[Slow]
	public sealed class GroupedAlternatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// Indicates the last index of the collection.
		/// </summary>
		private static readonly Index LastIndex = ^1;


		/// <summary>
		/// Indicates whether the searcher will search for X-Chains.
		/// </summary>
		private readonly bool _searchX;

		/// <summary>
		/// Indicates whether the searcher will search for Y-Chains.
		/// Here Y-Chains means for multi-digit AICs.
		/// </summary>
		private readonly bool _searchY;

		/// <summary>
		/// Indicates whether the searcher will check locked candidates nodes.
		/// </summary>
		private readonly bool _checkLockedCandidatesNodes;

		/// <summary>
		/// Indicates whether the searcher will reduct same AICs
		/// which has same head and tail nodes.
		/// </summary>
		private readonly bool _reductDifferentPathAic;

		/// <summary>
		/// Indicates whether the searcher will store the shortest path only.
		/// </summary>
		private readonly bool _onlySaveShortestPathAic;

		/// <summary>
		/// Indicates whether the searcher will store the discontinuous nice loop
		/// whose head and tail is a same node.
		/// </summary>
		private readonly bool _addHeadCollision;

		/// <summary>
		/// Indicates whether the searcher will check the chain forms a continuous
		/// nice loop.
		/// </summary>
		private readonly bool _checkContinuousNiceLoop;

		/// <summary>
		/// Indicates the maximum length to search.
		/// </summary>
		private readonly int _maxLength;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="searchX">Indicates searching X-Chains or not.</param>
		/// <param name="searchY">Indicates searching Y-Chains or not.</param>
		/// <param name="checkLockedCandidatesNodes">
		/// Indicates whether the searcher will check the locked candidates nodes.
		/// </param>
		/// <param name="maxLength">
		/// Indicates the maximum length of a chain to search.
		/// </param>
		/// <param name="reductDifferentPathAic">
		/// Indicates whether the searcher will reduct same AICs
		/// which has same head and tail nodes.
		/// </param>
		/// <param name="onlySaveShortestPathAic">
		/// Indicates whether the searcher will store the shortest path
		/// only.
		/// </param>
		/// <param name="addHeadCollision">
		/// Indicates whether the searcher will store the discontinuous nice loop
		/// whose head and tail is a same node.
		/// </param>
		/// <param name="checkContinuousNiceLoop">
		/// Indicates whether the searcher will check the chain forms a continuous
		/// nice loop.
		/// </param>
		public GroupedAlternatingInferenceChainTechniqueSearcher(
			bool searchX, bool searchY, bool checkLockedCandidatesNodes,
			int maxLength, bool reductDifferentPathAic, bool onlySaveShortestPathAic,
			bool addHeadCollision, bool checkContinuousNiceLoop)
		{
			_searchX = searchX;
			_searchY = searchY;
			_checkLockedCandidatesNodes = checkLockedCandidatesNodes;
			_maxLength = maxLength;
			_reductDifferentPathAic = reductDifferentPathAic;
			_onlySaveShortestPathAic = onlySaveShortestPathAic;
			_addHeadCollision = addHeadCollision;
			_checkContinuousNiceLoop = checkContinuousNiceLoop;
		}


		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{

		}

		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All strong relations.</returns>
		private IReadOnlyList<Inference> GetAllStrongInferences(IReadOnlyGrid grid)
		{
			var result = new List<Inference>();
			if (_searchX)
			{
				for (int region = 0; region < 27; region++)
				{
					for (int digit = 0; digit < 9; digit++)
					{
						if (!grid.IsBilocationRegion(digit, region, out short mask))
						{
							continue;
						}

						int pos1 = mask.FindFirstSet();
						result.Add(
							new Inference(
								new CandidateNode(
									GetCellOffset(region, pos1) * 9 + digit),
								false,
								new CandidateNode(
									GetCellOffset(region, mask.GetNextSetBit(pos1)) * 9 + digit),
								true));
					}
				}
			}

			if (_searchY)
			{
				for (int cell = 0; cell < 81; cell++)
				{
					if (!grid.IsBivalueCell(cell, out short mask))
					{
						continue;
					}

					int digit1 = mask.FindFirstSet();
					result.Add(
						new Inference(
							new CandidateNode(cell * 9 + digit1), false,
							new CandidateNode(cell * 9 + mask.GetNextSetBit(digit1)), true));
				}
			}

			if (_checkLockedCandidatesNodes)
			{

			}

			return result;
		}
	}
}
