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
		Singles,

		/// <summary>
		/// Indicates the intersection technique.
		/// </summary>
		Intersections,

		/// <summary>
		/// Indicates the subset technique.
		/// </summary>
		Subsets,

		/// <summary>
		/// Indicates the normal fish technique.
		/// </summary>
		Fishes,

		/// <summary>
		/// Indicates the complex fish technique.
		/// </summary>
		ComplexFishes,

		/// <summary>
		/// Indicates the wing technique.
		/// </summary>
		Wings,

		/// <summary>
		/// Indicates the single digit pattern technique.
		/// </summary>
		SingleDigitPatterns,

		/// <summary>
		/// Indicates the AIC technique.
		/// </summary>
		AlternatingInferenceChaining,

		/// <summary>
		/// Indicates the forcing chains technique.
		/// </summary>
		ForcingChaining,

		/// <summary>
		/// Indicates the unique rectangle technique.
		/// </summary>
		UniqueRectangle,

		/// <summary>
		/// Indicates the unique rectangle (+) techique.
		/// </summary>
		UniqueRectanglePlus,

		/// <summary>
		/// Indicates the unique loop technique.
		/// </summary>
		UniqueLoop,

		/// <summary>
		/// Indicates the extended rectangle technique.
		/// </summary>
		ExtendedRectangle,

		/// <summary>
		/// Indicates the deadly pattern technique.
		/// </summary>
		DeadlyPattern,

		/// <summary>
		/// Indicates the ALS chaining-like (ALS-XZ, ALS-XY-Wing, ALS-W-Wing) technique.
		/// </summary>
		AlsChainingLike,

		/// <summary>
		/// Indicates the MSLS technique.
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
		RankTheory
	}
}
