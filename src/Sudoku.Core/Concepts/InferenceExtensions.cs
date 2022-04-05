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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetIdentifier(this Inference @this) =>
		@this switch
		{
			Inference.Strong => " == ",
			Inference.Weak => " -- ",
			Inference.StrongGeneralized => " =~ ",
			Inference.WeakGeneralized => " -~ ",
			Inference.ConjuagtePair => " == ",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
