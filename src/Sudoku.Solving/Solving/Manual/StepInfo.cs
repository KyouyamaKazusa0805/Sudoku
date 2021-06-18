using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Techniques;
using Sudoku.CodeGen;
using static Sudoku.Resources.TextResources;
using Group = Sudoku.Techniques.TechniqueGroup;
using Level = Sudoku.Solving.Manual.DifficultyLevel;
using Tags = Sudoku.Techniques.TechniqueTags;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel))]
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel), nameof(Conclusions))]
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel), nameof(Conclusions), nameof(Views))]
	public abstract partial record StepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
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
		public virtual string Name => Current[TechniqueCode.ToString()];

		/// <summary>
		/// Indicates the acronym of the step name. For example, the acronym of the technique
		/// "Almost Locked Candidates" is ALC.
		/// </summary>
		/// <remarks>
		/// This property only contains the result in English. Other languages doesn't contain any
		/// abbreviations by default. On the other hand, this is really an easier way to implement
		/// than storing values in resource dictionary files.
		/// </remarks>
		public virtual string? Acronym => null;

		/// <summary>
		/// Indicates the technique name alias.
		/// </summary>
		public string[]? NameAlias => Current[$"{TechniqueCode.ToString()}Alias"]?.Split(new[] { ';', ' ' });

		/// <summary>
		/// The difficulty or this step.
		/// </summary>
		public abstract decimal Difficulty { get; }

		/// <summary>
		/// The technique code of this instance used for comparison
		/// (e.g. search for specified puzzle that contains this technique).
		/// </summary>
		public abstract Technique TechniqueCode { get; }

		/// <summary>
		/// The technique tags of this instance.
		/// </summary>
		public abstract Tags TechniqueTags { get; }

		/// <summary>
		/// The technique group that this technique instance belongs to.
		/// </summary>
		public virtual Group TechniqueGroup =>
			Enum.TryParse<Group>(TechniqueCode.ToString(), out var inst) ? inst : Group.None;

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract Level DifficultyLevel { get; }


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public void ApplyTo(ref SudokuGrid grid)
		{
			foreach (var conclusion in Conclusions)
			{
				conclusion.ApplyTo(ref grid);
			}
		}

		/// <summary>
		/// Determine whether the current step information instance with the specified flags.
		/// </summary>
		/// <param name="flags">
		/// The flags. If the argument contains more than one set bit, all flags will be checked
		/// one by one.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public unsafe bool HasTag(Tags flags)
		{
			delegate*<Tags, Tags, bool> func = flags.IsFlag() ? &EnumEx.Flags : &EnumEx.MultiFlags;

			return func(TechniqueTags, flags);
		}

		/// <summary>
		/// Returns a string that only contains the name and the basic information. Different with
		/// <see cref="ToFullString()"/>, the method will only contains the basic introduction
		/// about the technique.
		/// For example, in the <see cref="ExocetStepInfo"/>, the detail will contain the several special
		/// eliminations, in this method, those won't be displayed, But the method <see cref="ToFullString()"/>
		/// will.
		/// </summary>
		/// <returns>The string instance.</returns>
		/// <seealso cref="ExocetStepInfo"/>
		/// <seealso cref="ToFullString()"/>
		public abstract override string ToString();

		/// <summary>
		/// Returns a string that only contains the name and the conclusions.
		/// </summary>
		/// <returns>The string instance.</returns>
		public string ToSimpleString() => $"{Name} => {new ConclusionCollection(Conclusions).ToString()}";

		/// <summary>
		/// Returns a string that contains the name, the conclusions and its all details.
		/// This method is used for displaying details in text box control.
		/// </summary>
		/// <returns>The string instance.</returns>
		public virtual string ToFullString() => ToString();
	}
}
