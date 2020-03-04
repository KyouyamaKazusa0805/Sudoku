using System;
using System.Runtime.CompilerServices;

namespace Sudoku
{
	/// <summary>
	/// Provides operations for throwing exceptions.
	/// </summary>
	public static class Throwing
	{
		/// <summary>
		/// <para>
		/// Indicates an exception throwing when the case is impossible
		/// in switch expressions or switch-case clauses.
		/// </para>
		/// <para>
		/// This property will be used as a placeholder when expressions
		/// generate a warning (CS8509).
		/// </para>
		/// </summary>
		public static SwitchExpressionException ImpossibleCase =>
			new SwitchExpressionException("Impossible case.");


		/// <summary>
		/// Indicates an exception throwing when the case is impossible
		/// in switch expressions or switch-case clauses.
		/// </summary>
		/// <param name="message">The specified message to display.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SwitchExpressionException ImpossibleCaseWithMessage(string message) =>
			new SwitchExpressionException(message);

		/// <summary>
		/// Indicates an exception throwing when the parsing is failed.
		/// </summary>
		/// <typeparam name="TTarget">The target type to parse.</typeparam>
		/// <param name="paramName">The name of the parameter.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException ParsingError<TTarget>(string paramName)
		{
			return new ArgumentException(
				message: $"Argument cannot be parsed and converted to target type {typeof(TTarget)}.",
				paramName);
		}

		/// <summary>
		/// Indicates an exception throwing when the format string is error.
		/// </summary>
		/// <param name="message">The inner message to display.</param>
		/// <param name="paramName">The name of format string parameter.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FormatException FormatErrorWithMessage(string message, string paramName)
		{
			return new FormatException(
				message: "The specified format is invalid.",
				innerException: new ArgumentException(message, paramName));
		}
	}
}
