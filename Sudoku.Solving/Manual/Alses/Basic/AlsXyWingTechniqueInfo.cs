using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XY-Wing</b> (ALS-XY-Wing) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1.</param>
	/// <param name="Als2">The ALS 2.</param>
	/// <param name="Bridge">The bridge ALS.</param>
	/// <param name="XDigitsMask">The X digits mask.</param>
	/// <param name="YDigitsMask">The Y digits mask.</param>
	/// <param name="ZDigitsMask">The Z digits mask.</param>
	public sealed record AlsXyWingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, Als Als1, Als Als2,
		Als Bridge, short XDigitsMask, short YDigitsMask, short ZDigitsMask)
		: AlsTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.AlsXyWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string xStr = new DigitCollection(XDigitsMask.GetAllSets()).ToString();
			string yStr = new DigitCollection(YDigitsMask.GetAllSets()).ToString();
			string zStr = new DigitCollection(ZDigitsMask.GetAllSets()).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Als1} -> {BridgeAls} -> {Als2}, x = {xStr}, y = {yStr}, z = {zStr} => {elimStr}";
		}
	}
}
