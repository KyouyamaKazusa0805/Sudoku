using static System.AttributeTargets;

namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on deconstruction methods.
/// </summary>
[AttributeUsage(Class | Struct | Interface, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructionAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructionAttribute"/> instance
	/// via the specified property names you want to deconstruct.
	/// </summary>
	/// <param name="propertyNames">The name of the properties you want to deconstruct.</param>
	/// <exception cref="ArgumentException">Throws when the argument is empty.</exception>
	public AutoDeconstructionAttribute(params string[] propertyNames)
		=> PropertyNames = propertyNames is []
			? throw new ArgumentException("You must set at least one instance to be deconstructed.")
			: propertyNames;


	/// <summary>
	/// Indicates the property names whose corresponding properties will be able to be deconstructed.
	/// </summary>
	public string[] PropertyNames { get; }
}
