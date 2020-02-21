using System.Runtime.CompilerServices;

namespace System.Externals
{
	/// <summary>
	/// Provides operations for throwing exceptions.
	/// </summary>
	public static class Throwing
	{
		/// <summary>
		/// Indicates an exception throwing when the case is impossible
		/// in switch expressions or switch-case clauses.
		/// </summary>
		public static readonly SwitchExpressionException ImpossibleCase =
			new SwitchExpressionException("Impossible case.");
	}
}
