using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of template technique.
	/// </summary>
	public sealed class TemplateTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="isTemplateDeletion">
		/// Indicates whether this technique is template deletion.
		/// </param>
		public TemplateTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			bool isTemplateDeletion)
			: base(conclusions, views) => IsTemplateDeletion = isTemplateDeletion;


		/// <summary>
		/// Indicates whether this technique is template deletion.
		/// </summary>
		public bool IsTemplateDeletion { get; }

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit => Conclusions[0].Digit;

		/// <inheritdoc/>
		public override string Name => $"Template {(IsTemplateDeletion ? "Delete" : "Set")}";

		/// <inheritdoc/>
		public override decimal Difficulty => 9.0m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString()
		{
			string conclusionsStr = ConclusionCollection.ToString(Conclusions);
			int digit = Digit + 1;
			return $"{Name}: Digit {digit} => {conclusionsStr}";
		}
	}
}
