using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

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
	public sealed record AlsXyWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Als Als1, in Als Als2,
		in Als Bridge, short XDigitsMask, short YDigitsMask, short ZDigitsMask) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.AlsXyWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string xStr = new DigitCollection(XDigitsMask.GetAllSets()).ToString();
			string yStr = new DigitCollection(YDigitsMask.GetAllSets()).ToString();
			string zStr = new DigitCollection(ZDigitsMask.GetAllSets()).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Als1} -> {Bridge} -> {Als2}, x = {xStr}, y = {yStr}, z = {zStr} => {elimStr}";
		}
	}
}
