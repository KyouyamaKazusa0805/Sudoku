namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute that is applied to an enumeration typed field, indicating all specified commands
/// are supported to be used as a part of the command line arguments when introducing the enumeration instance.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SupportedArgumentsAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SupportedArgumentsAttribute"/> instance
	/// via the specified array of supported names.
	/// </summary>
	/// <param name="supportedNames">The supported names corresponding to the current enumeration field.</param>
	public SupportedArgumentsAttribute(params string[] supportedNames) => SupportedArguments = supportedNames;


	/// <summary>
	/// Indicates the supported names corresponding to the current enumeration field.
	/// </summary>
	public string[] SupportedArguments { get; }

	/// <summary>
	/// <para>
	/// Indicates whether the parser will ignore the case of the names when parsing to the actual instance.
	/// </para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	public bool IgnoreCase { get; init; } = true;
}
