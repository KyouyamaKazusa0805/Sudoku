#pragma warning disable 0660 // Equals
#pragma warning disable 0661 // GetHashCode
#pragma warning disable IDE0060 // Unused parameters

namespace Sudoku.DocComments
{
	public sealed class Operators
	{
		/// <summary>
		/// To check whether two instances are equal.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool operator ==(Operators left, Operators right) => true;

		/// <summary>
		/// To check whether two instances are not equal, which means they hold difference values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool operator !=(Operators left, Operators right) => false;
	}
}
