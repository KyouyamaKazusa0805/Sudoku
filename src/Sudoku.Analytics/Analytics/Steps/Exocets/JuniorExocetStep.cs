namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet</b> technique.
/// </summary>
public sealed class JuniorExocetStep(
	View[]? views,
	Exocet exocet,
	short digitsMask,
#if false
	short lockedMemberQ,
	short lockedMemberR,
#endif
	ExocetElimination[] eliminations
) : ExocetStep(views, exocet, digitsMask, eliminations)
{
#if false
	/// <summary>
	/// Indicates the locked member bound with Q cells.
	/// </summary>
	public short LockedMemberQ { get; } = lockedMemberQ;

	/// <summary>
	/// Indicates the locked member bound with R cells.
	/// </summary>
	public short LockedMemberR { get; } = lockedMemberR;
#endif

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocet;

	/// <summary>
	/// Indicates the eliminations for raw usages.
	/// </summary>
	public ExocetElimination[] Eliminations { get; } = eliminations;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Mirror, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.Mirror) ? .1M : 0),
			new(ExtraDifficultyCaseNames.BiBiPattern, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.BiBiPattern) ? .3M : 0),
			new(ExtraDifficultyCaseNames.TargetPair, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.TargetPair) ? .1M : 0),
			new(ExtraDifficultyCaseNames.GeneralizedSwordfish, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.GeneralizedSwordfish) ? .2M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts => null;

#if false
	/// <summary>
	/// Indicates the locked member Q string.
	/// </summary>
	private string LockedMemberQStr
	{
		get
		{
			string? cells = LockedMemberQ == 0 ? null : DigitMaskFormatter.Format(LockedMemberQ, FormattingMode.Normal);
			return $"{R["LockedMember1"]}{cells}";
		}
	}

	/// <summary>
	/// Indicates the locked member R string.
	/// </summary>
	private string LockedMemberRStr
	{
		get
		{
			string? cells = LockedMemberR == 0 ? null : DigitMaskFormatter.Format(LockedMemberR, FormattingMode.Normal);
			return $"{R["LockedMember2"]}{cells}";
		}
	}
#endif
}
