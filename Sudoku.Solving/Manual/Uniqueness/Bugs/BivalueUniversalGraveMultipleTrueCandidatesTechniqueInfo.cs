using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of BUG+n technique.
	/// </summary>
	public sealed class BivalueUniversalGraveMultipleTrueCandidatesTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// The table of extra difficulty values.
		/// </summary>
		private static readonly decimal[] DifficultyExtra = new[]
		{
			.1m, .2m, .2m, .3m, .3m, .3m, .4m, .4m, .4m, .4m,
			.5m, .5m, .5m, .5m, .5m, .6m, .6m, .6m
		};


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="candidates">All candidates.</param>
		public BivalueUniversalGraveMultipleTrueCandidatesTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> candidates)
			: base(conclusions, views) => Candidates = candidates;


		/// <summary>
		/// Indicates all candidates used.
		/// </summary>
		public IReadOnlyList<int> Candidates { get; }

		/// <inheritdoc/>
		public override string Name => $"Bivalue Universal Grave + {Candidates.Count}";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.7m + DifficultyExtra[Candidates.Count - 1];

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candsStr = CandidateCollection.ToString(Candidates);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: True candidates: {candsStr} => {elimStr}";
		}
	}
}
