namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute type that is applied to an enumeration field,
/// indicating the routed type.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class RouteToTypeAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="RouteToTypeAttribute"/> instance via the type to route.
	/// </summary>
	/// <param name="typeToRoute">The type to route.</param>
	public RouteToTypeAttribute(Type typeToRoute) => TypeToRoute = typeToRoute;


	/// <summary>
	/// Indicates the type to be routed.
	/// </summary>
	public Type TypeToRoute { get; }
}
