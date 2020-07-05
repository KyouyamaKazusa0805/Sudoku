using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;
using static Sudoku.Solving.Annotations.TechniqueDisplayAttribute;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>(grouped) alternating inference chain</b> technique.
	/// </summary>
	public sealed class AicTechniqueInfo : ChainingTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="xEnabled">Indicates whether the chain is enabled X strong relations.</param>
		/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
		/// <param name="target">The target.</param>
		public AicTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool xEnabled, bool yEnabled, Node target)
			: base(conclusions, views, xEnabled, yEnabled, default, default, default, default) => Target = target;


		/// <summary>
		/// The target node.
		/// </summary>
		public Node Target { get; }

		/// <inheritdoc/>
		public override decimal Difficulty =>
			(XEnabled && YEnabled ? 7.0M : 6.6M) + GetExtraDifficultyByLength(FlatComplexity - 2);

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
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			(Target.Chain[^2].Digit == Target.Digit) switch
			{
				true => TechniqueCode.Aic,
				false => Conclusions.Count switch
				{
					1 => TechniqueCode.DiscontinuousNiceLoop,
					2 => TechniqueCode.XyXChain,
					_ => throw Throwings.ImpossibleCase
				}
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Views[0].Links!).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
