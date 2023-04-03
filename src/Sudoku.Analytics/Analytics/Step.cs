namespace Sudoku.Analytics;

/// <summary>
/// Provides with a logical solving step that is a technique usage, and contains the conclusions.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="IVisual.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="IVisual.Views" path="/summary"/></param>
public abstract class Step(Conclusion[] conclusions, View[]? views) : IVisual
{
	/// <summary>
	/// Indicates the technique name. The technique name are all stored in the resource dictionary,
	/// you can find them in the <c>Resources</c> folder (Type <see cref="MergedResources"/>).
	/// </summary>
	/// <exception cref="ResourceNotFoundException">Throws when the specified resource key is not found.</exception>
	public virtual string Name => R[Code.ToString()] ?? throw new ResourceNotFoundException(Code.ToString(), EqualityContract.Assembly);

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
	/// public override IDictionary<string, string[]?> FormatInterpolatedParts
	///     => [["en-US": [CellsStr, DigitsStr]], ["zh-CN": [CellsStr, DigitsStr]]];
	/// ]]></code>
	/// via the feature "<see href="https://github.com/dotnet/csharplang/issues/5354">Collection Literals</see>" introduced in C# 12.
	/// If you cannot decide the real name of the culture name, just use suffix instead like <c>"en"</c> and <c>"zh"</c>, ignoring cases.
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
	public virtual string? Format => R[$"TechniqueFormat_{EqualityContract.Name}"];

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
	/// The technique group that this technique instance belongs to.
	/// </summary>
	public virtual TechniqueGroup Group => Enum.TryParse<TechniqueGroup>(Code.ToString(), out var instance) ? instance : TechniqueGroup.None;

	/// <summary>
	/// The difficulty level of this step.
	/// </summary>
	/// <remarks>
	/// Although the type of this property is marked <see cref="FlagsAttribute"/>,
	/// we still can't set multiple flag values into the result. The flags are filtered
	/// during generating puzzles.
	/// </remarks>
	/// <seealso cref="FlagsAttribute"/>
	public abstract DifficultyLevel DifficultyLevel { get; }

	/// <summary>
	/// Indicates the extra difficulty cases of the technique step. If the step does not contain such cases,
	/// this property will keep <see langword="null"/> value.
	/// </summary>
	public virtual ExtraDifficultyCase[]? ExtraDifficultyCases => null;

	/// <inheritdoc/>
	public Conclusion[] Conclusions { get; } = conclusions;

	/// <inheritdoc/>
	public View[]? Views { get; } = views;

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
	public abstract IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts { get; }

	/// <summary>
	/// Indicates the string representation of the conclusions of the step.
	/// </summary>
	protected string ConclusionText => ConclusionFormatter.Format(Conclusions, FormattingMode.Normal);

	/// <summary>
	/// Returns a <see cref="Type"/> instance that specifies the type information of this current object.
	/// </summary>
	protected Type EqualityContract => GetType();


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	public void ApplyTo(scoped ref Grid grid)
	{
		foreach (var conclusion in Conclusions)
		{
			conclusion.ApplyTo(ref grid);
		}
	}

	/// <summary>
	/// Returns a string that only contains the name and the basic description.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks><i>
	/// This method uses modifiers <see langword="sealed"/> and <see langword="override"/> to prevent with compiler overriding this method.
	/// </i></remarks>
	public sealed override string ToString()
	{
		var currentCultureName = CultureInfo.CurrentCulture.Name;
		var formatArgs = FormatInterpolatedParts?.FirstOrDefault(c).Value;
		var colonToken = R.EmitPunctuation(Punctuation.Colon);
		return (Format, formatArgs) switch
		{
			(null, _) => ToSimpleString(),
			(_, null) => $"{Name}{colonToken}{Format} => {ConclusionText}",
			_ => $"{Name}{colonToken}{string.Format(Format, formatArgs)} => {ConclusionText}"
		};


		bool c(KeyValuePair<string, string[]?> kvp) => currentCultureName.StartsWith(kvp.Key, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ConclusionText}";
}
