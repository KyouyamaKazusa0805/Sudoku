using System;
using System.Diagnostics;
using System.Drawing;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Define a layer for displaying on the form controls.
	/// </summary>
	[DebuggerStepThrough]
	public abstract class Layer : IComparable<Layer>, IDisposable, IEquatable<Layer>
	{
		/// <summary>
		/// Indicates a basic string format.
		/// </summary>
		protected static readonly StringFormat DefaultStringFormat = new StringFormat
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center
		};


		/// <summary>
		/// The internal pointer converter.
		/// </summary>
		protected readonly PointConverter _pointConverter;


		/// <summary>
		/// Provides initialization for inherit instances.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		protected Layer(PointConverter pointConverter)
		{
			_pointConverter = pointConverter;
			(Width, Height) = _pointConverter.ControlSize;
		}


		/// <summary>
		/// Indicates the width of the target image.
		/// </summary>
		public float Width { get; }

		/// <summary>
		/// Indicates the height of the target image.
		/// </summary>
		public float Height { get; }

		/// <summary>
		/// Indicates the priority of the layer.
		/// If the value is greater, the order of drawing is more forward.
		/// </summary>
		public abstract int Priority { get; }

		/// <summary>
		/// Indicates the name of each kind of layers.
		/// This value is used for compare two layers.
		/// </summary>
		public virtual string Name => GetType().Name;

		/// <summary>
		/// Indicates the internal bitmap. If the value is <see langword="null"/>,
		/// the current state is invalid (unavailable).
		/// </summary>
		public Image? Target { get; protected set; }


		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) => obj is Layer comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Layer other) => Name == other.Name;

		/// <inheritdoc/>
		public sealed override int GetHashCode() => Name.GetHashCode();

		/// <inheritdoc/>
		public sealed override string ToString() => Name;

		/// <inheritdoc/>
		public void Dispose() => Target?.Dispose();

		/// <inheritdoc/>
		public int CompareTo(Layer other) => Priority.CompareTo(other.Priority);

		/// <summary>
		/// To redraw to the bitmap.
		/// </summary>
		public void Redraw() => Draw();

		/// <summary>
		/// To draw into the <see cref="Target"/> image.
		/// </summary>
		/// <seealso cref="Target"/>
		protected abstract void Draw();


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Layer left, Layer right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Layer left, Layer right) => !(left == right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThan"]'/>
		public static bool operator >(Layer left, Layer right) => left.CompareTo(right) > 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThanOrEqual"]'/>
		public static bool operator >=(Layer left, Layer right) => left.CompareTo(right) >= 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThan"]'/>
		public static bool operator <(Layer left, Layer right) => left.CompareTo(right) < 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThanOrEqual"]'/>
		public static bool operator <=(Layer left, Layer right) => left.CompareTo(right) <= 0;
	}
}
