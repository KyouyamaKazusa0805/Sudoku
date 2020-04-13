using System.Collections.Generic;
using System.Text;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Indicates the detail data of UR+.
	/// </summary>
	public sealed class UrExtensionDetailData : UrDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="conjugatePairs">All conjugate pairs used.</param>
		/// <param name="name">The name of this instance.</param>
		public UrExtensionDetailData(
			IReadOnlyList<int> cells, IReadOnlyList<int> digits, IReadOnlyList<ConjugatePair> conjugatePairs,
			string name) : base(cells, digits) => (ConjugatePairs, Name) = (conjugatePairs, name);


		/// <summary>
		/// Indicates the conjugate pairs used.
		/// </summary>
		public IReadOnlyList<ConjugatePair> ConjugatePairs { get; }

		/// <summary>
		/// Indicates the name of this instance.
		/// </summary>
		public string Name { get; }

		/// <inheritdoc/>
		/// <remarks>
		/// This field will not be used in this instance.
		/// </remarks>
		public override int Type => 7;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string cellsStr = CellCollection.ToString(Cells);
			return $"{digitsStr} in cells {cellsStr} with {conjugatePairsToString()}";

			string conjugatePairsToString()
			{
				const string separator = ", ";
				var sb = new StringBuilder(ConjugatePairs.Count == 1 ? "a conjugate pair: " : "conjugate pairs: ");
				foreach (var cp in ConjugatePairs)
				{
					sb.Append($"{cp}{separator}");
				}

				return sb.RemoveFromEnd(separator.Length).ToString();
			}
		}
	}
}
