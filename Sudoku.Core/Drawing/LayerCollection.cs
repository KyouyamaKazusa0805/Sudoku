using System.Collections.Generic;
using Sudoku.Drawing.Layers;

namespace Sudoku.Drawing
{
	/// <summary>
	/// The collection of <see cref="Layer"/>s.
	/// </summary>
	public sealed class LayerCollection
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly IList<Layer> _internalList = new List<Layer>();


		public void Add(Layer layer) => _internalList.Add(layer);

		public void Remove(Layer layer) => _internalList.Remove(layer);
	}
}
