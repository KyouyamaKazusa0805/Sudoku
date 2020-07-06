using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;
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
			(XEnabled && YEnabled ? 5.0M : 4.6M) + GetExtraDifficultyByLength(FlatComplexity - 2);

		/// <inheritdoc/>
		public override int SortKey =>
			TechniqueCode switch
			{
				TechniqueCode.XyChain => 1,
				TechniqueCode.XyXChain => 2,
				TechniqueCode.DiscontinuousNiceLoop => 3,
				TechniqueCode.Aic => 4,
				_ => throw Throwings.ImpossibleCase
			};

		/// <inheritdoc/>
		public override int FlatComplexity => Target.AncestorsCount;

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode
		{
			get
			{
				var chain = Target.Chain;
				return (chain[^2].Digit == chain[1].Digit) switch
				{
					true => IsXyChain switch
					{
						true => TechniqueCode.XyChain,
						_ => TechniqueCode.Aic
					},
					false => Conclusions.Count switch
					{
						1 => TechniqueCode.DiscontinuousNiceLoop,
						2 => TechniqueCode.XyXChain,
						_ => throw Throwings.ImpossibleCase
					}
				};
			}
		}

		/// <summary>
		/// Indicates whether the specified chain is an XY-Chain.
		/// </summary>
		private bool IsXyChain
		{
			get
			{
				var links = Views[0].Links!;
				for (int i = 0; i < links.Count; i += 2)
				{
					var link = links[i];
					if (link.StartCandidate / 9 != link.EndCandidate / 9)
					{
						return false;
					}
				}

				return true;
			}
		}


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Views[0].Links!).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
