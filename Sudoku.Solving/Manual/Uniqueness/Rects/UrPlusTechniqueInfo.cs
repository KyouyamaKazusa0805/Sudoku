using System.Collections.Generic;
using System.Text;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle plus</b> (UR+) or
	/// <b>avoidable rectangle plus</b> (AR+) technique.
	/// </summary>
	public class UrPlusTechniqueInfo : UrTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeCode">The type code.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All UR cells.</param>
		/// <param name="conjugatePairs">All conjugate pairs.</param>
		/// <param name="isAr">Indicates whether the specified structure is an AR.</param>
		public UrPlusTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			UrTypeCode typeCode, int digit1, int digit2, int[] cells,
			IReadOnlyList<ConjugatePair> conjugatePairs, bool isAr)
			: base(conclusions, views, typeCode, digit1, digit2, cells, isAr) =>
			ConjugatePairs = conjugatePairs;


		/// <summary>
		/// Indicates all conjugate pairs used.
		/// </summary>
		public IReadOnlyList<ConjugatePair> ConjugatePairs { get; }

		/// <inheritdoc/>
		public sealed override decimal Difficulty => ConjugatePairs.Count * .2M + 4.4M;

		/// <inheritdoc/>
		public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		protected sealed override string GetAdditional()
		{
			bool singular = ConjugatePairs.Count == 1;
			return $"{(singular ? "a" : string.Empty)} conjugate pair{(singular ? string.Empty : "s")} {getStr()}";

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
