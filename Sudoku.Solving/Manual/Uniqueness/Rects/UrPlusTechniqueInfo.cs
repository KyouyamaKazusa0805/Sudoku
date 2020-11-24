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
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public record UrPlusTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		UrTypeCode TypeCode, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		IReadOnlyList<ConjugatePair> ConjugatePairs, int AbsoluteOffset)
		: UrTechniqueInfo(Conclusions, Views, TypeCode, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
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
			return $"{(singular ? "a " : string.Empty)}conjugate pair{(singular ? string.Empty : "s")} {g()}";

			unsafe string g()
			{
				const string separator = ", ";
				static string? converter(in ConjugatePair cp) => $"{cp}{separator}";
				return new StringBuilder()
					.AppendRange<ConjugatePair, string?>(ConjugatePairs, &converter)
					.RemoveFromEnd(separator.Length)
					.ToString();
			}
		}
	}
}
