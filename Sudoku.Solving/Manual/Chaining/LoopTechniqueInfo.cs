using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>(grouped) continuous nice loop</b> technique.
	/// </summary>
	public sealed class LoopTechniqueInfo : ChainingTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="xEnabled">Indicates whether the chain is enabled X strong relations.</param>
		/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
		/// <param name="target">The target.</param>
		public LoopTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool xEnabled, bool yEnabled, Node target)
			: base(conclusions, views, xEnabled, yEnabled, default, default, default, default) => Target = target;


		/// <summary>
		/// The target.
		/// </summary>
		public Node Target { get; }

		/// <inheritdoc/>
		public override decimal Difficulty =>
			(XEnabled && YEnabled ? 5.0M : 4.5M) + GetExtraDifficultyByLength(FlatComplexity - 2);

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.ContinuousNiceLoop;

		/// <inheritdoc/>
		public override int SortKey =>
			(XEnabled, YEnabled) switch
			{
				(true, true) => 4,
				(false, true) => 3,
				_ => 2
			};

		/// <inheritdoc/>
		public override int FlatComplexity => Target.AncestorsCount;


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Views[0].Links!).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
