using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>alternating inference chain</b> (AIC) technique.
	/// </summary>
	public sealed class AicTechniqueInfo : ChainingTechniqueInfo
	{
		/// <summary>
		/// The complexity (cache).
		/// </summary>
		private int _complexity = -1;


		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="isX">Indicates whether the chain is a X-Chain.</param>
		/// <param name="isY">Indicates whether the chain is a Y-Chain.</param>
		/// <param name="isNishio">Indicates whether the chain is nishio.</param>
		/// <param name="isMultiple">Indicates whether the chain is multiple.</param>
		/// <param name="isDynamic">Indicates whether the chain is dynamic.</param>
		/// <param name="level">The dynamic level.</param>
		/// <param name="target">The target.</param>
		public AicTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool isX, bool isY,
			bool isNishio, bool isMultiple, bool isDynamic, int level, Node target)
			: base(conclusions, views, isX, isY, isNishio, isMultiple, isDynamic, level) => Target = target;


		/// <summary>
		/// Indicates the target of this chain.
		/// </summary>
		public Node Target { get; }

		/// <inheritdoc/>
		public override int FlatComplexity
		{
			get
			{
				if (_complexity < 0)
				{
					_complexity = GetAncestorCount(Target);
				}

				return _complexity;
			}
		}

		/// <inheritdoc/>
		public override int SortKey =>
			(_isX, _isY) switch
			{
				(true, true) => 4,
				(_, true) => 3,
				_ => 2
			};

		/// <inheritdoc/>
		public override decimal Difficulty => (_isX && _isY ? 7M : 6.6M) + LengthDifficulty;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			true switch
			{
				_ when _isX && _isY => TechniqueCode.Aic,
				_ when _isY => TechniqueCode.Aic,
				_ => GetAncestorCount(Target) == 6 ? TechniqueCode.TurbotFish : TechniqueCode.XChain
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		protected override int FlatViewCount => 1;

		/// <inheritdoc/>
		protected override Node Result => Target;

		/// <inheritdoc/>
		protected internal override ICollection<Node> ChainsTargets => new List<Node> { Target };


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Target}{(Target.IsOn ? " on" : " off")} => {elimStr}";
		}

		/// <inheritdoc/>
		public override string ToFullString()
		{
			var reverse = new Node(Target.Candidate, !Target.IsOn);
			string assumption = reverse.ToString(LinkType.Weak);
			string consequence = reverse.ToString(LinkType.Strong);
			string conclusion = Target.ToString(LinkType.Weak);
			string htmlChain = GetHtmlChain(Target);
			string result =
				$"If we assume that {assumption}, it follows, through an AIC, that {consequence}. " +
				$"Therefore we can conclude that {conclusion}. " +
				$"The details of the chain: {htmlChain}";
			string details = AppendNestedChainDetails(result);
			return details;
		}

		/// <summary>
		/// Get all links.
		/// </summary>
		/// <param name="viewNumber">The view number.</param>
		/// <returns>The links.</returns>
		public override ICollection<Link> GetLinks(int viewNumber) =>
			ViewCount >= FlatViewCount ? GetNestedLinks(viewNumber) : GetLinks(Target);

		/// <inheritdoc/>
		protected override SudokuMap GetGreenNodes(int nestedViewNumber)
		{
			if (ViewCount >= FlatViewCount)
			{
				return base.GetGreenNodes(nestedViewNumber);
			}

			var result = GetColorNodes(true);
			if (!Target.IsOn)
			{
				// Make the target orange.
				result.Remove(Target.Candidate);
			}

			return result;
		}

		/// <inheritdoc/>
		protected override SudokuMap GetOrangeNodes(int nestedViewNumber)
		{
			if (ViewCount >= FlatViewCount)
			{
				return base.GetOrangeNodes(nestedViewNumber);
			}

			var result = GetColorNodes(true);
			if (Target.IsOn)
			{
				// Make the target green.
				result.Remove(Target.Candidate);
			}

			return result;
		}

		/// <inheritdoc/>
		protected override Node GetChainTarget(int viewNumber) => Target;

		/// <summary>
		/// Get color nodes from the state.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>The map.</returns>
		private SudokuMap GetColorNodes(bool state) => GetColorNodes(Target, state, state);
	}
}
