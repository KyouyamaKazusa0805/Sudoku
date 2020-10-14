using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Windows;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract partial record TechniqueInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	{
		/// <summary>
		/// <para>
		/// Indicates whether the difficulty rating of this technique should be
		/// shown in the output screen. Some techniques such as <b>Gurth's symmetrical placement</b>
		/// doesn't need to show the difficulty (because the difficulty of this technique
		/// is unstable).
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, the analysis result won't show the difficulty
		/// of this instance.
		/// </para>
		/// </summary>
		public virtual bool ShowDifficulty => true;

		/// <summary>
		/// Indicates the technique name.
		/// </summary>
		public virtual string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <summary>
		/// The difficulty or this step.
		/// </summary>
		public abstract decimal Difficulty { get; }

		/// <summary>
		/// The technique code of this instance used for comparison
		/// (e.g. search for specified puzzle that contains this technique).
		/// </summary>
		public abstract TechniqueCode TechniqueCode { get; }

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract DifficultyLevel DifficultyLevel { get; }


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public void ApplyTo(Grid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(grid);
			}
		}

		/// <summary>
		/// Returns a string that only contains the name and the basic information. Different with
		/// <see cref="ToFullString"/>, the method will only contains the basic introduction about the technique.
		/// For example, in the <see cref="ExocetTechniqueInfo"/>, the detail will contain the several special
		/// eliminations, in this method, those won't be displayed, But the method <see cref="ToFullString"/>
		/// will.
		/// </summary>
		/// <returns>The string instance.</returns>
		/// <seealso cref="ExocetTechniqueInfo"/>
		/// <seealso cref="ToFullString"/>
		public abstract override string ToString();

		/// <summary>
		/// Returns a string that only contains the name and the conclusions.
		/// </summary>
		/// <returns>The string instance.</returns>
		public string ToSimpleString()
		{
			using var elims = new ConclusionCollection(Conclusions);
			return $"{Name} => {elims.ToString()}";
		}

		/// <summary>
		/// Returns a string that contains the name, the conclusions and its all details.
		/// This method is used for displaying details in text box control.
		/// </summary>
		/// <returns>The string instance.</returns>
		public virtual string ToFullString() => ToString();
	}
}
