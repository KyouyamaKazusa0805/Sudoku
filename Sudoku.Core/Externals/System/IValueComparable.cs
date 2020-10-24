namespace System
{
	/// <inheritdoc cref="IComparable{T}"/>
	/// <typeparam name="TStruct">
	/// The type of objects to compare. Here it should be a <see langword="struct"/>.
	/// </typeparam>
	public interface IValueComparable<TStruct> : IComparable<TStruct> where TStruct : struct
	{
		/// <summary>
		/// Compares the current instance with another object of the same type and returns
		/// an integer that indicates whether the current instance precedes, follows, or
		/// occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="other">(<see langword="in"/> parameter) An object to compare with this instance.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The
		/// return value has these meanings: Value Meaning Less than zero This instance precedes
		/// other in the sort order. Zero This instance occurs in the same position in the
		/// sort order as other. Greater than zero This instance follows other in the sort
		/// order.
		/// </returns>
		int CompareTo(in TStruct other);


		/// <inheritdoc/>
		int IComparable<TStruct>.CompareTo(TStruct other) => CompareTo(other);
	}
}
