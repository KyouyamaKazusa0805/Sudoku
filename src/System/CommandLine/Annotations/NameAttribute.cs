namespace System.CommandLine.Annotations;

/// <summary>
/// Defines an attribute that is applied to an enumeration field, indicating the name of the field.
/// </summary>
/// <param name="name">Indicates the name of the enumeration field.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed partial class NameAttribute([PrimaryConstructorParameter] string name) : Attribute;
