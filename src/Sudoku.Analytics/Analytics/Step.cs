namespace Sudoku.Analytics;

/// <summary>
/// Provides with a solving step that describes for a technique usage, with conclusions and detail data for the corresponding technique pattern.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="IRenderable.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="IRenderable.Views" path="/summary"/></param>
[GetHashCode(OtherModifiers = "sealed")]
public abstract partial class Step([PrimaryConstructorParameter] Conclusion[] conclusions, [PrimaryConstructorParameter] View[]? views) :
	IRenderable
{
	/// <summary>
	/// The placeholder of the hash code.
	/// </summary>
	[HashCodeMember]
	private const int HashCodeDefaultValue = 0;


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

	/// <summary>s
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
	/// public override IDictionary<string, string[]?> FormatInterpolatedParts
	///     => new Dictionary<string, string[]?>
	///     {
	///         { "en-US", [CellsStr, DigitsStr] },
	///         { "zh-CN", [CellsStr, DigitsStr] }
	///     };
	/// ]]></code>
	/// via the feature "<see href="https://github.com/dotnet/csharplang/issues/5354">Collection Expressions</see>" introduced in C# 12.
	/// If you cannot decide the real name of the culture name, just use suffix instead like <c>"en"</c> and <c>"zh"</c>, ignoring cases.
	/// </para>
	/// <para>
	/// If you want to use the values in the resource dictionary, you can just use method <see cref="GetString(string)"/>, for example:
	/// <code><![CDATA[
	/// public override string Format => Sudoku.Analytics.Strings.StringsAccessor.GetString("TheKeyYouWantToSearch");
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="FormatInterpolatedParts"/>
	/// <seealso cref="GetString(string)"/>
	public virtual string? Format => GetString($"TechniqueFormat_{GetType().Name}");

	/// <summary>
	/// Indicates the difficulty of this technique step.
	/// </summary>
	/// <remarks>
	/// Generally this property holds the default and basic difficulty of the step.
	/// If the step's difficulty rating requires multiple factors, this property will provide with a basic difficulty value
	/// as elementary and default rating value; other factors will be given in the other property <see cref="ExtraDifficultyCases"/>.
	/// </remarks>
	/// <seealso cref="ExtraDifficultyCases"/>
	public abstract decimal BaseDifficulty { get; }

	/// <summary>
	/// Indicates the total difficulty of the technique step. This value is the total sum of merged result from two properties
	/// <see cref="BaseDifficulty"/> and <see cref="ExtraDifficultyCases"/>. For property <see cref="ExtraDifficultyCases"/>,
	/// the result is to sum all values up of inner property <see cref="ExtraDifficultyCase.Value"/>.
	/// </summary>
	/// <seealso cref="BaseDifficulty"/>
	/// <seealso cref="ExtraDifficultyCases"/>
	/// <seealso cref="ExtraDifficultyCase"/>
	public decimal Difficulty => BaseDifficulty + (ExtraDifficultyCases?.Sum(static @case => @case.Value) ?? 0);

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
		=> Code.GetDifficultyLevel() switch
		{
			var level and not 0 => level,
			_ => throw new InvalidOperationException(
				$"""
				The target level is unknown. If you see this exception thrown, 
				please append '{typeof(DifficultyLevelAttribute).FullName}' to the target technique code field 
				defined in type '{typeof(Technique).FullName}'.
				""".RemoveLineEndings()
			)
		};

	/// <summary>
	/// Indicates the extra difficulty cases of the technique step. If the step does not contain such cases,
	/// this property will keep <see langword="null"/> value.
	/// </summary>
	public virtual ExtraDifficultyCase[]? ExtraDifficultyCases => null;

	/// <summary>
	/// Indicates the interpolated parts that is used for the format.
	/// The formats will be interpolated into the property <see cref="Format"/> result.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property use a dictionary to merge format data of globalization.
	/// The key type is <see cref="string"/>, which can be used for the comparison of the current culture via type <see cref="CultureInfo"/>,
	/// for example, <c>"zh"</c> and <c>"en-US"</c>.
	/// </para>
	/// <para>For more backing implementation details, please visit method <see cref="ToString"/> in derived <see langword="class"/>es.</para>
	/// </remarks>
	/// <seealso cref="Format"/>
	/// <seealso cref="CultureInfo"/>
	/// <seealso cref="ToString"/>
	public virtual IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts => null;

	/// <summary>
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	private protected string ConclusionText => ConclusionFormatter.Format(Conclusions, FormattingMode.Normal);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		const BindingFlags staticMethodFlag = BindingFlags.NonPublic | BindingFlags.Static;
		var (equalityContract, currentTypeEqualityContract) = (GetType(), typeof(Step));
		return (
			obj?.GetType() != equalityContract,
			equalityContract.IsGenericAssignableTo(typeof(IEquatableStep<>)),
			equalityContract.IsGenericAssignableTo(typeof(IComparableStep<>))
		) switch
		{
			(true, _, _) => false,
			(_, true, _) => (bool)currentTypeEqualityContract.GetMethod(nameof(EquatableStepEntry), staticMethodFlag)!
				.MakeGenericMethod(equalityContract)
				.Invoke(null, [this, obj])!,
			(_, _, true) => (int)currentTypeEqualityContract.GetMethod(nameof(ComparableStepEntry), staticMethodFlag)!
				.MakeGenericMethod(equalityContract)
				.Invoke(null, [this, obj])! == 0,
			_ => false
		};
	}

	/// <summary>
	/// Returns a string that only contains the name and the basic description.
	/// </summary>
	/// <returns>The string instance.</returns>
	public sealed override string ToString()
	{
		const StringComparison casingOption = StringComparison.CurrentCultureIgnoreCase;
		var currentCultureName = CultureInfo.CurrentCulture.Name;
		var colonToken = GetString("Colon");
		return (Format, FormatInterpolatedParts?.FirstOrDefault(kvp => currentCultureName.StartsWith(kvp.Key, casingOption)).Value) switch
		{
			(null, _) => ToSimpleString(),
			(_, null) => $"{Name}{colonToken}{Format} => {ConclusionText}",
			var (_, formatArgs) => $"{Name}{colonToken}{string.Format(Format, formatArgs)} => {ConclusionText}"
		};
	}

	/// <summary>
	/// Gets the string representation for the current step, describing only its technique name and conclusions.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ConclusionText}";


	/// <summary>
	/// The entry method to visit operators defined in type <see cref="IEquatableStep{TSelf}"/>.
	/// </summary>
	private static bool EquatableStepEntry<T>(T left, T right) where T : Step, IEquatableStep<T> => left == right;

	/// <summary>
	/// The entry method to visit methods defined in type <see cref="IComparableStep{TSelf}"/>.
	/// </summary>
	private static int ComparableStepEntry<T>(T left, T right) where T : Step, IComparableStep<T> => T.Compare(left, right);
}
