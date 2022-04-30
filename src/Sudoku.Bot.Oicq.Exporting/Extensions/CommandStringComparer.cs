#nullable enable

namespace Sudoku.Bot.Oicq;

/// <summary>
/// Defines a string comparer that can recorgnize the command raw values.
/// </summary>
internal static class CommandStringComparer
{
	/// <summary>
	/// Compares two <see cref="string"/>s.
	/// </summary>
	/// <param name="a">The first <see cref="string"/> to be compared.</param>
	/// <param name="b">The second <see cref="string"/> to be compared.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AreEqual(string? a, string? b)
		=> (a, b) switch
		{
			(null, null) => true,
			(not null, not null) => a.Equals(b, StringComparison.OrdinalIgnoreCase),
			_ => false
		};
}
