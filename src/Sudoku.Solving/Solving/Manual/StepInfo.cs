using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Techniques;
using Sudoku.CodeGenerating;
using Sudoku.Resources;

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
		public virtual string Name => TextResources.Current[TechniqueCode.ToString()];

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
		public string[]? NameAlias =>
			TextResources.Current[$"{TechniqueCode.ToString()}Alias"]?.Split(new[] { ';', ' ' });

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
		public abstract TechniqueTags TechniqueTags { get; }

		/// <summary>
		/// The technique group that this technique instance belongs to.
		/// </summary>
		public virtual TechniqueGroup TechniqueGroup =>
			Enum.TryParse<TechniqueGroup>(TechniqueCode.ToString(), out var inst) ? inst : TechniqueGroup.None;

		/// <summary>
		/// The difficulty level of this step.
		/// </summary>
		public abstract DifficultyLevel DifficultyLevel { get; }


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
		public bool HasTag(TechniqueTags flags) =>
			flags.IsFlag() ? TechniqueTags.Flags(flags) : TechniqueTags.MultiFlags(flags);

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

		/// <summary>
		/// Gets the format of the current instance.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="string"/> result. If the resource dictionary doesn't contain
		/// any valid formats here, the result value will be <see langword="null"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// A <b>format</b> is the better way to format the result text of this technique information instance,
		/// It'll be represented by the normal characters and the placeholders, e.g.
		/// <code>
		/// <![CDATA["{Name}: Cells {CellsStr} => {ElimsStr}"]]>
		/// </code>
		/// Here the string result <b>shouldn't</b> be with the leading <c>'$'</c> character, because this is a
		/// format string, rather than a interpolated string.
		/// </para>
		/// <para>
		/// Here the property <c>Name</c>, <c>CellsStr</c> and <c>ElimsStr</c> should be implemented before
		/// the property invoked, you should creates those 3 properties, returns the corresponding correct string
		/// result, makes them <see langword="private"/> or <see langword="protected"/> and marks the attribute
		/// <see cref="FormatItemAttribute"/> to help the code analyzer (if the code analyzer is available).
		/// The recommended implementation pattern is:
		/// <code><![CDATA[
		/// #if SOLUTION_WIDE_CODE_ANALYSIS
		/// [FormatItem]
		/// #endif
		/// private string CellsStr
		/// {
		///     [MethodImpl(MethodImplOptions.AggressiveInlining)]
		///	    get => Cells.ToString();
		/// }
		/// ]]></code>
		/// You can use the code snippet <c>fitem</c> to create the pattern, whose corresponding file is added
		/// into the <c>required/vssnippets</c> folder. For more information, please open the markdown file
		/// <see href="file:///../../../../required/README.md">README.md</see> in the <c>required</c> folder
		/// to learn more information.
		/// </para>
		/// <para>
		/// Because this property will get the value from the resource dictionary, the property supports
		/// multiple language switching, which is better than the normal methods <see cref="ToString"/>
		/// and <see cref="ToFullString"/>. Therefore, this property is the substitution plan of those two methods.
		/// </para>
		/// </remarks>
		/// <seealso cref="ToString"/>
		/// <seealso cref="ToFullString"/>
		public virtual string? Format
		{
			get
			{
				try
				{
					return TextResources.Current[$"Format_{GetType().Name}"];
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
