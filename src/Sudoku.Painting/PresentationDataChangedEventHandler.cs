using System.Collections.Generic;

namespace Sudoku.Painting
{
	/// <summary>
	/// Represents a event handler that triggered when the data is changed.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the data.</typeparam>
	/// <param name="args">The event arguments provided.</param>
	public delegate void PresentationDataChangedEventHandler<TUnmanaged>(
		ICollection<PaintingPair<TUnmanaged>>? args
	) where TUnmanaged : unmanaged;
}
