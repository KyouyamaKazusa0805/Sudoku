namespace SudokuStudio.Drawing;

/// <summary>
/// Represents a type that records a list of animating data.
/// </summary>
public sealed class AnimatedResultCollection : List<(Action Animating, Action Adding)>;
