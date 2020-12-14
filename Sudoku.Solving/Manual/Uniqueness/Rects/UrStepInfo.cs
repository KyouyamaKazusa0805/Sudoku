#nullable disable warnings

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
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
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public abstract record UrStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable, int AbsoluteOffset)
		: UniquenessStepInfo(Conclusions, Views), IComparable<UrStepInfo>
	{
		/// <inheritdoc/>
		public sealed override string Name => base.Name;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;

		/// <inheritdoc/>
		public sealed override TechniqueCode TechniqueCode =>
			AliasAttribute.Convert<UrTypeCode, TechniqueCode>(TypeCode)!.Value;

		/// <inheritdoc/>
		public override string ToString() => ToStringInternal();


		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string? GetAdditional();

		/// <summary>
		/// Same as <see cref="ToString"/> but this is implementation part.
		/// </summary>
		/// <returns>The result.</returns>
		protected string ToStringInternal()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string? additional = GetAdditional();
			return
				$"{Name}: {d1}, {d2} in " +
				$"{cellsStr}{(additional is null ? string.Empty : $" with {additional}")} => {elimStr}";
		}

		/// <inheritdoc/>
		int IComparable<UrStepInfo>.CompareTo(UrStepInfo other) => InternalComparsion(this, other);


		/// <summary>
		/// Internal comparsion.
		/// </summary>
		/// <param name="l">The left comparer.</param>
		/// <param name="r">The right comparer.</param>
		/// <returns>An <see cref="int"/> value indicating the result.</returns>
		private static int InternalComparsion(UrStepInfo l, UrStepInfo r) =>
			Math.Sign(l.TypeCode.CompareTo(r.TypeCode)) switch
			{
				0 => l.AbsoluteOffset.CompareTo(r.AbsoluteOffset) switch
				{
					0 => Math.Sign((l.Digit1 * 9 + l.Digit2).CompareTo(r.Digit1 * 9 + r.Digit2)) switch
					{
						0 => l.TypeCode.CompareTo(r.TypeCode),
						1 => 1,
						-1 => -1
					},
					1 => 1,
					-1 => -1
				},
				1 => 1,
				-1 => -1
			};


		/// <inheritdoc cref="Operators.operator &gt;"/>
		public static bool operator >(UrStepInfo left, UrStepInfo right) => InternalComparsion(left, right) > 0;

		/// <inheritdoc cref="Operators.operator &gt;="/>
		public static bool operator >=(UrStepInfo left, UrStepInfo right) => InternalComparsion(left, right) >= 0;

		/// <inheritdoc cref="Operators.operator &lt;"/>
		public static bool operator <(UrStepInfo left, UrStepInfo right) => InternalComparsion(left, right) < 0;

		/// <inheritdoc cref="Operators.operator &lt;="/>
		public static bool operator <=(UrStepInfo left, UrStepInfo right) => InternalComparsion(left, right) <= 0;
	}
}
