namespace System;

/// <summary>
/// Defines a type that can compare the equality with other instances.
/// </summary>
/// <typeparam name="T">The type to compare.</typeparam>
internal interface IProxyEqualityComparer<in T>
{
	/// <summary>
	/// Determines whether two <typeparamref name="T"/> instances hold the same value.
	/// </summary>
	/// <param name="left">The left-side-operator instance to compare.</param>
	/// <param name="right">The right-side-operator instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	static abstract bool Equals([NotNullIfNotNull("right")] T? left, [NotNullIfNotNull("left")] T? right);
}
