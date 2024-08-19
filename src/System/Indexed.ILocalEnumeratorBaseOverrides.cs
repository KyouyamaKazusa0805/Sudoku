namespace System;

public partial struct Indexed<T>
{
	/// <summary>
	/// Represents an interface type that constraints the usages and members declared in an enumerator type defined in this type.
	/// </summary>
	/// <typeparam name="TSelf">The type itself.</typeparam>
	private interface ILocalEnumerator<TSelf> :
		IEnumerator<T>,
		IEnumerable<T>,
		IToArrayMethod<TSelf, T>
		where TSelf : ILocalEnumerator<TSelf>, allows ref struct
	{
		/// <summary>
		/// Indicates the previous element. <b>This property may cause "out of bound" critical problem.</b>
		/// </summary>
		public abstract T UnsafePrevious { get; }

		/// <summary>
		/// Indicates the next element. <b>This property may cause "out of bound" critical problem.</b>
		/// </summary>
		public abstract T UnsafeNext { get; }

		/// <summary>
		/// Indicates the field <c>_pos</c>.
		/// </summary>
		protected abstract int Pos { get; }

		/// <summary>
		/// Indicates the field <c>_length</c>.
		/// </summary>
		protected abstract int Length { get; }


		/// <inheritdoc cref="this[int, IndexingMode]"/>
		public abstract T this[int index] { get; }

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>The element.</returns>
		public abstract T this[int index, IndexingMode mode] { get; }


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public new abstract TSelf GetEnumerator();
	}
}
