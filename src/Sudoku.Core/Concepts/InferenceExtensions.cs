namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Inference"/>.
/// </summary>
/// <seealso cref="Inference"/>
public static class InferenceExtensions
{
	/// <summary>
	/// Gets connecting notation of the inference, with two spaces sandwiching the characters.
	/// </summary>
	/// <param name="this">The inference instance.</param>
	/// <returns>The connecting notation of the inference.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="this"/> is not defined in the enumeration type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ConnectingNotation(this Inference @this)
		=> @this switch
		{
			Inference.Strong => " == ",
			Inference.Weak => " -- ",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
