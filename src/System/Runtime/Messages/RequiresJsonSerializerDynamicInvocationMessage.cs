namespace System.Runtime.Messages;

/// <summary>
/// Provides with a message that is represented as deprecated message in JSON serialization dynamic invocation.
/// </summary>
internal static class RequiresJsonSerializerDynamicInvocationMessage
{
	/// <summary>
	/// Indicates the message that reports an error that user cannot use this constructor because of requirements on dynamic invocation
	/// by JSON serializer handling.
	/// </summary>
	public const string DynamicInvocationByJsonSerializerOnly =
		"You cannot use this constructor. This constructor is reserved by compiler and runtime.";
}
