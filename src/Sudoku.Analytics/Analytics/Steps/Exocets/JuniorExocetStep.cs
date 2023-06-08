namespace Sudoku.Analytics.Steps;

#pragma warning disable CS1572
/// <summary>
/// Provides with a step that is a <b>Junior Exocet</b> technique.
/// </summary>
/// <param name="views"><inheritdoc/></param>
/// <param name="exocet"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="lockedMemberQ">Indicates the locked member bound with Q cells.</param>
/// <param name="lockedMemberR">Indicates the locked member bound with R cells.</param>
/// <param name="eliminations"><inheritdoc/></param>
#pragma warning restore CS1572
public sealed partial class JuniorExocetStep(
	View[]? views,
	Exocet exocet,
	Mask digitsMask,
#if false
	[PrimaryConstructorParameter] Mask lockedMemberQ,
	[PrimaryConstructorParameter] Mask lockedMemberR,
#endif
	[PrimaryConstructorParameter] ExocetElimination[] eliminations
) : ExocetStep(views, exocet, digitsMask, eliminations)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocet;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Mirror, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.Mirror) ? .1M : 0),
			(ExtraDifficultyCaseNames.BiBiPattern, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.BiBiPattern) ? .3M : 0),
			(ExtraDifficultyCaseNames.TargetPair, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.TargetPair) ? .1M : 0),
			(ExtraDifficultyCaseNames.GeneralizedSwordfish, Eliminations.Any(static e => e.Reason == ExocetEliminatedReason.GeneralizedSwordfish) ? .2M : 0)
		};

#if false
	/// <summary>
	/// Indicates the locked member Q string.
	/// </summary>
	private string LockedMemberQStr
	{
		get
		{
			string? cells = LockedMemberQ == 0 ? null : DigitMaskFormatter.Format(LockedMemberQ, FormattingMode.Normal);
			return $"{GetString("LockedMember1")}{cells}";
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
			return $"{GetString("LockedMember2")}{cells}";
		}
	}
#endif
}
