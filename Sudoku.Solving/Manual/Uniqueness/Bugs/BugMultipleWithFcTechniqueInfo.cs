using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>BUG + n with forcing chains</b> technique.
	/// </summary>
	public sealed class BugMultipleWithFcTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="candidates">All candidates.</param>
		/// <param name="chains">The sub-chains.</param>
		public BugMultipleWithFcTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, IReadOnlyList<int> candidates,
			IReadOnlyDictionary<int, Node> chains) : base(conclusions, views) =>
			(Candidates, Chains) = (candidates, chains);


		/// <summary>
		/// The true candidates.
		/// </summary>
		public IReadOnlyList<int> Candidates { get; }

		/// <summary>
		/// All sub-chains.
		/// </summary>
		public IReadOnlyDictionary<int, Node> Chains { get; }

		/// <summary>
		/// The difficulty for the number of true candidates.
		/// </summary>
		public decimal CountDifficulty => Math.Floor((decimal)Math.Sqrt(2 * Candidates.Count + .5)) / 10;

		/// <summary>
		/// The length difficluty.
		/// </summary>
		public decimal LengthDifficulty
		{
			get
			{
				decimal result = 0;
				int ceil = 4;
				int length = Complexity - 2;
				for (bool isOdd = false; length > ceil; isOdd.Flip())
				{
					result += .1M;
					ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
				}

				return result;
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M + CountDifficulty + LengthDifficulty;

		/// <summary>
		/// The total length of all sub-chains gathered.
		/// </summary>
		public int Complexity
		{
			get
			{
				int result = 0;
				foreach (var node in Chains.Values)
				{
					result += node.AncestorsCount;
				}

				return result;
			}
		}

		/// <inheritdoc/>
		public override string Name => $"{Resources.GetValue("Bug")} + {Candidates.Count} (+)";

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugMultipleFc;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Candidates.Count < 6 ? DifficultyLevel.Fiendish : DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candsStr = new CandidateCollection(Candidates).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: True candidates: {candsStr} => {elimStr}";
		}
	}
}
