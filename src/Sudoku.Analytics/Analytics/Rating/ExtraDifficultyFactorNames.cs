namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents with kinds of the phased difficulty rating that is used for the calculations the total rating of a step.
/// </summary>
public static class ExtraDifficultyFactorNames
{
	/// <summary>
	/// Indicates the kind is the size. This field is used for sized techniques,
	/// such as deadly patterns of type 3, naked or hidden subsets, fishes.
	/// </summary>
	public const string Size = nameof(Size);

	/// <summary>
	/// Indicates the kind is the extra digit. This field is used for deadly patterns of type 2.
	/// </summary>
	public const string ExtraDigit = nameof(ExtraDigit);

	/// <summary>
	/// Indicates the kind is the sashimi. This field is only used for fishes.
	/// </summary>
	public const string Sashimi = nameof(Sashimi);

	/// <summary>
	/// Indicates the kind is the fish shape. This field is only used for complex fishes.
	/// </summary>
	public const string FishShape = nameof(FishShape);

	/// <summary>
	/// Indicates the kind is the hidden. This field is used for some techniques that has hidden views.
	/// </summary>
	public const string Hidden = nameof(Hidden);

	/// <summary>
	/// Indicates the kind is the locked. This field is only used for naked subsets.
	/// </summary>
	public const string Locked = nameof(Locked);

	/// <summary>
	/// Indicates the kind is the locked digit. This field is only used for Locked Qiu's Deadly Pattern.
	/// </summary>
	public const string LockedDigit = nameof(LockedDigit);

	/// <summary>
	/// Indicates the kind is the length. This field is used for variable-length chaining techniques.
	/// </summary>
	public const string Length = nameof(Length);

	/// <summary>
	/// Indicates the kind is the conjugate pair. This field is used for deadly patterns of type 4,
	/// or advanced Unique Rectangle techniques.
	/// </summary>
	public const string ConjugatePair = nameof(ConjugatePair);

	/// <summary>
	/// Indicates the kind is the guardian. This field is used for some guardian-related techniques,
	/// such as guardian itself, Unique Rectangle + Guardian (i.e. Unique Rectangle External Types).
	/// </summary>
	public const string Guardian = nameof(Guardian);

	/// <summary>
	/// Indicates the number of petals used in death blossom.
	/// </summary>
	public const string Petals = nameof(Petals);

	/// <summary>
	/// Indicates the kind is the value cell used in the pattern. This field is used for Almost Locked Candidates.
	/// </summary>
	public const string ValueCell = nameof(ValueCell);

	/// <summary>
	/// Indicates the kind is the pattern use grouped nodes, such as irregular wings and chains.
	/// </summary>
	public const string IsGrouped = nameof(IsGrouped);

	/// <summary>
	/// Indicates the kind is the incompleteness. This field is used for Unique Rectangle incomplete types.
	/// </summary>
	public const string Incompleteness = nameof(Incompleteness);

	/// <summary>
	/// Indicates the kind is the avoidable. This field is used for avoidable rectangles.
	/// </summary>
	public const string Avoidable = nameof(Avoidable);

	/// <summary>
	/// Indicates the kind is the wing size. This field is used for Unique Rectangle + Wings,
	/// or Regular Wing technique itself.
	/// </summary>
	public const string WingSize = nameof(WingSize);

	/// <summary>
	/// Indicates the kind is the isolated digits. This field is used for Sue de Coq variant types,
	/// or Unique Rectangle + Sue de Coq.
	/// </summary>
	public const string Isolated = nameof(Isolated);

	/// <summary>
	/// Indicates the kind is the cannibalism. This field is used for Sue de Coq variant types.
	/// </summary>
	public const string Cannibalism = nameof(Cannibalism);

	/// <summary>
	/// Indicates the kind is the mirror eliminations. This field is only used for exocets.
	/// </summary>
	public const string Mirror = nameof(Mirror);

	/// <summary>
	/// Indicates the kind is the incompatible pair eliminations. This field is only used for exocets.
	/// </summary>
	public const string IncompatiblePair = nameof(IncompatiblePair);

	/// <summary>
	/// Indicates the kind is the target pair eliminations. This field is only used for exocets.
	/// </summary>
	public const string TargetPair = nameof(TargetPair);

	/// <summary>
	/// Indicates the kind is the generalized fish eliminations. This field is only used for exocets.
	/// </summary>
	public const string GeneralizedFish = nameof(GeneralizedFish);

	/// <summary>
	/// Indicates the kind is the almost hidden set. This field is only used for exocets.
	/// </summary>
	public const string AlmostHiddenSet = nameof(AlmostHiddenSet);

	/// <summary>
	/// Indicates the kind is the locked member. This field is only used for exocets.
	/// </summary>
	public const string LockedMember = nameof(LockedMember);

	/// <summary>
	/// Indicates the kind is the true base. This field is only used for senior exocets and complex senior exocets.
	/// </summary>
	public const string TrueBase = nameof(TrueBase);

	/// <summary>
	/// Indicates the kind is the extra house. This field is only used for complex senior exocets.
	/// </summary>
	public const string ExtraHouse = nameof(ExtraHouse);

	/// <summary>
	/// Indicates the kind is the slash pattern elimination. This field is only used for weak exocets.
	/// </summary>
	public const string SlashElimination = nameof(SlashElimination);

	/// <summary>
	/// Indicates the kind is the BZ rectangle pattern elimination. This field is only used for weak exocets.
	/// </summary>
	public const string BzRectangle = nameof(BzRectangle);

	/// <summary>
	/// Indicates the kind is the stability balancer is missing. This field is only used for lame weak exocets.
	/// </summary>
	public const string MissingStabilityBalancer = nameof(MissingStabilityBalancer);
}
