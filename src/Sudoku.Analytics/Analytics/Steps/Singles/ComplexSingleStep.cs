namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn">Indicates the single technique that is based on.</param>
/// <param name="indirectTechniques">
/// <para>Indicates the indirect techniques used in this pattern.</para>
/// <para>
/// This value is an array of array of <see cref="Technique"/> instances,
/// describing the detail usage on the complex combination of indirect technique usages. For example:
/// <list type="table">
/// <listheader>
/// <term>Value</term>
/// <description>Meaning</description>
/// </listheader>
/// <item>
/// <term>[[<see cref="Technique.Pointing"/>, <see cref="Technique.Claiming"/>]]</term>
/// <description>The naked single will use a pointing and a claiming technique in one step</description>
/// </item>
/// <item>
/// <term>[[<see cref="Technique.Pointing"/>], [<see cref="Technique.Claiming"/>]]</term>
/// <description>We should apply a pointing firstly, and then claiming appears, then naked single appears</description>
/// </item>
/// </list>
/// </para>
/// </param>
public abstract partial class ComplexSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	[PrimaryConstructorParameter] Technique basedOn,
	[PrimaryConstructorParameter] Technique[][] indirectTechniques
) : SingleStep(conclusions, views, options, cell, digit, subtype)
{
	/// <summary>
	/// Indicates "Technique not supported" message.
	/// </summary>
	protected const string TechniqueNotSupportedMessage = "The specified technique is not supported.";


	/// <summary>
	/// Indicates the prefix name of the technique.
	/// </summary>
	protected abstract int PrefixNameLength { get; }

	/// <summary>
	/// Indicates the prefix name replaced.
	/// </summary>
	protected abstract string PrefixName { get; }


	/// <inheritdoc/>
	public new string GetName(CultureInfo? culture)
		=> $"{PrefixName}{base.GetName(culture ?? ResultCurrentCulture)[PrefixNameLength..]}";
}
