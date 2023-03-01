namespace Sudoku.Solving.Logical;

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
	/// <code><![CDATA["Cells {0}, with digits {1}"]]></code>
	/// </para>
	/// <para>
	/// Here the placeholder <c>{0}</c>, and <c>{1}</c> must be provided by property <see cref="FormatInterpolatedParts"/>.
	/// You should create 2 values that can be replaced with the placeholder <c>{0}</c> and <c>{1}</c>.
	/// </para>
	/// <para>
	/// The recommended implementation pattern is:
	/// <code><![CDATA[
	/// private string CellsStr => Cells.ToString();
	/// private string DigitsStr => Digits.ToString(separator: R.EmitPunctuation(Punctuation.Comma));
	/// ]]></code>
	/// </para>
	/// <para>
	/// And then fill the blank via property <see cref="FormatInterpolatedParts"/>:
	/// <code><![CDATA[
	/// public override IDictionary<string, string[]?>> FormatInterpolatedParts
	///     => [["en-US": [CellsStr, DigitsStr]], ["zh-CN": [CellsStr, DigitsStr]]];
	/// ]]></code>
	/// via the feature provided by C# 12: Collection Literals. If you cannot decide the real name of the culture name,
	/// just use suffix instead like <c>"en"</c> and <c>"zh"</c>, ignoring cases.
	/// </para>
	/// <para>
	/// If you want to use the values in the resource dictionary, you can just use a <see langword="static readonly"/> field
	/// called <see cref="R"/>, for example:
	/// <code><![CDATA[
	/// using static Sudoku.Resources.MergedResources;
	/// 
	/// public override string Format => R["TheKeyYouWantToSearch"];
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="FormatInterpolatedParts"/>
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
	/// Indicates the interpolated parts that is used for the format.
	/// The formats will be interpolated into the property <see cref="Format"/> result.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property use a dictionary to merge globalizational format data.
	/// The key type is <see cref="string"/>, which can be used for the comparison of the current culture via type <see cref="CultureInfo"/>,
	/// for example, <c>"zh"</c>.
	/// </para>
	/// <para>
	/// For more backing implementation details, please visit method <c>ToString</c> in derived class type called <c>Step</c>.
	/// </para>
	/// </remarks>
	/// <seealso cref="Format"/>
	/// <seealso cref="CultureInfo"/>
	IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts { get; }


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
	/// Returns a string that only contains the name and the basic description.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks>
	/// <para><i>
	/// This method uses modifiers <see langword="sealed"/> and <see langword="override"/> to prevent with compiler overriding this method.
	/// </i></para>
	/// <para><b><i>
	/// In addition, <c>ToString</c> is a special method that has already been declared in type <see cref="object"/>
	/// as <see langword="virtual"/> one, so this interface member lacks of binding behavior on implementing,
	/// which means even if you don't implement a parameterless method called <c>ToString</c> that returns <see cref="string"/>,
	/// the code will also be compiled successfully if nullability analysis is disabled.
	/// This member declared here is only used as a role of XML documentation comment provider.
	/// </i></b></para>
	/// </remarks>
	string ToString();

	/// <summary>
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	protected abstract string ConclusionText { get; }
}
