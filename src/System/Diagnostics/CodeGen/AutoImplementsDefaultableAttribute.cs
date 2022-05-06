namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on automatically implementing
/// <see cref="IDefaultable{T}"/>.
/// </summary>
/// <seealso cref="IDefaultable{T}"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoImplementsDefaultableAttribute : SourceGeneratorOptionProviderAttribute, IPatternProvider
{
	/// <summary>
	/// Initializes an <see cref="AutoImplementsDefaultableAttribute"/> instance via the specified default field name.
	/// </summary>
	/// <param name="defaultFieldName">The default field name.</param>
	public AutoImplementsDefaultableAttribute(string defaultFieldName) => DefaultFieldName = defaultFieldName;


	/// <summary>
	/// Indicates the description of the default field.
	/// </summary>
	public string? DefaultFieldDescription { get; init; }

	/// <summary>
	/// Indicates the name of the default field.
	/// </summary>
	public string DefaultFieldName { get; }

	/// <summary>
	/// Indicates the pattern that initializes the default field.
	/// </summary>
	[DisallowNull]
	public string? Pattern { get; init; }

	/// <summary>
	/// Indicates the expression that determines whether the current instance is default.
	/// The default value is <see langword="null"/>.
	/// </summary>
	public string? IsDefaultExpression { get; init; }
}
