namespace System.Runtime.Messages;

/// <summary>
/// Provides with a message that is represented as deprecated message for compiler invocation.
/// </summary>
internal static class RequiresCompilerInvocationMessage
{
	/// <summary>
	/// Indicates the message that reports an error that user cannot use this constructor because of requirements on compiler invocation.
	/// </summary>
	public const string CompilerInvocationOnly = "You cannot use this constructor due to unnecessary. This constructor is reserved by compiler and runtime.";
}
