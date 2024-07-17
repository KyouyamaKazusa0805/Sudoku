namespace Sudoku.Analytics;

/// <summary>
/// Provides with a solving step that describes for a technique usage, with conclusions and detail data for the corresponding technique pattern.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="IDrawable.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="IDrawable.Views" path="/summary"/></param>
/// <param name="options">
/// Indicates an optional instance that provides with extra information for a step searcher.
/// This instance can be used for checking some extra information about a step such as notations to a cell, candidate, etc..
/// </param>
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators,
	OtherModifiersOnEquals = "sealed",
	OtherModifiersOnToString = "sealed")]
public abstract partial class Step(
	[PrimaryConstructorParameter(SetterExpression = "internal set")] Conclusion[] conclusions,
	[PrimaryConstructorParameter] View[]? views,
	[PrimaryConstructorParameter] StepSearcherOptions options
) :
	IComparable<Step>,
	IComparisonOperators<Step, Step, bool>,
	IDrawable,
	IEqualityOperators<Step, Step, bool>,
	IEquatable<Step>,
	IFormattable
{
	/// <summary>
	/// Indicates whether the step is an assignment. The possible result values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The step is assignment, meaning all conclusions are assignment ones</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The step is elimination, meaning all conclusions are elimination ones</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The step contains mixed conclusion types</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <exception cref="NotSupportedException">Throws when the step contains no conclusions.</exception>
	public bool? IsAssignment
		=> Conclusions.Aggregate(0, static (interim, next) => interim | (next.ConclusionType == Assignment ? 0b01 : 0b10)) switch
		{
			0b11 => null,
			0b01 => true,
			0b10 => false,
			_ => throw new NotSupportedException(SR.ExceptionMessage("StepContainsNoConclusions"))
		};

	/// <summary>
	/// Indicates the English name of the technique.
	/// </summary>
	public virtual string EnglishName => Code.GetEnglishName();

	/// <summary>
	/// Indicates the difficulty of this technique step.
	/// </summary>
	/// <remarks>
	/// Generally this property holds the default and basic difficulty of the step.
	/// If the step's difficulty rating requires multiple factors, this property will provide with a basic difficulty value
	/// as elementary and default rating value; other factors will be given in the other property <see cref="Factors"/>.
	/// </remarks>
	/// <seealso cref="Factors"/>
	public abstract int BaseDifficulty { get; }

	/// <summary>
	/// Indicates the total difficulty of the technique step. This value is the total sum of merged result from two properties
	/// <see cref="BaseDifficulty"/> and <see cref="Factors"/>.
	/// </summary>
	/// <seealso cref="BaseDifficulty"/>
	/// <seealso cref="Factors"/>
	/// <seealso cref="Factor"/>
	public int Difficulty => BaseDifficulty + Factors.Sum(this);

	/// <summary>
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	[HashCodeMember]
	public string ConclusionText => Options.Converter.ConclusionConverter(Conclusions);

	/// <summary>
	/// The technique code of this instance used for comparison (e.g. search for specified puzzle that contains this technique).
	/// </summary>
	[HashCodeMember]
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
			: throw new InvalidOperationException(SR.ExceptionMessage("TechniqueLevelCannotBeDetermined"));

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
	/// Please note the type of this property is <see cref="StepFormat"/>, which is not a plain string text.
	/// However, you can specify the target value using interpolated strings like <c><![CDATA[$"UniqueRectangle{Type}Step"]]></c>,
	/// where the interpolation <c>Type</c> is an integer that describes the sub-type of the Unique Rectangle (e.g. 1-6 stands for UR type 1-6).
	/// The format text will be expanded to this expression in runtime:
	/// <code><![CDATA[
	/// var formatText = ResourceDictionary.Get($"TechniqueFormat_UniqueRectangle{Type}Step");
	/// ]]></code>
	/// You can use this value to get the final text:
	/// <code><![CDATA[
	/// var culture = ...; // The culture string.
	/// var formatArguments = FormatInterpolationParts?.FirstOrDefault(culture).ResourcePlaceholderValues;
	/// var description = Format.ToString(formatArguments);
	/// ]]></code>
	/// See the documentation documents defined in method <see cref="ToString(IFormatProvider?)"/> to learn more information.
	/// </para>
	/// </remarks>
	/// <seealso cref="FormatInterpolationParts"/>
	/// <seealso cref="SR.Get(string, CultureInfo?, Assembly?)"/>
	/// <seealso cref="StepFormat"/>
	/// <seealso cref="ToString(IFormatProvider?)"/>
	public StepFormat Format => new(GetType().Name);

	/// <summary>
	/// Indicates the interpolated parts that is used for the format.
	/// The formats will be interpolated into the property <see cref="Format"/> result.
	/// </summary>
	/// <seealso cref="Format"/>
	/// <seealso cref="FormatInterpolation"/>
	public virtual FormatInterpolation[]? FormatInterpolationParts => null;

	/// <summary>
	/// Represents a collection of factors that describes the difficulty rating on extra values.
	/// </summary>
	public virtual FactorCollection Factors => [];


	/// <inheritdoc/>
	public virtual bool Equals([NotNullWhen(true)] Step? other)
		=> other is not null && (Code, ConclusionText) == (other.Code, other.ConclusionText);

	/// <summary>
	/// Compares two <see cref="Step"/> instances, determining which one is greater.
	/// </summary>
	/// <param name="other">The other object to be compared.</param>
	/// <returns>
	/// An <see cref="int"/> value indicating the result. The comparison rule is:
	/// <list type="number">
	/// <item>If the argument <paramref name="other"/> is <see langword="null"/>, return 1.</item>
	/// <item>
	/// If the argument <paramref name="other"/> isn't <see langword="null"/>, compare the technique used.
	/// If the code is greater, the instance will be greater.
	/// </item>
	/// </list>
	/// The rule (2) can also be replaced with customized logic
	/// if you want to make the comparison perform better and more strict.
	/// </returns>
	/// <remarks>
	/// <para>
	/// Please note that the argument can be <see langword="null"/>, which is expected. If the argument is not of the same type
	/// as <see langword="this"/>, we should return 1 to describe the comparison is not successful
	/// (the number 1 indicates <see langword="this"/> is greater).
	/// </para>
	/// <para>In addition, the return value must be -1, 0 or 1; otherwise, an unexpected behavior might be raised.</para>
	/// </remarks>
	public virtual int CompareTo(Step? other) => other is null ? -1 : Math.Sign(Code - other.Code);

	/// <summary>
	/// Try to fetch the name of this technique step, with the specified culture.
	/// </summary>
	/// <param name="formatProvider">The culture information provider instance.</param>
	/// <returns>The string representation.</returns>
	public virtual string GetName(IFormatProvider? formatProvider) => Code.GetName(GetCulture(formatProvider));

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var culture = GetCulture(formatProvider);
		var colonToken = SR.Get("Colon", culture);
		return (Format, FormatInterpolationParts?.FirstOrDefault(m).ResourcePlaceholderValues) switch
		{
			({ } p, _) when p.GetResourceFormat(null) is null => ToSimpleString(formatProvider),
			(_, null) => $"{GetName(formatProvider)}{colonToken}{Format} => {ConclusionText}",
			var (_, formatArgs) => $"{GetName(formatProvider)}{colonToken}{Format.Format(culture, formatArgs)} => {ConclusionText}"
		};


		bool m(FormatInterpolation kvp) => culture.Name.StartsWith(kvp.LanguageName, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	/// Gets the string representation for the current step, describing only its technique name and conclusions.
	/// </summary>
	/// <param name="formatProvider">The culture information.</param>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString(IFormatProvider? formatProvider) => $"{GetName(formatProvider)} => {ConclusionText}";

	/// <summary>
	/// Try to get the current culture used. The return value cannot be <see langword="null"/>.
	/// </summary>
	/// <param name="formatProvider">
	/// The format provider instance.
	/// The value can be <see langword="null"/> if you want to check for default culture used in <see cref="Options"/>.
	/// </param>
	/// <returns>The corresponding culture information.</returns>
	/// <seealso cref="Options"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected CultureInfo GetCulture(IFormatProvider? formatProvider)
		=> formatProvider as CultureInfo ?? Options.CurrentCulture;

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
