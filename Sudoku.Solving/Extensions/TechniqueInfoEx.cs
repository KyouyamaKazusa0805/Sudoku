using System.Runtime.CompilerServices;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Uniqueness;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="TechniqueInfo"/>.
	/// </summary>
	/// <seealso cref="TechniqueInfo"/>
	public static class TechniqueInfoEx
	{
		/// <summary>
		/// Check whether the currrent technique information is an ALS technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlsTechnique(this TechniqueInfo @this) =>
			@this is DbTechniqueInfo or AlsXzTechniqueInfo or AlsXyWingTechniqueInfo or AlsWWingTechniqueInfo;

		/// <summary>
		/// Check whether the currrent technique information is a chaining-ruled technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsChainingTechnique(this TechniqueInfo @this) =>
			@this is ChainingTechniqueInfo or BowmanBingoTechniqueInfo;

		/// <summary>
		/// Check whether the currrent technique information is a forcing chains technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsForcingChainsTechnique(this TechniqueInfo @this) =>
			@this is BowmanBingoTechniqueInfo or RegionChainingTechniqueInfo or CellChainingTechniqueInfo;

		/// <summary>
		/// Check whether the currrent technique information is a uniqueness technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUniqueness(this TechniqueInfo @this) => @this is UniquenessTechniqueInfo;
	}
}
