namespace Sudoku.Reflection;

/// <summary>
/// Represents a list of <see cref="string"/> text as messages to describe what kind of error.
/// </summary>
public static class ReflectionMessage
{
	/// <summary>
	/// Indicates the error message is that you cannot directly access a certain member,
	/// because it must be referenced by reflection, and binds a resource value.
	/// </summary>
	public const string RequiresReflectionDueToResourceDictionary = "You can only reference this method by reflection, because this method is related to the value stored in resource dictionary, with a same name.";
}
