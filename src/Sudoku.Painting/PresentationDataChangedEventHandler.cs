using System.Collections.Generic;

namespace Sudoku.Painting
{
	/// <summary>
	/// Represents a event handler that triggered when the data is changed.
	/// </summary>
	/// <typeparam name="T">The type of the data.</typeparam>
	/// <param name="args">The event arguments provided.</param>
	public delegate void PresentationDataChangedEventHandler<T>(ICollection<PaintingPair<T>>? args)
		where T : unmanaged;
}
