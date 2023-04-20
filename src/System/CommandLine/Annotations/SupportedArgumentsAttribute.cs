namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute that is applied to an enumeration typed field, indicating all specified commands
/// are supported to be used as a part of the command line arguments when introducing the enumeration instance.
/// </summary>
/// <param name="supportedNames">Indicates the supported names corresponding to the current enumeration field.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed partial class SupportedArgumentsAttribute(
	[PrimaryConstructorParameter(GeneratedMemberName = "SupportedArguments")] params string[] supportedNames
) : Attribute
{
	/// <summary>
	/// <para>
	/// Indicates whether the parser will ignore the case of the names when parsing to the actual instance.
	/// </para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	public bool IgnoreCase { get; init; } = true;
}
