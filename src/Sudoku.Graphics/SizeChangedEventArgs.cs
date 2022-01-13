using System;

namespace Sudoku.Graphics;

/// <summary>
/// Indicates the event data that is provided when the event is triggered.
/// </summary>
public sealed class PictureSizeChangedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="PictureSizeChangedEventArgs"/> instance.
	/// </summary>
	/// <param name="newValue">The size to modify the older one.</param>
	public PictureSizeChangedEventArgs(float newValue) => NewValue = newValue;


	/// <summary>
	/// Indicates the value that replace with the older one.
	/// </summary>
	public float NewValue { get; }
}
