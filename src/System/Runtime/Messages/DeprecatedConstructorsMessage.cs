namespace System.Runtime.Messages;

/// <summary>
/// Provides with a message that is represented as deprecated message for deprecated constructors.
/// </summary>
internal static class DeprecatedConstructorsMessage
{
	/// <summary>
	/// Indicates the message that reports a warning that user cannot invoke this constructor because of meaninglessness.
	/// </summary>
	public const string ConstructorIsMeaningless = "This constructor is meaningless; please do not invoke this constructor.";
}
