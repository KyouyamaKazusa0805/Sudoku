#pragma warning disable IDE0060

using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle plus</b> (UR+) or
	/// <b>avoidable rectangle plus</b> (AR+) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TypeCode">The type code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ConjugatePairs">All conjugate pairs.</param>
	public record UrPlusTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		IReadOnlyList<ConjugatePair> ConjugatePairs)
		: UrTechniqueInfo(Conclusions, Views, TypeCode, Digit1, Digit2, Cells, IsAvoidable)
	{
		/// <inheritdoc/>
		public sealed override decimal Difficulty => ConjugatePairs.Count * .2M + 4.4M;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString() => ToStringInternal();

		/// <inheritdoc/>
		protected sealed override string GetAdditional()
		{
			bool singular = ConjugatePairs.Count == 1;
			return $"{(singular ? "a " : string.Empty)}conjugate pair{(singular ? string.Empty : "s")} {getStr()}";

			string getStr()
			{
				const string separator = ", ";
				var sb = new StringBuilder();
				foreach (var cp in ConjugatePairs)
				{
					sb.Append($"{cp}{separator}");
				}

				return sb.RemoveFromEnd(separator.Length).ToString();
			}
		}
	}
}
