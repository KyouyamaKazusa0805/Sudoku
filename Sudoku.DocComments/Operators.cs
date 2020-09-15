#pragma warning disable 0660 // Equals
#pragma warning disable 0661 // GetHashCode
#pragma warning disable IDE0060 // Unused parameters

using System;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides with doc comments for operators.
	/// </summary>
	public sealed class Operators
	{
		/// <summary>
		/// To check whether two instances are equal.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_Equality</c>.
		/// </remarks>
		public static bool operator ==(Operators left, Operators right) => throw new NotImplementedException();

		/// <summary>
		/// To check whether two instances are not equal, which means they hold difference values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_Inequality</c>.
		/// </remarks>
		public static bool operator !=(Operators left, Operators right) => throw new NotImplementedException();

		/// <summary>
		/// To determine whether the left instance is greater than the right one by their values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_GreaterThan</c>.
		/// </remarks>
		public static bool operator >(Operators left, Operators right) => throw new NotImplementedException();

		/// <summary>
		/// To determine whether the left instance is greater than or equal to the right one by their values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_GreaterThanOrEqual</c>.
		/// </remarks>
		public static bool operator >=(Operators left, Operators right) => throw new NotImplementedException();

		/// <summary>
		/// To determine whether the left instance is less than the right one by their values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_LessThan</c>.
		/// </remarks>
		public static bool operator <(Operators left, Operators right) => throw new NotImplementedException();

		/// <summary>
		/// To determine whether the left instance is less than or equal to the right one by their values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <remarks>
		/// This method is equivalent to the bottom-level method name called <c>op_LessThanOrEqual</c>.
		/// </remarks>
		public static bool operator <=(Operators left, Operators right) => throw new NotImplementedException();
	}
}
