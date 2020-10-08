#pragma warning disable IDE0060

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
	/// Provides a usage of <b>(grouped) continuous nice loop</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="Target">The target.</param>
	public sealed record LoopTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, bool XEnabled, bool YEnabled, Node Target)
		: ChainingTechniqueInfo(Conclusions, Views, XEnabled, YEnabled, default, default, default, default)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			(XEnabled && YEnabled ? 5.0M : 4.5M) + (FlatComplexity - 2).GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsXCycle switch
			{
				true => TechniqueCode.FishyCycle,
				_ => IsXyChain switch { true => TechniqueCode.XyCycle, _ => TechniqueCode.ContinuousNiceLoop }
			};

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey => Enum.Parse<ChainingTypeCode>(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override int FlatComplexity => Target.AncestorsCount;

		/// <summary>
		/// Indicates whether the specified cycle is an X-Cycle.
		/// </summary>
		private bool IsXCycle => XEnabled && !YEnabled;


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Views[0].Links!).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
