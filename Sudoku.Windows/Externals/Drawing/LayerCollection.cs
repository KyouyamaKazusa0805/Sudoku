using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Sudoku.Drawing.Layers;
using System.Linq;
using Sudoku.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// The collection of <see cref="Layer"/>s.
	/// </summary>
	[Obsolete("Use the class 'TargetPainter' instead.", true)]
	public sealed class LayerCollection : IEnumerable<Layer>, IDisposable
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly ISet<Layer> _internalList;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public LayerCollection() => _internalList = new SortedSet<Layer>();

		/// <summary>
		/// Initializes an instance with specified layers.
		/// </summary>
		/// <param name="layers">The layers.</param>
		public LayerCollection(IEnumerable<Layer> layers) : this() => _internalList.AddRange(layers);


		/// <summary>
		/// Get the specified layer.
		/// </summary>
		/// <param name="layerName">The layer name.</param>
		/// <returns>The layer.</returns>
		public Layer? this[Type layerType] =>
			(from layer in _internalList where layer.Name == layerType.Name select layer).FirstOrDefault();


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
		/// Remove a layer.
		/// </summary>
		/// <typeparam name="TLayer">The type of the layer.</typeparam>
		/// <exception cref="ArgumentException">
		/// Throws when the specified type parameter is abstract.
		/// </exception>
		public void Remove<TLayer>() where TLayer : Layer
		{
			if (typeof(TLayer).IsAbstract)
			{
				throw new ArgumentException(
					$"The specified type parameter is invalid: It is not instance class, but abstract.",
					nameof(TLayer));
			}

			string name = typeof(TLayer).Name;
			foreach (var layer in _internalList)
			{
				if (layer.Name == name)
				{
					Remove(layer);
					return;
				}
			}
		}

		/// <summary>
		/// Remove a layer from this collection.
		/// If the collection does not contain any layers with the specified value,
		/// the method will do nothing rather than throwing an exception.
		/// </summary>
		/// <param name="layer">The layer.</param>
		public void Remove(Layer layer) => _internalList.Remove(layer);

		/// <summary>
		/// Remove a series of layers that match the specified condition.
		/// If the collection does not contain any layers with the specified value,
		/// the method will do nothing rather than throwing an exception.
		/// </summary>
		/// <param name="predicate">The condition.</param>
		public void RemoveWhen(Predicate<Layer> predicate)
		{
			bool flag;
			do
			{
				foreach (var layer in _internalList)
				{
					if (predicate(layer))
					{
						Remove(layer);
						continue;
					}
				}

				flag = false;
			} while (!flag);
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
		/// To integrate to the target bitmap, and then using the specified <see cref="Graphics"/>
		/// instance to integrate all target images.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/> instance.</param>
		public void IntegrateTo(Graphics g)
		{
			foreach (var layer in _internalList)
			{
				layer.Redraw();
				g.DrawImage(layer.Target, 0, 0);
			}
		}

		/// <inheritdoc/>
		public IEnumerator<Layer> GetEnumerator() => _internalList.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
