namespace System;

/// <summary>
/// Represents a type that supports a property returning a <see langword="null"/> reference.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface INullRef<TSelf> where TSelf : unmanaged, INullRef<TSelf>
{
	/// <summary>
	/// Represents the default reference of this type.
	/// </summary>
	public static abstract ref readonly TSelf NullRef { get; }
}
