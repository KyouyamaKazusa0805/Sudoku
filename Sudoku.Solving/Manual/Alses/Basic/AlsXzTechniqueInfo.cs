using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XZ rule</b> (ALS-XZ)
	/// or <b>extended subset principle</b> technique.
	/// </summary>
	public sealed class AlsXzTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="als1">The ALS 1 used.</param>
		/// <param name="als2">The ALS 2 used.</param>
		/// <param name="xDigitsMask">The X digits mask.</param>
		/// <param name="zDigitsMask">The Z digits mask.</param>
		/// <param name="isDoublyLinked">Indicates whether the instance is a doubly linked ALS-XZ.</param>
		public AlsXzTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Als als1,
			Als als2, short xDigitsMask, short zDigitsMask, bool? isDoublyLinked) : base(conclusions, views) =>
			(Als1, Als2, XDigitsMask, ZDigitsMask, IsDoublyLinked) = (als1, als2, xDigitsMask, zDigitsMask, isDoublyLinked);


		/// <summary>
		/// The ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// The ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// The X digits mask (RCC digits).
		/// </summary>
		public short XDigitsMask { get; }

		/// <summary>
		/// The Z digits mask (target digits).
		/// </summary>
		public short ZDigitsMask { get; }

		/// <summary>
		/// <para>Indicates whether the instance is a doubly linked ALS-XZ.</para>
		/// <para>
		/// The property contains three different values:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="true"/></c></term>
		/// <description>The current instance is a Doubly Linked ALS-XZ.</description>
		/// </item>
		/// <item>
		/// <term><c><see langword="false"/></c></term>
		/// <description>The current instance is a Singly Linked ALS-XZ.</description>
		/// </item>
		/// <item>
		/// <term><c><see langword="null"/></c></term>
		/// <description>The current instance is a Extended Subset Principle.</description>
		/// </item>
		/// </list>
		/// </para>
		/// </summary>
		public bool? IsDoublyLinked { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsDoublyLinked switch
			{
				true => TechniqueCode.DoublyLinkedAlsXz,
				false => TechniqueCode.SinglyLinkedAlsXz,
				null => TechniqueCode.Esp
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			if (IsDoublyLinked is null)
			{
				// Extended subset principle.
				if (ZDigitsMask == 0)
				{
					string cellsStr = new CellCollection((Als1.Map | Als2.Map).Offsets).ToString();
					return $"{Name}: All digits cannot be duplicate in cells {cellsStr} => {elimStr}";
				}
				else
				{
					string digitStr = (ZDigitsMask.FindFirstSet() + 1).ToString();
					string cellsStr = new CellCollection((Als1.Map | Als2.Map).Offsets).ToString();
					return $"{Name}: Only the digit {digitStr} can be duplicate in cells {cellsStr} => {elimStr}";
				}
			}
			else
			{
				// ALS-XZ.
				string xStr = new DigitCollection(XDigitsMask.GetAllSets()).ToString();
				string zResultStr = ZDigitsMask != 0
					? $", z = {new DigitCollection(ZDigitsMask.GetAllSets()).ToString()}"
					: string.Empty;
				return $"{Name}: ALS 1: {Als1}, ALS 2: {Als2}, x = {xStr}{zResultStr} => {elimStr}";
			}
		}
	}
}
