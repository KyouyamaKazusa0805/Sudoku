namespace System.CommandLine;

/// <summary>
/// Defines a base attribute type that is used for the inheritance of the attribute types
/// used in the command line environment.
/// </summary>
public abstract class CommandLineAttributeBase : Attribute
{
	/// <summary>
	/// Assigns the basic information for a <see cref="CommandLineAttributeBase"/>-typed instance.
	/// </summary>
	private protected CommandLineAttributeBase()
	{
	}
}
