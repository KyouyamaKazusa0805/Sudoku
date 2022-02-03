using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides a basic manual solving step.
/// </summary>
public interface IStep
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
	bool ShowDifficulty { get; }

	/// <summary>
	/// <para>Indicates whether the step is an EST (i.e. Elementary Sudoku Technique) step.</para>
	/// <para>
	/// Here we define that the techniques often appearing and commonly to be used as below are ESTs:
	/// <list type="bullet">
	/// <item>Full House, Last Digit, Hidden Single, Naked Single</item>
	/// <item>Pointing, Claiming</item>
	/// <item>Naked Pair, Naked Triple, Naked Quarduple</item>
	/// <item>Naked Pair (+), Naked Triple (+), Naked Quarduple (+)</item>
	/// <item>Hidden Pair, Hidden Triple, Hidden Quarduple</item>
	/// <item>Locked Pair, Locked Triple</item>
	/// </list>
	/// </para>
	/// <para>The default value of this property is <see langword="false"/>.</para>
	/// </summary>
	bool IsElementary { get; }

	/// <summary>
	/// Indicates the technique name. The default value is in the resource dictionary.
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
	/// making them non-<see langword="public"/> and applying attribute <see cref="FormatItemAttribute"/>
	/// to it.
	/// </para>
	/// <para>
	/// The recommended implementation pattern is:
	/// <code><![CDATA[
	/// [FormatItem]
	/// private string CellsStr
	/// {
	///     [MethodImpl(MethodImplOptions.AggressiveInlining)]
	///	    get => Cells.ToString();
	/// }
	/// ]]></code>
	/// You can use the code snippet <c>fitem</c> to create the pattern, whose corresponding file is added
	/// into the <c>optional/vssnippets</c> folder. For more information, please open the markdown file
	/// <see href="https://github.com/SunnieShine/Sudoku/blob/main/optional/README.md">README.md</see>
	/// (in the <c>optional</c> folder) for more information.
	/// </para>
	/// <para>
	/// Because this property will get the value from the resource dictionary, the property supports
	/// multiple language switching, which is better than the normal methods <see cref="ToString"/>
	/// and <see cref="ToFullString"/>. Therefore, this property is the substitution of those two methods.
	/// </para>
	/// <para>
	/// If you want to use the values in the resource documents, just use the property
	/// <see cref="ResourceManager.Shared"/> is okay:
	/// <code>
	/// public override string Format => ResourceDocumentManager.Shared["TheKeyYouWantToSearch"];
	/// </code>
	/// </para>
	/// </remarks>
	/// <seealso cref="ToString"/>
	/// <seealso cref="ToFullString"/>
	/// <seealso cref="ResourceManager.Shared"/>
	string? Format { get; }

	/// <summary>
	/// The difficulty or this step.
	/// </summary>
	decimal Difficulty { get; }

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
	/// during generting puzzles.
	/// </remarks>
	/// <seealso cref="FlagsAttribute"/>
	DifficultyLevel DifficultyLevel { get; }

	/// <summary>
	/// Indicates the stableness of this technique. The default value is <see cref="Stableness.Stable"/>.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generting puzzles.
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
	/// during generting puzzles.
	/// </remarks>
	/// <seealso cref="FlagsAttribute"/>
	Rarity Rarity { get; }

	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	ImmutableArray<Conclusion> Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	ImmutableArray<PresentationData> Views { get; }

	/// <summary>
	/// Indicates the string representation of the conclusions.
	/// </summary>
	/// <remarks>
	/// Most of techniques uses eliminations
	/// so this property is named <c>ElimStr</c>. In other words, if the conclusion is an assignment one,
	/// the property will still use this name rather than <c>AssignmentStr</c>.
	/// </remarks>
	protected abstract string ElimStr { get; }


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	void ApplyTo(ref Grid grid);

	/// <summary>
	/// Returns a string that only contains the name and the basic information.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks>
	/// This method uses <see langword="sealed"/> and <see langword="override"/> modifiers
	/// to prevent the compiler overriding the method; in additional, the default behavior is changed to
	/// output as the method <see cref="Formatize(bool)"/> invoking.
	/// </remarks>
	/// <seealso cref="Formatize(bool)"/>
	string ToString();

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
}
