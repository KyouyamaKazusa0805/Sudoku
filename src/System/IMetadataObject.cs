namespace System;

/// <summary>
/// Defines a metadata object.
/// </summary>
/// <typeparam name="T">The type of metadata.</typeparam>
public interface IMetadataObject<out T> where T : class?
{
	/// <summary>
	/// Try to fetch metadata of the object.
	/// </summary>
	/// <returns>The metadata returned.</returns>
	public abstract T GetMetadata();
}
