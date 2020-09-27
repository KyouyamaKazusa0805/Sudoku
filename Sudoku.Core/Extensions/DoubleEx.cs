using System;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="double"/>.
	/// </summary>
	/// <seealso cref="double"/>
	public static class DoubleEx
	{
		/// <summary>
		/// Indicates whether the specified value is nearly equals to the current value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="other">The other value.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool NearlyEquals(this double @this, double other) => @this.NearlyEquals(other, double.Epsilon);

		/// <summary>
		/// Indicates whether the specified value is nearly equals to the current value.
		/// If the differ of two values to compare is lower than the specified epsilon value,
		/// the method will return <see langword="true"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="other">The other value to compare.</param>
		/// <param name="epsilon">The epsilon value (the minimal differ).</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool NearlyEquals(this double @this, double other, double epsilon) => Math.Abs(@this - other) < epsilon;
	}
}
