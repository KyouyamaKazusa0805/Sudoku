using System;

namespace Sudoku.Graphics;

/// <summary>
/// Indicates the event data that is provided when the event is triggered.
/// </summary>
public sealed class GridOutlineOffsetChangedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="GridOutlineOffsetChangedEventArgs"/> instance.
	/// </summary>
	/// <param name="newValue">The size to modify the older one.</param>
	public GridOutlineOffsetChangedEventArgs(float newValue) => NewValue = newValue;


	/// <summary>
	/// Indicates the value that replace with the older one.
	/// </summary>
	public float NewValue { get; }
}
