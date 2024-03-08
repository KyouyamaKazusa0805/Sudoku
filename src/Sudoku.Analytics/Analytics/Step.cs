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
[Equals(OtherModifiers = "sealed")]
[GetHashCode]
[EqualityOperators]
[ComparisonOperators]
public abstract partial class Step(
	[PrimaryConstructorParameter(SetterExpression = "internal set")] Conclusion[] conclusions,
	[PrimaryConstructorParameter] View[]? views,
	[PrimaryConstructorParameter] StepSearcherOptions options
) :
	IComparable<Step>,
	IComparisonOperators<Step, Step, bool>,
	ICultureFormattable,
	IEqualityOperators<Step, Step, bool>,
	IEquatable<Step>,
	IRenderable
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
	{
		get
		{
			var bits = 0b00;
			foreach (var conclusion in Conclusions)
			{
				bits |= conclusion.ConclusionType == Assignment ? 0b01 : 0b10;
			}

			return bits switch
			{
				0b11 => null,
				0b01 => true,
				0b10 => false,
				_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("StepContainsNoConclusions"))
			};
		}
	}

	/// <summary>
	/// Indicates the technique name.
	/// </summary>
	/// <remarks>
	/// The technique name are all stored in the resource dictionary,
	/// you can find them in the <c>Strings</c> folder (Type <see cref="ResourceDictionary"/>).
	/// </remarks>
	public virtual string Name => GetName(ResultCurrentCulture);

	/// <summary>
	/// Indicates the English name of the technique.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Name" path="/remarks"/>
	/// </remarks>
	/// <!--<exception cref="TargetResourceNotFoundException">Throws when the resource doesn't contain the name of the technique.</exception>-->
	public virtual string EnglishName
		=> Code.GetEnglishName() ?? throw new ResourceNotFoundException(GetType().Assembly, Code.ToString(), ResultCurrentCulture);

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
	/// Indicates the total difficulty of the technique step. This value is the total sum of merged result from two properties
	/// <see cref="BaseDifficulty"/> and <see cref="ExtraDifficultyFactors"/>. For property <see cref="ExtraDifficultyFactors"/>,
	/// the result is to sum all values up of inner property <see cref="ExtraDifficultyFactor.Value"/>.
	/// </summary>
	/// <seealso cref="BaseDifficulty"/>
	/// <seealso cref="ExtraDifficultyFactors"/>
	/// <seealso cref="ExtraDifficultyFactor"/>
	public decimal Difficulty => BaseDifficulty + ExtraDifficultyFactors.SumNullable(static @case => @case.Value, 0);

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
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("TechniqueLevelCannotBeDetermined"));

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
	/// var formatText = ResourceDictionary.Get($"TechniqueFormat_UniqueRectangle{Type}Step");
	/// ]]></code>
	/// You can use this value to get the final text:
	/// <code><![CDATA[
	/// var culture = ...; // The culture string.
	/// var formatArguments = FormatInterpolationParts?.FirstOrDefault(culture).ResourcePlaceholderValues;
	/// var description = Format.ToString(formatArguments);
	/// ]]></code>
	/// See the documentation documents defined in method <see cref="ToString(CultureInfo?)"/> to learn more information.
	/// </para>
	/// </remarks>
	/// <seealso cref="FormatInterpolationParts"/>
	/// <seealso cref="ResourceDictionary.Get(string, CultureInfo?, Assembly?)"/>
	/// <seealso cref="TechniqueFormat"/>
	/// <seealso cref="ToString(CultureInfo?)"/>
	public virtual TechniqueFormat Format => GetType().Name;

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
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	[HashCodeMember]
	private protected string ConclusionText => Options.Converter.ConclusionConverter(Conclusions);

	/// <summary>
	/// Indicates the result culture.
	/// </summary>
	private protected CultureInfo ResultCurrentCulture => Options.Converter.CurrentCulture ?? CultureInfo.CurrentUICulture;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual int CompareTo(Step? other) => other is null ? 1 : Math.Sign(Code - other.Code);

	/// <summary>
	/// Try to fetch the name of this technique step, with the specified culture.
	/// </summary>
	/// <param name="culture">The culture information instance.</param>
	/// <returns>The string representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string GetName(CultureInfo? culture) => Code.GetName(culture ?? ResultCurrentCulture);

	/// <summary>
	/// Returns a string that only contains the name and the basic description.
	/// </summary>
	/// <returns>The string instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => ToString(Options.Converter.CurrentCulture);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null)
	{
		const StringComparison casingOption = StringComparison.CurrentCultureIgnoreCase;
		var currentCultureName = (culture ?? ResultCurrentCulture).Name;
		var colonToken = ResourceDictionary.Get("Colon", culture ?? ResultCurrentCulture);
		bool cultureMatcher(FormatInterpolation kvp) => currentCultureName.StartsWith(kvp.LanguageNameOrIdentifier, casingOption);
		return (Format, FormatInterpolationParts?.FirstOrDefault(cultureMatcher).ResourcePlaceholderValues) switch
		{
			({ } p, _) when p.GetTargetFormat(null) is null => ToSimpleString(culture),
			(_, null) => $"{GetName(culture)}{colonToken}{Format} => {ConclusionText}",
			var (_, formatArgs) => $"{GetName(culture)}{colonToken}{Format.ToString(culture, formatArgs)} => {ConclusionText}"
		};
	}

	/// <summary>
	/// Gets the string representation for the current step, describing only its technique name and conclusions.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString(CultureInfo? culture = null)
		=> culture is null ? $"{Name} => {ConclusionText}" : $"{GetName(culture)} => {ConclusionText}";


	/// <summary>
	/// Sorts <typeparamref name="TStep"/> instances from the list collection.
	/// </summary>
	/// <typeparam name="TStep">The type of each step.</typeparam>
	/// <param name="accumulator">The accumulator instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SortItems<TStep>(List<TStep> accumulator) where TStep : Step
		=> accumulator.Sort(ValueComparison.Create<TStep>(static (l, r) => l.CompareTo(r)));

#pragma warning disable format
	/// <summary>
	/// Compares <typeparamref name="TStep"/> instances from the list collection,
	/// removing duplicate items by using <see cref="Equals(Step?)"/> to as equality comparison rules.
	/// </summary>
	/// <typeparam name="TStep">The type of each step.</typeparam>
	/// <param name="accumulator">The accumulator instance.</param>
	/// <returns>The final collection of <typeparamref name="TStep"/> instances.</returns>
	/// <seealso cref="Equals(Step?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<TStep> RemoveDuplicateItems<TStep>(List<TStep> accumulator) where TStep : Step
		=> accumulator switch
		{
			[] => [],
			[var firstElement] => [firstElement],
			[var a, var b] => a == b ? [a] : [a, b],
			_ => new HashSet<TStep>(accumulator, ValueComparison.Create<TStep>(static (x, y) => x == y, static v => v.GetHashCode()))
		};
#pragma warning restore format
}
