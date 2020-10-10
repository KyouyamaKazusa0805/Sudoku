using System;
using System.Runtime.CompilerServices;

namespace Sudoku.Constants
{
	/// <summary>
	/// Provides operations for throwing exceptions.
	/// </summary>
	internal static class Throwings
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
		public static SwitchExpressionException ImpossibleCase => new("Impossible case.");

		/// <summary>
		/// Indicates all <see langword="ref struct"/>s can't
		/// use any boxing operations.
		/// </summary>
		public static NotSupportedException RefStructNotSupported => new("Ref structs can't use any boxing operations.");


		/// <summary>
		/// Indicates an exception throwing when the case is impossible
		/// in switch expressions or switch-case clauses.
		/// </summary>
		/// <param name="message">The specified message to display.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SwitchExpressionException ImpossibleCaseWithMessage(string message) => new(message);

		/// <summary>
		/// Indicates an exception throwing when the parsing is failed.
		/// </summary>
		/// <typeparam name="TTarget">The target type to parse.</typeparam>
		/// <param name="paramName">The name of the parameter.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException ParsingError<TTarget>(string paramName) =>
			new($"Argument can't be parsed and converted to target type {typeof(TTarget)}.", paramName);

		/// <summary>
		/// Indicates an exception throwing when the format string is error.
		/// </summary>
		/// <param name="message">The inner message to display.</param>
		/// <param name="paramName">The name of format string parameter.</param>
		/// <returns>The exception.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FormatException FormatErrorWithMessage(string message, string paramName) =>
			new("The specified format is invalid.", new ArgumentException(message, paramName));
	}
}
