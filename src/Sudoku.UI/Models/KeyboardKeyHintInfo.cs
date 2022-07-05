namespace Sudoku.UI.Models;

/// <summary>
/// Provides with a keyboard key information that is used for construction for an instance.
/// </summary>
/// <param name="KeyName">The key hint name.</param>
/// <param name="KeyboardPair">The keyboard pairs. Please see the type <see cref="Models.KeyboardPair"/>.</param>
/// <seealso cref="Models.KeyboardPair"/>
internal readonly record struct KeyboardKeyHintInfo(string KeyName, KeyboardPair KeyboardPair);
