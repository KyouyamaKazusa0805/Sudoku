namespace System.Runtime.Messages;

/// <summary>
/// Provides with a message that is represented as warning message used by module initializers.
/// </summary>
internal static class ModuleInitializerMessage
{
	/// <summary>
	/// Indicates the message is telling user that module initializers cannot be used manually.
	/// </summary>
	public const string ModuleInitializerCannotBeCalledManually =
		"This is a method called 'Module Initializer'. The method is reserved for compiler usage and you cannot call this method explicitly.";
}
