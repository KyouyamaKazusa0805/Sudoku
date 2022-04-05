namespace Sudoku.Concepts;

/// <summary>
/// Provides the extensions methods on <see cref="Inference"/>.
/// </summary>
/// <seealso cref="Inference"/>
public static class InferenceExtensions
{
	/// <summary>
	/// Gets the identifier of the inference.
	/// </summary>
	/// <param name="this">The inference.</param>
	/// <returns>The identifier value.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="this"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetIdentifier(this Inference @this) =>
		typeof(Inference).GetField(@this.ToString())?.GetCustomAttribute<EnumFieldNameAttribute>()?.Name
			?? throw new ArgumentOutOfRangeException(nameof(@this));
}
