namespace System
{
	/// <summary>
	/// Provides a data structure that stores a value type to avoid boxing and unboxing operations.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="Value">The value stored.</param>
	public sealed record WeakBox<T>(T Value) where T : struct;
}
