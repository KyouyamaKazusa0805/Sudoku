namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for source generation on properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NotifyBackingFieldAttribute : Attribute
{
}
