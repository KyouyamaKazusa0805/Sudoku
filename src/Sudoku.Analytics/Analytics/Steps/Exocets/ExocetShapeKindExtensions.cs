using System.Runtime.CompilerServices;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with extension methods around enumeration type <see cref="ExocetShapeKind"/>.
/// </summary>
internal static class ExocetShapeKindExtensions
{
	/// <summary>
	/// Try to get shape kind via the houses.
	/// </summary>
	/// <param name="this">The houses provider.</param>
	/// <returns>A <see cref="ExocetShapeKind"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ExocetShapeKind GetShapeKind(this ComplexSeniorExocetBaseStep @this)
	{
		var finalMask = @this.CrosslineHousesMask | @this.ExtraHousesMask;
		return (finalMask & AllBlocksMask, finalMask & AllRowsMask, finalMask & AllColumnsMask) switch
		{
			(_, not 0, not 0) => ExocetShapeKind.Mutant,
			(not 0, _, _) => ExocetShapeKind.Franken,
			_ => ExocetShapeKind.Basic
		};
	}

	/// <inheritdoc cref="GetShapeKind(ComplexSeniorExocetBaseStep)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ExocetShapeKind GetShapeKind(this ComplexSeniorExocetCompatiablePairStep @this)
	{
		var finalMask = @this.CrosslineHousesMask | @this.ExtraHousesMask;
		return (finalMask & AllBlocksMask, finalMask & AllRowsMask, finalMask & AllColumnsMask) switch
		{
			(_, not 0, not 0) => ExocetShapeKind.Mutant,
			(not 0, _, _) => ExocetShapeKind.Franken,
			_ => ExocetShapeKind.Basic
		};
	}
}
