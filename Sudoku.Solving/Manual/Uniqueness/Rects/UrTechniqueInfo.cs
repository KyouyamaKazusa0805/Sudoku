#nullable disable warnings
#pragma warning disable CA1036

using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	public abstract record UrTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable)
		: UniquenessTechniqueInfo(Conclusions, Views), IComparable<UrTechniqueInfo>
	{
		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;

		/// <inheritdoc/>
		public sealed override TechniqueCode TechniqueCode =>
			AliasAttribute.Convert<UrTypeCode, TechniqueCode>(TypeCode)!.Value;

		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string cellsStr = new GridMap(Cells).ToString();
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
				0 => new GridMap(Cells).CompareTo(other.Cells) switch
				{
					0 => Math.Sign((Digit1 * 9 + Digit2).CompareTo(other.Digit1 * 9 + other.Digit2)) switch
					{
						0 => TypeCode.CompareTo(other.TypeCode),
						1 => 1,
						-1 => -1,
						_ => throw Throwings.ImpossibleCase
					},
					1 => 1,
					-1 => -1,
					_ => throw Throwings.ImpossibleCase
				},
				1 => 1,
				-1 => -1,
				_ => throw Throwings.ImpossibleCase
			};
	}
}
