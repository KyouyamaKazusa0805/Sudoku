namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Provides with extension methods around enumeration type <see cref="ExocetShapeKind"/>.
/// </summary>
public static class ExocetShapeKindExtensions
{
	/// <summary>
	/// Try to get shape kind via the houses.
	/// </summary>
	/// <param name="this">The houses provider.</param>
	/// <returns>A <see cref="ExocetShapeKind"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ExocetShapeKind GetShapeKind(this IComplexSeniorExocet @this)
	{
		var finalMask = @this.CrosslineHousesMask | @this.ExtraHousesMask;
		return (
			finalMask & HouseMaskOperations.AllBlocksMask,
			finalMask & HouseMaskOperations.AllRowsMask,
			finalMask & HouseMaskOperations.AllColumnsMask
		) switch
		{
			(_, not 0, not 0) => ExocetShapeKind.Mutant,
			(not 0, _, _) => ExocetShapeKind.Franken,
			_ => ExocetShapeKind.Basic
		};
	}
}
