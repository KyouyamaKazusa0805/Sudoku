using System.Globalization;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Analytics.Strings;
using Sudoku.Rendering;
using Sudoku.Strings;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics;

/// <summary>
/// Provides with a solving step that describes for a technique usage, with conclusions and detail data for the corresponding technique pattern.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="IRenderable.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="IRenderable.Views" path="/summary"/></param>
/// <param name="options">
/// Indicates an optional instance that provides with extra information for a step searcher.
/// This instance can be used for checking some extra information about a step such as notations to a cell, candidate, etc..
/// </param>
public abstract partial class Step(
	[Data(SetterExpression = "internal set")] Conclusion[] conclusions,
	[Data] View[]? views,
	[Data] StepSearcherOptions options
) : IRenderable
{
	/// <summary>
	/// The error information for difficulty level cannot be determined.
	/// </summary>
	private static readonly string ErrorInfo_TechniqueLevelCannotBeDetermined = $"""
		The target level is unknown. If you see this exception thrown, 
		please append '{typeof(DifficultyLevelAttribute).FullName}' to the target technique code field 
		defined in type '{typeof(Technique).FullName}'.
		""".RemoveLineEndings();


	/// <summary>
	/// Indicates the technique name.
	/// </summary>
	/// <remarks>
	/// The technique name are all stored in the resource dictionary,
	/// you can find them in the <c>Strings</c> folder (Type <see cref="StringsAccessor"/>).
	/// </remarks>
	public virtual string Name => Code.GetName();

	/// <summary>
	/// Indicates the English name of the technique.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Name" path="/remarks"/>
	/// </remarks>
	public virtual string EnglishName => Code.GetEnglishName() ?? throw new ResourceNotFoundException(Code.ToString(), GetType().Assembly);

	/// <summary>
	/// Indicates the difficulty of this technique step.
	/// </summary>
	/// <remarks>
	/// Generally this property holds the default and basic difficulty of the step.
	/// If the step's difficulty rating requires multiple factors, this property will provide with a basic difficulty value
	/// as elementary and default rating value; other factors will be given in the other property <see cref="ExtraDifficultyFactors"/>.
	/// </remarks>
	/// <seealso cref="ExtraDifficultyFactors"/>
	public abstract decimal BaseDifficulty { get; }

	/// <summary>
	/// Indicates the difficulty of this technique step on locating.
	/// </summary>
	public virtual decimal BaseLocatingDifficulty { get; }

	/// <summary>
	/// Indicates the total difficulty of the technique step. This value is the total sum of merged result from two properties
	/// <see cref="BaseDifficulty"/> and <see cref="ExtraDifficultyFactors"/>. For property <see cref="ExtraDifficultyFactors"/>,
	/// the result is to sum all values up of inner property <see cref="ExtraDifficultyFactor.Value"/>.
	/// </summary>
	/// <seealso cref="BaseDifficulty"/>
	/// <seealso cref="ExtraDifficultyFactors"/>
	/// <seealso cref="ExtraDifficultyFactor"/>
	public decimal Difficulty => BaseDifficulty + (ExtraDifficultyFactors?.Sum(static @case => @case.Value) ?? 0);

	/// <summary>
	/// The technique code of this instance used for comparison (e.g. search for specified puzzle that contains this technique).
	/// </summary>
	public abstract Technique Code { get; }

	/// <summary>
	/// The difficulty level of this step.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generating puzzles.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when the target difficulty level is <see cref="DifficultyLevel.Unknown"/>.
	/// </exception>
	/// <seealso cref="FlagsAttribute"/>
	public DifficultyLevel DifficultyLevel
		=> Code.GetDifficultyLevel() is var level and not 0
			? level
			: throw new InvalidOperationException(ErrorInfo_TechniqueLevelCannotBeDetermined);

	/// <summary>
	/// Gets the format of the current instance.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="string"/> result. If the resource dictionary doesn't contain
	/// any valid formats here, the result value will be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// A <b>format</b> is a way to describe the details to a technique usage (i.e. <see cref="Step"/>).
	/// A format text will be combined with two parts: literals and placeholders. Here is an example:
	/// <code><![CDATA["Cells {0}, with digits {1}"]]></code>
	/// </para>
	/// <para>
	/// The placeholders <c>{0}</c> and <c>{1}</c> will use normal characters '<c>{</c>', digit characters '<c>0</c>' or '<c>1</c>'
	/// and '<c>}</c>'. The internal values will be offered by another property called <see cref="FormatInterpolationParts"/>.
	/// In that property, the value (an array) will contain 2 elements
	/// that can fill with the placeholders <c>{0}</c> and <c>{1}</c> respectively.
	/// </para>
	/// <para>
	/// The recommended implementation pattern is to declare with properties in the target type derived from <see cref="Step"/> instance like:
	/// <code><![CDATA[
	/// private string CellsStr => Cells.ToString();
	/// private string DigitsStr => Digits.ToString(", ");
	/// ]]></code>
	/// </para>
	/// <para>
	/// And then override the property <see cref="FormatInterpolationParts"/> like:
	/// <code><![CDATA[
	/// public override FormatInterpolation FormatInterpolationParts
	///     => [new("en-US", [CellsStr, DigitsStr]), new("zh-CN", [CellsStr, DigitsStr])];
	/// ]]></code>
	/// The culture name "<c>en-US</c>" and "<c>zh-CN</c>" stands for the target country or region is English and China (Mainland) respectively.
	/// If you don't determine which region should be declared, just remove suffixes like "<c>US</c>" and "<c>CN</c>".
	/// </para>
	/// <para>
	/// Please note the type of this property is <see cref="TechniqueFormat"/>, which is not a plain string text.
	/// However, you can specify the target value using interpolated strings like <c><![CDATA[$"UniqueRectangle{Type}Step"]]></c>,
	/// where the interpolation <c>Type</c> is an integer that describes the sub-type of the Unique Rectangle (e.g. 1-6 stands for UR type 1-6).
	/// The format text will be expanded to this expression in runtime:
	/// <code><![CDATA[
	/// var formatText = StringAccessor.GetString($"TechniqueFormat_UniqueRectangle{Type}Step");
	/// ]]></code>
	/// You can use this value to get the final text:
	/// <code><![CDATA[
	/// var culture = ...; // The culture string.
	/// var formatArguments = FormatInterpolationParts?.FirstOrDefault(culture).ResourcePlaceholderValues;
	/// var description = Format.ToString(formatArguments);
	/// ]]></code>
	/// See the documentation documents defined in method <see cref="ToString"/> to learn more information.
	/// </para>
	/// </remarks>
	/// <seealso cref="FormatInterpolationParts"/>
	/// <seealso cref="GetString(string)"/>
	/// <seealso cref="TechniqueFormat"/>
	/// <seealso cref="ToString"/>
	public virtual TechniqueFormat Format => $"{GetType().Name}";

	/// <summary>
	/// Indicates the interpolated parts that is used for the format.
	/// The formats will be interpolated into the property <see cref="Format"/> result.
	/// </summary>
	/// <seealso cref="Format"/>
	/// <seealso cref="FormatInterpolation"/>
	public virtual FormatInterpolation[]? FormatInterpolationParts => null;

	/// <summary>
	/// <para>Indicates the extra difficulty factors of the technique step.</para>
	/// <para>If the step does not contain such cases, this property will keep <see langword="null"/> value.</para>
	/// </summary>
	public virtual ExtraDifficultyFactor[]? ExtraDifficultyFactors => null;

	/// <summary>
	/// <para>Indicates the factors that can measure how difficult a technique pattern can be located by sudoku players.</para>
	/// <para><inheritdoc cref="ExtraDifficultyFactors" path="//summary/para[2]"/></para>
	/// </summary>
	public virtual LocatingDifficultyFactor[]? LocatingDifficultyFactors => null;

	/// <summary>
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	private protected string ConclusionText => Options.Converter.ConclusionConverter(Conclusions);


	/// <summary>
	/// Returns a string that only contains the name and the basic description.
	/// </summary>
	/// <returns>The string instance.</returns>
	public sealed override string ToString()
	{
		const StringComparison casingOption = StringComparison.CurrentCultureIgnoreCase;
		var currentCultureName = CultureInfo.CurrentCulture.Name;
		var colonToken = GetString("Colon");
		bool cultureMatcher(FormatInterpolation kvp) => currentCultureName.StartsWith(kvp.LanguageNameOrIdentifier, casingOption);
		return (Format, FormatInterpolationParts?.FirstOrDefault(cultureMatcher).ResourcePlaceholderValues) switch
		{
			({ TargetFormat: null }, _) => ToSimpleString(),
			(_, null) => $"{Name}{colonToken}{Format} => {ConclusionText}",
			var (_, formatArgs) => $"{Name}{colonToken}{Format.ToString(formatArgs)} => {ConclusionText}"
		};
	}

	/// <summary>
	/// Gets the string representation for the current step, describing only its technique name and conclusions.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ConclusionText}";
}
