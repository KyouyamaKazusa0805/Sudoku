namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the marked member will participate to-string operation.
/// </summary>
/// <param name="displayName">
/// Indicates the display name of the field or property to be changed.
/// By default the value is <see langword="null"/>, which means no conversion will be existing here.
/// </param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false)]
public sealed partial class StringMemberAttribute([PrimaryConstructorParameter] string? displayName = null) : Attribute;
