namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents a type that encapsulates a list of options adjusted by users and used by <see cref="StepSearcher"/> instances.
/// Some options may not relate to a real <see cref="StepSearcher"/> instance directly, but relate to a <see cref="Step"/>
/// that a <see cref="StepSearcher"/> instance can create.
/// For example, setting notation to the coordinates.
/// </summary>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="Analyzer"/>
public sealed record StepSearcherOptions : IStepSearcherOptions<StepSearcherOptions>
{
	/// <summary>
	/// Indicates whether the step searchers will adjust the searching order to distinct two modes on displaying candidates,
	/// making the experience better.
	/// </summary>
	public bool DistinctDirectMode { get; init; } = false;

	/// <summary>
	/// Indicates whether the current solver uses direct mode to solve a puzzle,
	/// meaning UI will display the grid without any candidates.
	/// </summary>
	public bool IsDirectMode { get; init; } = false;

	/// <summary>
	/// Indicates whether the step searcher will checks for hidden singles in rows or columns
	/// if <see cref="PrimarySingle"/> property has flag <see cref="SingleTechniqueFlag.HiddenSingle"/>.
	/// </summary>
	/// <seealso cref="SingleTechniqueFlag.HiddenSingle"/>
	public bool PrimaryHiddenSingleAllowsLines { get; init; } = false;

	/// <summary>
	/// Indicates the preferred single technique. If the value isn't (<see cref="SingleTechniqueFlag"/>)0,
	/// the analyzer will ignore the other single technique not related to this field.
	/// </summary>
	/// <remarks>
	/// For example, if the value is <see cref="SingleTechniqueFlag.HiddenSingle"/>,
	/// the analyzer will automatically ignore naked single steps to analyze the grid.
	/// If the puzzle cannot be solved, the analyzer will return an <see cref="AnalysisResult"/> with
	/// <see cref="AnalysisResult.IsSolved"/> a <see langword="false"/> value.
	/// </remarks>
	/// <seealso cref="SingleTechniqueFlag"/>
	/// <seealso cref="AnalysisResult.IsSolved"/>
	public SingleTechniqueFlag PrimarySingle { get; init; } = 0;

	/// <summary>
	/// Indicates the initial letter to be used and displayed in a <see cref="BabaGroupViewNode"/>.
	/// </summary>
	/// <seealso cref="BabaGroupViewNode"/>
	public BabaGroupInitialLetter BabaGroupInitialLetter { get; init; } = BabaGroupInitialLetter.EnglishLetter_X;

	/// <summary>
	/// Indicates letter casing of characters in <see cref="BabaGroupViewNode"/> should be displayed.
	/// </summary>
	/// <seealso cref="BabaGroupViewNode"/>
	public BabaGroupLetterCasing BabaGroupLetterCasing { get; init; } = BabaGroupLetterCasing.Lower;

	/// <summary>
	/// Indicates the default link option.
	/// </summary>
	public LinkOption DefaultLinkOption { get; init; } = LinkOption.House;

	/// <inheritdoc cref="CoordinateConverter"/>
	public CoordinateConverter Converter { get; init; } = CoordinateConverter.InvariantCultureInstance;

	/// <summary>
	/// Indicates the current culture used.
	/// </summary>
	public CultureInfo CurrentCulture => Converter.CurrentCulture ?? CultureInfo.CurrentUICulture;

	/// <summary>
	/// Indicates the link options overridden.
	/// </summary>
	public IDictionary<LinkType, LinkOption> OverriddenLinkOptions { get; } = new Dictionary<LinkType, LinkOption>();


	/// <inheritdoc/>
	/// <remarks>
	/// This default option makes the internal members be:
	/// <list type="bullet">
	/// <item><see cref="Converter"/>: <see cref="RxCyConverter"/></item>
	/// <item><see cref="DistinctDirectMode"/>: <see langword="false"/></item>
	/// <item><see cref="IsDirectMode"/>: <see langword="false"/></item>
	/// <item><see cref="BabaGroupInitialLetter"/>: <see cref="BabaGroupInitialLetter.EnglishLetter_X"/></item>
	/// <item><see cref="BabaGroupLetterCasing"/>: <see cref="BabaGroupLetterCasing.Lower"/></item>
	/// <item><see cref="DefaultLinkOption"/>: <see cref="LinkOption.House"/></item>
	/// <item><see cref="OverriddenLinkOptions"/>: <c>[]</c></item>
	/// <item><see cref="PrimarySingle"/>: <see cref="SingleTechniqueFlag.None"/></item>
	/// <item><see cref="PrimaryHiddenSingleAllowsLines"/>: <see langword="false"/></item>
	/// </list>
	/// </remarks>
	public static StepSearcherOptions Default => new();
}
