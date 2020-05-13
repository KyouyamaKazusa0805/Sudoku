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
	public sealed class AlsXyWingTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="als1">The ALS 1.</param>
		/// <param name="als2">The ALS 2.</param>
		/// <param name="bridgeAls">The bridge ALS.</param>
		/// <param name="xDigitsMask">The X digits mask.</param>
		/// <param name="yDigitsMask">The Y digits mask.</param>
		/// <param name="zDigitsMask">The Z digits mask.</param>
		public AlsXyWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Als als1, Als als2, Als bridgeAls, short xDigitsMask, short yDigitsMask, short zDigitsMask)
			: base(conclusions, views) =>
			(Als1, Als2, BridgeAls, XDigitsMask, YDigitsMask, ZDigitsMask) = (als1, als2, bridgeAls, xDigitsMask, yDigitsMask, zDigitsMask);


		/// <summary>
		/// The ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// The ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// The bridge ALS.
		/// </summary>
		public Als BridgeAls { get; }

		/// <summary>
		/// The X digits mask.
		/// </summary>
		public short XDigitsMask { get; }

		/// <summary>
		/// The Y digits mask.
		/// </summary>
		public short YDigitsMask { get; }

		/// <summary>
		/// The Z digits mask.
		/// </summary>
		public short ZDigitsMask { get; }

		/// <inheritdoc/>
		public override string Name => "Almost Locked Sets XY-Wing";

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
