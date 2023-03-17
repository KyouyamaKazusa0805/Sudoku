namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop</b> technique.
/// </summary>
public abstract class UniqueLoopStep(Conclusion[] conclusions, View[]? views, int digit1, int digit2, scoped in CellMap loop) :
	DeadlyPatternStep(conclusions, views),
	IEquatableStep<UniqueLoopStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the first digit used.
	/// </summary>
	public int Digit1 { get; } = digit1;

	/// <summary>
	/// Indicates the second digit used.
	/// </summary>
	public int Digit2 { get; } = digit2;

	/// <summary>
	/// Indicates the type.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	[AllowNull]
	[MaybeNull]
	public sealed override string Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"UniqueLoopType{Type}");

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.UniqueLoop;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the whole loop of cells used.
	/// </summary>
	public CellMap Loop { get; } = loop;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Length, (A004526(Loop.Count) - 3) * .1M) };

	private protected string LoopStr => Loop.ToString();

	private protected string Digit1Str => (Digit1 + 1).ToString();

	private protected string Digit2Str => (Digit2 + 1).ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UniqueLoopStep left, UniqueLoopStep right)
		=> (left.Type, left.Loop, left.Digit1, left.Digit2) == (right.Type, right.Loop, right.Digit1, right.Digit2)
		&& (left, right) switch
		{
			(UniqueLoopType3Step { SubsetDigitsMask: var a }, UniqueLoopType3Step { SubsetDigitsMask: var b }) => a == b,
			(UniqueLoopType4Step { ConjugatePair: var a }, UniqueLoopType4Step { ConjugatePair: var b }) => a == b,
			_ => true
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UniqueLoopStep left, UniqueLoopStep right) => !(left == right);
}
