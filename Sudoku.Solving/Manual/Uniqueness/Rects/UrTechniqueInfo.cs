using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	public abstract class UrTechniqueInfo : UniquenessTechniqueInfo, IComparable<UrTechniqueInfo>
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="isAr">Indicates whether the structure is an AR.</param>
		public UrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			UrTypeCode typeCode, int digit1, int digit2, int[] cells, bool isAr) : base(conclusions, views) =>
			(Digit1, Digit2, Cells, IsAr, TypeCode) = (digit1, digit2, cells, isAr, typeCode);


		/// <summary>
		/// Indicates the UR type code.
		/// </summary>
		public UrTypeCode TypeCode { get; }

		/// <summary>
		/// Indicates the digit 1.
		/// </summary>
		public int Digit1 { get; }

		/// <summary>
		/// Indicates the digit 2.
		/// </summary>
		public int Digit2 { get; }

		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public int[] Cells { get; }

		/// <summary>
		/// Indicates the current structure is UR or AR.
		/// </summary>
		public bool IsAr { get; }

		/// <inheritdoc/>
		public override string Name => $"{(IsAr ? "Avoidable" : "Unique")} Rectangle {TypeCode.GetCustomName()}";

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			TypeCode switch
			{
				UrTypeCode.Type1 => TechniqueCode.UrType1,
				UrTypeCode.Type2 => TechniqueCode.UrType2,
				UrTypeCode.Type3 => TechniqueCode.UrType3,
				UrTypeCode.Type4 => TechniqueCode.UrType4,
				UrTypeCode.Type5 => TechniqueCode.UrType5,
				UrTypeCode.Type6 => TechniqueCode.UrType6,
				UrTypeCode.Hidden => TechniqueCode.HiddenUr,
				UrTypeCode.Plus2D => TechniqueCode.UrPlus2D,
				UrTypeCode.Plus2B1SL => TechniqueCode.UrPlus2B1SL,
				UrTypeCode.Plus2D1SL => TechniqueCode.UrPlus2D1SL,
				UrTypeCode.Plus3X => TechniqueCode.UrPlus3X,
				UrTypeCode.Plus3x1SL => TechniqueCode.UrPlus3x1SL,
				UrTypeCode.Plus3X1SL => TechniqueCode.UrPlus3X1SL,
				UrTypeCode.Plus3X2SL => TechniqueCode.UrPlus3X2SL,
				UrTypeCode.Plus3N2SL => TechniqueCode.UrPlus3N2SL,
				UrTypeCode.Plus3U2SL => TechniqueCode.UrPlus3U2SL,
				UrTypeCode.Plus3E2SL => TechniqueCode.UrPlus3E2SL,
				UrTypeCode.Plus4x1SL => TechniqueCode.UrPlus4x1SL,
				UrTypeCode.Plus4X1SL => TechniqueCode.UrPlus4X1SL,
				UrTypeCode.Plus4x2SL => TechniqueCode.UrPlus4x2SL,
				UrTypeCode.Plus4X2SL => TechniqueCode.UrPlus4X2SL,
				UrTypeCode.Plus4X3SL => TechniqueCode.UrPlus4X3SL,
				UrTypeCode.Plus4C3SL => TechniqueCode.UrPlus4C3SL,
				UrTypeCode.XyWing => TechniqueCode.UrXyWing,
				UrTypeCode.XyzWing => TechniqueCode.UrXyzWing,
				UrTypeCode.WxyzWing => TechniqueCode.VwxyzWing,
				UrTypeCode.Sdc => TechniqueCode.UrSdc,
				_ => throw Throwing.ImpossibleCase
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: {d1}, {d2} in {cellsStr}{(additional is null ? string.Empty : $" with {additional}")} => " +
				$"{elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();

		/// <inheritdoc/>
		int IComparable<UrTechniqueInfo>.CompareTo(UrTechniqueInfo other) =>
			Math.Sign(TypeCode.CompareTo(other.TypeCode)) switch
			{
				0 => new GridMap(Cells).CompareTo(new GridMap(other.Cells)) switch
				{
					0 => Math.Sign((Digit1 * 9 + Digit2).CompareTo(other.Digit1 * 9 + other.Digit2)) switch
					{
						0 => TypeCode.CompareTo(other.TypeCode),
						1 => 1,
						-1 => -1,
						_ => throw Throwing.ImpossibleCase
					},
					1 => 1,
					-1 => -1,
					_ => throw Throwing.ImpossibleCase
				},
				1 => 1,
				-1 => -1,
				_ => throw Throwing.ImpossibleCase
			};
	}
}
