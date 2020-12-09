using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>(grouped) alternating inference chain</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="YEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="Target">The target node.</param>
	public sealed record AicStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, bool XEnabled, bool YEnabled,
		in Node Target)
		: ChainingStepInfo(Conclusions, Views, XEnabled, YEnabled, default, default, default, default)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			(XEnabled && YEnabled ? 5.0M : 4.6M) + (FlatComplexity - 2).GetExtraDifficultyByLength();

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		public override Node[] ChainsTargets => new[] { Target };
#endif

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey => Enum.Parse<ChainingTypeCode>(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override int FlatComplexity => Target.AncestorsCount;

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Target.Chain switch
			{
				var chain => IsXChain switch
				{
					true => TechniqueCode.XChain,
					_ => (chain[^2].Digit == chain[1].Digit) switch
					{
						true => IsXyChain ? TechniqueCode.XyChain : TechniqueCode.Aic,
						false => Conclusions.Count switch
						{
							1 => TechniqueCode.DiscontinuousNiceLoop,
							2 => TechniqueCode.XyXChain,
							_ => TechniqueCode.Aic
						}
					}
				}
			};

		/// <summary>
		/// Indicates whether the specified chain is an X-Chain.
		/// </summary>
		private bool IsXChain => XEnabled && !YEnabled;


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Views[0].Links!).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
