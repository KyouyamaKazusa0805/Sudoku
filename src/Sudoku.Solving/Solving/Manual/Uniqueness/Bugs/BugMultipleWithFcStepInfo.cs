using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>BUG + n with forcing chains</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Candidates">All candidates.</param>
	/// <param name="Chains">The sub-chains.</param>
	public sealed record BugMultipleWithFcStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<int> Candidates,
		IReadOnlyDictionary<int, Node> Chains
	) : BugStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty for the number of true candidates.
		/// </summary>
		public decimal CountDifficulty => Math.Floor((decimal)Math.Sqrt(2 * Candidates.Count + .5)) / 10;

		/// <summary>
		/// The length difficulty.
		/// </summary>
		public decimal LengthDifficulty
		{
			get
			{
				decimal result = 0;
				int ceil = 4;
				int length = Complexity - 2;
				for (bool isOdd = false; length > ceil; isOdd = !isOdd)
				{
					result += .1M;
					ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
				}

				return result;
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty - .1M + CountDifficulty + LengthDifficulty;

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
		public override string Name => $"{TextResources.Current.Bug} + {Candidates.Count.ToString()} (+)";

		/// <inheritdoc/>
		public override string? Acronym => $"BUG + {Candidates.Count.ToString()} (+)";

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugMultipleFc;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Candidates.Count < 6 ? DifficultyLevel.Fiendish : DifficultyLevel.Nightmare;

		[FormatItem]
		private string CandidatesStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates(Candidates).ToString();
		}

		[FormatItem]
		private string ElimStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ConclusionCollection(Conclusions).ToString();
		}


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: True candidates: {CandidatesStr} => {ElimStr}";
	}
}
