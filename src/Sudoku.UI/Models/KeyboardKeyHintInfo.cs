namespace Sudoku.UI.Models;

/// <summary>
/// Provides with a keyboard key information that is used for construction for a <see cref="KeyboardKeyHint"/> instance.
/// </summary>
/// <param name="KeyNameResourceKey">The resource key that describes the key hint name.</param>
/// <param name="KeyboardPairs">The keyboard pairs. Please see the type <see cref="KeyboardPair"/>.</param>
/// <seealso cref="KeyboardKeyHint"/>
/// <seealso cref="KeyboardPair"/>
internal readonly record struct KeyboardKeyHintInfo(string KeyNameResourceKey, IList<KeyboardPair> KeyboardPairs);
