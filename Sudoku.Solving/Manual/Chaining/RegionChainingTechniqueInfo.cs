using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>region forcing chains</b> technique.
	/// </summary>
	public sealed class RegionChainingTechniqueInfo : ChainingTechniqueInfo
	{
		/// <include file = 'SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="region">The region.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="chains">All branches.</param>
		public RegionChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int region, int digit, IReadOnlyDictionary<int, Node> chains)
			: base(conclusions, views, default, default, default, true, default, default) =>
			(Region, Digit, Chains) = (region, digit, chains);


		/// <summary>
		/// Indicates the region.
		/// </summary>
		public int Region { get; }
		
		/// <summary>
		/// Indicates the digit used.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates all branches.
		/// </summary>
		public IReadOnlyDictionary<int, Node> Chains { get; }

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override int SortKey => 6;

		/// <inheritdoc/>
		public override int FlatComplexity
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
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string regionStr = new RegionCollection(Region).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: It can be proved using chains that digit {Digit + 1} from {regionStr} are false" +
				$" => {elimStr}";
		}
	}
}
