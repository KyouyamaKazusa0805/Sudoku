namespace System
{
	/// <summary>
	/// Supports cloning, which creates a new instance of a class with the same value
	/// as an existing instance. Different than <see cref="ICloneable"/>, the cloneation
	/// has the same type with this existing instance.
	/// </summary>
	/// <typeparam name="T">
	/// The type of this instance. This type should be only a class because the cloning
	/// operation is needed only in reference types, while the value types will be passed
	/// by value, at this time all value members (fields and properties) will be copied
	/// one by one.
	/// </typeparam>
	/// <seealso cref="ICloneable"/>
	public interface ICloneable<out T> : ICloneable where T : class
	{
		/// <summary>
		/// Creates a new instance that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// The instance having the same type with the base one. Because the type is
		/// used and defined before using this method, so the return type will be
		/// never <see langword="null"/>.
		/// </returns>
		new T Clone();

		/// <inheritdoc/>
		object ICloneable.Clone() => Clone();
	}
}
