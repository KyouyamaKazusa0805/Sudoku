﻿namespace Sudoku.Solving.Logical;

/// <summary>
/// Provides a basic logical solving step.
/// </summary>
public interface IStep : IVisual
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
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	sealed bool ShowDifficulty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepDisplayingFeatureAttribute>() switch
		{
			{ Features: var f } => !f.Flags(StepDisplayingFeature.HideDifficultyRating),
			null => true
		};
	}

	/// <summary>
	/// Indicates the technique name. The technique name are all stored in the resource dictionary,
	/// you can find them in the <c>Resources</c> folder (Type <see cref="MergedResources"/>).
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Gets the format of the current instance.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="string"/> result. If the resource dictionary doesn't contain
	/// any valid formats here, the result value will be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// A <b>format</b> is the better way to format the result text of this technique information instance.
	/// It'll be represented by the normal characters and the placeholders, e.g.
	/// <code><![CDATA["{Name}: Cells {CellsStr} => {ElimsStr}"]]></code>
	/// Here the string result <b>shouldn't</b> be with the leading <c>'$'</c> character, because this is a
	/// format string instead of an interpolated one.
	/// </para>
	/// <para>
	/// Here the property <c>Name</c>, <c>CellsStr</c> and <c>ElimsStr</c> must have been implemented before
	/// the property invoked. You should create 3 properties whose names are <c>Name</c>, <c>CellsStr</c>
	/// and <c>ElimsStr</c>, and return the corresponding correct string result,
	/// making them non-<see langword="public"/> (suggested keyword is <see langword="internal"/>)
	/// and applying attribute <see cref="ResourceTextFormatterAttribute"/> to it.
	/// </para>
	/// <para>
	/// The recommended implementation pattern is:
	/// <code><![CDATA[
	/// [ResourceTextFormatter]
	/// internal string CellsStr() => Cells.ToString();
	/// ]]></code>
	/// You can use the code snippet <c>fitem</c> to create the pattern, whose corresponding file is added
	/// into the <c>optional/vssnippets</c> folder. For more information, please open the markdown file
	/// <see href="https://github.com/SunnieShine/Sudoku/blob/main/optional/README.md">README.md</see>
	/// (in the <c>optional</c> folder) for more information.
	/// </para>
	/// <para>
	/// Because this property will get the value from the resource dictionary, the property supports
	/// multiple language switching, which is better than the normal methods <c>ToString</c>
	/// and <c>ToFullString</c>. Therefore, this property is the substitution of those two methods.
	/// </para>
	/// <para>
	/// If you want to use the values in the resource documents,
	/// just use the <see langword="static readonly"/> field <see cref="R"/> is okay:
	/// <code><![CDATA[
	/// using static Sudoku.Resources.MergedResources;
	/// 
	/// public override string Format => R["TheKeyYouWantToSearch"];
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="ToFullString"/>
	/// <seealso cref="ResourceTextFormatterAttribute"/>
	/// <seealso cref="R"/>
	string? Format { get; }

	/// <summary>
	/// Indicates the difficulty of this technique step.
	/// </summary>
	/// <remarks>
	/// Generally this property holds the default and basic difficulty of the step.
	/// If the step's difficulty rating requires multiple factors, this property will provide with a basic difficulty value
	/// as elementary and default rating value; other factors will be given in the other property <see cref="ExtraDifficultyCases"/>.
	/// </remarks>
	/// <seealso cref="ExtraDifficultyCases"/>
	decimal BaseDifficulty { get; }

	/// <summary>
	/// Indicates the total difficulty of the technique step. This value is the total sum of merged result from two properties
	/// <see cref="BaseDifficulty"/> and <see cref="ExtraDifficultyCases"/>. For property <see cref="ExtraDifficultyCases"/>,
	/// the result is to sum all values up of inner property <see cref="ExtraDifficultyCase.Value"/>.
	/// </summary>
	/// <seealso cref="BaseDifficulty"/>
	/// <seealso cref="ExtraDifficultyCases"/>
	/// <seealso cref="ExtraDifficultyCase"/>
	sealed decimal Difficulty => BaseDifficulty + (ExtraDifficultyCases?.Sum(static @case => @case.Value) ?? 0);

	/// <summary>
	/// The technique code of this instance used for comparison
	/// (e.g. search for specified puzzle that contains this technique).
	/// </summary>
	Technique TechniqueCode { get; }

	/// <summary>
	/// The technique tags of this instance.
	/// </summary>
	TechniqueTags TechniqueTags { get; }

	/// <summary>
	/// The technique group that this technique instance belongs to.
	/// </summary>
	TechniqueGroup TechniqueGroup { get; }

	/// <summary>
	/// The difficulty level of this step.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generating puzzles.
	/// </remarks>
	/// <seealso cref="FlagsAttribute"/>
	DifficultyLevel DifficultyLevel { get; }

	/// <summary>
	/// Indicates the stableness of this technique. The default value is <see cref="Stableness.Stable"/>.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generating puzzles.
	/// </remarks>
	/// <seealso cref="Stableness.Stable"/>
	/// <seealso cref="FlagsAttribute"/>
	Stableness Stableness { get; }

	/// <summary>
	/// Indicates the rarity of this technique appears.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generating puzzles.
	/// </remarks>
	/// <seealso cref="FlagsAttribute"/>
	Rarity Rarity { get; }

	/// <summary>
	/// Indicates the extra difficulty cases of the technique step. If the step does not contain such cases,
	/// this property will keep <see langword="null"/> value.
	/// </summary>
	ExtraDifficultyCase[]? ExtraDifficultyCases { get; }


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	void ApplyTo(scoped ref Grid grid);

	/// <summary>
	/// Determine whether the current step information instance with the specified flags.
	/// </summary>
	/// <param name="flags">
	/// The flags. If the argument contains more than one set bit, all flags will be checked one by one.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	bool HasTag(TechniqueTags flags);

	/// <summary>
	/// Returns a string that only contains the name and the conclusions.
	/// </summary>
	/// <returns>The string instance.</returns>
	string ToSimpleString();

	/// <summary>
	/// Returns a string that contains the name, the conclusions and its all details.
	/// This method is used for displaying details in text box control.
	/// </summary>
	/// <returns>The string instance.</returns>
	string ToFullString();

	/// <summary>
	/// Formatizes the <see cref="Format"/> property string and output the result.
	/// </summary>
	/// <param name="handleEscaping">Indicates whether the method will handle the escaping characters.</param>
	/// <returns>The result string.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the format is invalid. The possible cases are:
	/// <list type="bullet">
	/// <item>The format is <see langword="null"/>.</item>
	/// <item>The interpolation part contains the empty value.</item>
	/// <item>Missing the closed brace character <c>'}'</c>.</item>
	/// <item>The number of interpolations failed to match.</item>
	/// </list>
	/// </exception>
	/// <seealso cref="Format"/>
	string Formatize(bool handleEscaping = false);

	/// <summary>
	/// Indicates the string representation of the conclusions.
	/// </summary>
	/// <remarks>
	/// Most of techniques uses eliminations
	/// so this property is named <c>ElimStr</c>. In other words, if the conclusion is an assignment one,
	/// the property will still use this name rather than <c>AssignmentStr</c>.
	/// </remarks>
	protected string ElimStr();
}
