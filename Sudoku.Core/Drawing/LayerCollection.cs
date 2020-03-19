using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Sudoku.Drawing.Layers;

namespace Sudoku.Drawing
{
	/// <summary>
	/// The collection of <see cref="Layer"/>s.
	/// </summary>
	public sealed class LayerCollection : IEnumerable<Layer>, IDisposable
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly ISet<Layer> _internalList = new HashSet<Layer>();


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public LayerCollection() { }

		/// <summary>
		/// Initializes an instance with specified layers.
		/// </summary>
		/// <param name="layers">The layers.</param>
		public LayerCollection(IEnumerable<Layer> layers)
		{
			foreach (var layer in layers)
			{
				_internalList.Add(layer);
			}
		}


		/// <inheritdoc/>
		public void Dispose()
		{
			foreach (var layer in _internalList)
			{
				layer.Dispose();
			}
		}

		/// <summary>
		/// Add a layer to this collection. If the list contains the layer,
		/// it will be replaced.
		/// </summary>
		/// <param name="layer">The layer.</param>
		public void Add(Layer layer)
		{
			if (_internalList.Contains(layer))
			{
				_internalList.Remove(layer);
			}

			_internalList.Add(layer);
		}

		/// <summary>
		/// Remove a layer from this collection.
		/// </summary>
		/// <param name="layer">The layer.</param>
		public void Remove(Layer layer) => _internalList.Remove(layer);

		/// <summary>
		/// Remove a series of layers from this collection.
		/// </summary>
		/// <param name="layerName">The layer name.</param>
		public void RemoveAll(string layerName)
		{
		Label_Start:
			foreach (var layer in _internalList)
			{
				if (layer.Name == layerName)
				{
					Remove(layer);
					goto Label_Start;
				}
			}
		}

		/// <summary>
		/// Remove a series of layers that match the specified condition.
		/// </summary>
		/// <param name="predicate">The condition.</param>
		public void RemoveWhen(Predicate<Layer> predicate)
		{
		Label_Start:
			foreach (var layer in _internalList)
			{
				if (predicate(layer))
				{
					Remove(layer);
					goto Label_Start;
				}
			}
		}

		/// <summary>
		/// To integrate to the target bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		public void IntegrateTo(Bitmap bitmap)
		{
			using var g = Graphics.FromImage(bitmap);
			foreach (var layer in _internalList)
			{
				layer.Redraw();
				g.DrawImage(layer.Target, 0, 0);
			}
		}

		/// <summary>
		/// Returns a <see cref="bool"/> value indicating whether the specified collection
		/// contains the specified layer whose name is same as specified argument.
		/// </summary>
		/// <param name="layerName">The layer name.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Contains(string layerName)
		{
			foreach (var layer in _internalList)
			{
				if (layer.Name == layerName)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns a <see cref="bool"/> value indicating whether the specified collection
		/// contains the specified layer.
		/// </summary>
		/// <param name="layer">The layer.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Contains(Layer layer) => _internalList.Contains(layer);

		/// <inheritdoc/>
		public IEnumerator<Layer> GetEnumerator() => _internalList.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Add a layer to this list. This operator can be used like:
		/// <code>
		/// var newLayer = ...;<br/>
		/// var layers = ...;<br/>
		/// layers += layer;
		/// </code>
		/// which is equivalent to
		/// <code>
		/// var newLayer = ...;<br/>
		/// var layers = ...;<br/>
		/// layers.Add(newLayer);
		/// </code>
		/// </summary>
		/// <param name="layers">The layers.</param>
		/// <param name="layer">The layer to add.</param>
		/// <returns>The reference of this collection.</returns>
		public static LayerCollection operator +(LayerCollection layers, Layer layer)
		{
			layers.Add(layer);
			return layers;
		}

		/// <summary>
		/// Remove a series of layers whose name is same as the specified argument.
		/// The operator can be used like:
		/// <code>layers -= "Basic";</code>
		/// which is equivalent to
		/// <code>
		/// layers.RemoveAll("Basic"); <br/>
		/// // or<br/>
		/// layers.RemoveWhen(layer => layer.Name == "Basic");
		/// </code>
		/// </summary>
		/// <param name="layers">The collection to remove layers.</param>
		/// <param name="layerName">The layer name.</param>
		/// <returns>The reference of this collection.</returns>
		public static LayerCollection operator -(LayerCollection layers, string layerName)
		{
			layers.RemoveAll(layerName);
			return layers;
		}

		/// <summary>
		/// Remove a series of layers that match the specified condition.
		/// The operator can be used like:
		/// <code>
		/// layers -= layer => layer.Name == "Basic";
		/// </code>
		/// which is equivalent to
		/// <code>
		/// layers.RemoveWhen(layer => layer.Name == "Basic");
		/// </code>
		/// </summary>
		/// <param name="layers">The layers.</param>
		/// <param name="predicate">The condition.</param>
		/// <returns>The reference of this collection.</returns>
		public static LayerCollection operator -(LayerCollection layers, Predicate<Layer> predicate)
		{
			layers.RemoveWhen(predicate);
			return layers;
		}
	}
}
