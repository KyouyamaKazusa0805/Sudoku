namespace System
{
	/// <inheritdoc cref="IEquatable{T}"/>
	/// <typeparam name="TStruct">
	/// The type of objects to compare. Here it should be a <see langword="struct"/>.
	/// </typeparam>
	public interface IValueEquatable<TStruct> : IEquatable<TStruct> where TStruct : struct
	{
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// <see langword="true"/> if the current object is equal to the other parameter;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		bool Equals(in TStruct other);

		/// <inheritdoc/>
		bool IEquatable<TStruct>.Equals(TStruct other) => Equals(other);
	}
}
