using System.Runtime.CompilerServices;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Uniqueness;

namespace Sudoku.Solving.Manual.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="StepInfo"/>.
	/// </summary>
	/// <seealso cref="StepInfo"/>
	public static class StepInfoEx
	{
		/// <summary>
		/// Check whether the currrent technique information is an ALS technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlsTechnique(this StepInfo @this) =>
			@this is DeathBlossomStepInfo
			or AlsXzStepInfo or AlsXyWingStepInfo or AlsWWingStepInfo;

		/// <summary>
		/// Check whether the currrent technique information is a chaining-ruled technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsChainingTechnique(this StepInfo @this) =>
			@this is ChainingStepInfo or BowmanBingoStepInfo;

		/// <summary>
		/// Check whether the currrent technique information is a forcing chains technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsForcingChainsTechnique(this StepInfo @this) =>
			@this is BowmanBingoStepInfo or RegionChainingStepInfo or CellChainingStepInfo;

		/// <summary>
		/// Check whether the currrent technique information is a uniqueness technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUniqueness(this StepInfo @this) => @this is UniquenessStepInfo;
	}
}
