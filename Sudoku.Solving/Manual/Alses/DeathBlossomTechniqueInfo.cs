using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>death blossom</b> technique.
	/// </summary>
	public sealed class DeathBlossomTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="pivot">The pivot cell.</param>
		/// <param name="alses">All ALSes used.</param>
		public DeathBlossomTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int pivot, IReadOnlyDictionary<int, Als> alses)
			: base(conclusions, views) => (Pivot, Alses) = (pivot, alses);


		/// <summary>
		/// Indicates how many petals used.
		/// </summary>
		public int PetalsCount => Alses.Count;

		/// <summary>
		/// Indicates the pivot cell.
		/// </summary>
		public int Pivot { get; }

		/// <summary>
		/// Indicates all ALSes used sorted by digit.
		/// </summary>
		public IReadOnlyDictionary<int, Als> Alses { get; }

		/// <inheritdoc/>
		public override string Name => $"Death Blossom ({PetalsCount} Petals)";

		/// <inheritdoc/>
		public override decimal Difficulty => 8M + PetalsCount * .1M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: Cell {CellUtils.ToString(Pivot)} - {GetAlsesStr()} => {elimStr}";
		}

		/// <summary>
		/// Get ALSes string.
		/// </summary>
		/// <returns>The string.</returns>
		private string GetAlsesStr()
		{
			const string separator = ", ";
			var sb = new StringBuilder();
			foreach (var (digit, als) in Alses)
			{
				sb.Append($"{digit + 1} - {als}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}
	}
}
