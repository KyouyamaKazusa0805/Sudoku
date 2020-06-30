using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>template</b> technique.
	/// </summary>
	public sealed class TemplateTechniqueInfo : LastResortTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="isTemplateDeletion">
		/// Indicates whether this technique is template deletion.
		/// </param>
		public TemplateTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool isTemplateDeletion)
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
		public override decimal Difficulty => 9.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsTemplateDeletion ? TechniqueCode.TemplateDelete : TechniqueCode.TemplateSet;


		/// <inheritdoc/>
		public override string ToString()
		{
			string conclusionsStr = new ConclusionCollection(Conclusions).ToString();
			int digit = Digit + 1;
			return $"{Name}: Digit {digit} => {conclusionsStr}";
		}
	}
}
