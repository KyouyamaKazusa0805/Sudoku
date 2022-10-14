namespace System.Runtime.Messages;

/// <summary>
/// Provides with a message that is represented as deprecated message in default-overridden methods
/// of <see langword="ref struct"/>-typed instances.
/// </summary>
internal static class RefStructDefaultImplementationMessage
{
	/// <summary>
	/// Indicates the message is telling user that the method <c>Equals</c> will become invalid
	/// in <see langword="ref struct"/> because the argument type is <see cref="object"/>,
	/// which is not able to be a target converted type for a <see langword="ref struct"/> instance.
	/// </summary>
	public const string OverriddenEqualsMethod =
		$"Method '{nameof(Equals)}' is never useful in a ref struct type, because the argument is a reference type, " +
		"but all ref struct instances cannot convert themselves into a reference-typed object.";

	/// <summary>
	/// Indicates the message is telling user that the method <c>GetHashCode</c> will become invalid
	/// in <see langword="ref struct"/> because the argument type is <see cref="object"/>,
	/// which is not able to be a target converted type for a <see langword="ref struct"/> instance.
	/// </summary>
	public const string OverriddenGetHashCodeMethod = $"Method '{nameof(GetHashCode)}' is never useful in a ref struct type.";
}
