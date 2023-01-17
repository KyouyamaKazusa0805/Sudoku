namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for source generation on properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NotifyBackingFieldAttribute : Attribute
{
	/// <summary>
	/// Indicates the accessibility of the generated property. The default value is <see cref="GeneralizedAccessibility.Public"/>.
	/// </summary>
	/// <seealso cref="GeneralizedAccessibility.Public"/>
	public GeneralizedAccessibility Accessibility { get; init; } = GeneralizedAccessibility.Public;
}
