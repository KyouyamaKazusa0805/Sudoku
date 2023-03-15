namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute type that is applied to an enumeration field,
/// indicating the routed type.
/// </summary>
/// <param name="typeToRoute">The type to route.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class RouteToTypeAttribute(Type typeToRoute) : Attribute
{
	/// <summary>
	/// Indicates the type to be routed.
	/// </summary>
	public Type TypeToRoute { get; } = typeToRoute;
}
