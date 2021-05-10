#pragma warning disable CA1720

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Techniques
{
	/// <summary>
	/// Indicates a technique group.
	/// </summary>
	/// <remarks>
	/// Different with <see cref="TechniqueTags"/>, this enumeration type contains
	/// the real technique group that the technique belongs to. In addition, the value
	/// of this type may effect the displaying of the analysis result.
	/// </remarks>
	[Closed]
	public enum TechniqueGroup : byte
	{
		/// <summary>
		/// Indicates the technique doesn't belong to any group.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		None,

		/// <summary>
		/// Indicates the singles technique.
		/// </summary>
		Single,

		/// <summary>
		/// Indicates the locked candidates (LC) technique.
		/// </summary>
		Lc,

		/// <summary>
		/// Indicates the subset technique.
		/// </summary>
		Subset,

		/// <summary>
		/// Indicates the normal fish technique.
		/// </summary>
		NormalFish,

		/// <summary>
		/// Indicates the complex fish technique.
		/// </summary>
		ComplexFish,

		/// <summary>
		/// Indicates the wing technique.
		/// </summary>
		Wing,

		/// <summary>
		/// Indicates the empty rectangle technique.
		/// </summary>
		EmptyRectangle,

		/// <summary>
		/// Indicates the single digit pattern (SDP) technique.
		/// </summary>
		Sdp,

		/// <summary>
		/// Indicates the empty rectangle intersection pair (ERIP) technique.
		/// </summary>
		Erip,

		/// <summary>
		/// Indicates the almost locked candidates (ALC) technique.
		/// </summary>
		Alc,

		/// <summary>
		/// Indicates the alternating inference chain (AIC) technique.
		/// </summary>
		Aic,

		/// <summary>
		/// Indicates the forcing chains (FC) technique.
		/// </summary>
		Fc,

		/// <summary>
		/// Indicates the unique rectangle (UR) technique.
		/// </summary>
		Ur,

		/// <summary>
		/// Indicates the unique rectangle plus (UR+) technique.
		/// </summary>
		UrPlus,

		/// <summary>
		/// Indicates the unique loop (UL) technique.
		/// </summary>
		Ul,

		/// <summary>
		/// Indicates the extended rectangle (XR) technique.
		/// </summary>
		Xr,

		/// <summary>
		/// Indicates the bi-value universal grave (BUG) technique.
		/// </summary>
		Bug,

		/// <summary>
		/// Indicates the reverse bi-value universal grave (Reverse BUG) technique.
		/// </summary>
		ReverseBug,

		/// <summary>
		/// Indicates the deadly pattern technique.
		/// </summary>
		DeadlyPattern,

		/// <summary>
		/// Indicates the bi-value oddagon technique.
		/// </summary>
		BivalueOddagon,

		/// <summary>
		/// Indicates the sue de coq (SdC) technique.
		/// </summary>
		Sdc,

		/// <summary>
		/// Indicates the guardian technique.
		/// </summary>
		Guardian,

		/// <summary>
		/// Indicates the ALS chaining-like (ALS-XZ, ALS-XY-Wing, ALS-W-Wing) technique.
		/// </summary>
		AlsChainingLike,

		/// <summary>
		/// Indicates the SK-Loop technique.
		/// </summary>
		SkLoop,

		/// <summary>
		/// Indicates the multi-sector locked sets (MSLS) technique.
		/// </summary>
		Msls,

		/// <summary>
		/// Indicates the exocet technique.
		/// </summary>
		Exocet,

		/// <summary>
		/// Indicates the symmetry technique.
		/// </summary>
		Symmetry,

		/// <summary>
		/// Indicates the technique checked and searched relies on the rank theory.
		/// </summary>
		RankTheory,

		/// <summary>
		/// Indicates the bowman bingo technique.
		/// </summary>
		BowmanBingo,

		/// <summary>
		/// Indicates the pattern overlay method (POM) technique.
		/// </summary>
		Pom,

		/// <summary>
		/// Indicates the templating technique.
		/// </summary>
		Templating,

		/// <summary>
		/// Indicates the brute force (BF) technique.
		/// </summary>
		Bf,
	}
}
