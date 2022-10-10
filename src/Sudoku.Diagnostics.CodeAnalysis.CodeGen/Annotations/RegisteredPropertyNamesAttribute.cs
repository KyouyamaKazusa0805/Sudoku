namespace Sudoku.Diagnostics.CodeGen.Annotations;

/// <summary>
/// The Registered property names attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RegisteredPropertyNamesAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="RegisteredPropertyNamesAttribute"/> instance via the specified registered property names.
	/// </summary>
	/// <param name="accessibility">The accessibility.</param>
	/// <param name="registeredPropertyNames">Registered property names.</param>
	public RegisteredPropertyNamesAttribute(CSharpAccessibility accessibility, params string[] registeredPropertyNames)
		=> (RegisteredPropertyNames, Accessibility) = (registeredPropertyNames, accessibility);


	/// <summary>
	/// Indicates the registered property names.
	/// </summary>
	public string[] RegisteredPropertyNames { get; }

	/// <summary>
	/// The accessibility.
	/// </summary>
	public CSharpAccessibility Accessibility { get; }
}
