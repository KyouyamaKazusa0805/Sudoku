namespace System
{
	/// <inheritdoc cref="ICloneable"/>
	/// <typeparam name="T">
	/// The type of this instance. This type should be only a class because the cloning
	/// operation is needed only in reference types, while the value types will be passed
	/// by value, at this time all value members (fields and properties) will be copied
	/// one by one.
	/// </typeparam>
	/// <remarks>
	/// Different with <see cref="ICloneable"/>, the cloneation
	/// has the same type with this existing instance.
	/// </remarks>
	/// <seealso cref="ICloneable"/>
	public interface ICloneable<out T> : ICloneable where T : class
	{
		/// <inheritdoc cref="ICloneable.Clone"/>
		new T Clone();

		/// <inheritdoc/>
		object ICloneable.Clone() => Clone();
	}
}
