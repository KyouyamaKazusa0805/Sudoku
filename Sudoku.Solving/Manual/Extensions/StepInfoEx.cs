using System.Runtime.CompilerServices;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="StepInfo"/>.
	/// </summary>
	/// <seealso cref="StepInfo"/>
	public static class StepInfoEx
	{
		/// <summary>
		/// Check whether the current technique information is an ALS technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlsTechnique(this StepInfo @this) => @this.HasTag(TechniqueFlags.Als);

		/// <summary>
		/// Check whether the current technique information is a chaining-ruled technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsChainingTechnique(this StepInfo @this) =>
			@this.HasTag(TechniqueFlags.Wings | TechniqueFlags.ShortChaining | TechniqueFlags.LongChaining);

		/// <summary>
		/// Check whether the current technique information is a uniqueness technique.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The technique instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUniqueness(this StepInfo @this) => @this.HasTag(TechniqueFlags.DeadlyPattern);
	}
}
