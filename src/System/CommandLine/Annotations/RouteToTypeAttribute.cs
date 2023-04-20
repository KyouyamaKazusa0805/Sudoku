namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute type that is applied to an enumeration field,
/// indicating the routed type.
/// </summary>
/// <param name="typeToRoute">Indicates the type to be routed.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed partial class RouteToTypeAttribute([PrimaryConstructorParameter] Type typeToRoute) : Attribute;
