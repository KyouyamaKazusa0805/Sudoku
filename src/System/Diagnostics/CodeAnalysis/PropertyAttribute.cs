namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the backing generated member of this primary constructor is a property.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class PropertyAttribute : ParameterTargetAttribute
{
	/// <summary>
	/// <para>Indicates the extra setter expression. The expression is same declaration as auto-implemented properties.</para>
	/// <para>
	/// For example, if the property is declared as <c>public object? Property { get; private set; }</c>,
	/// the setter expression will be "<c>private set</c>". By default, this value will be <see langword="null"/>,
	/// which means the target property does not contain a setter.
	/// </para>
	/// </summary>
	[DisallowNull]
	public string? Setter { get; init; }

	/// <summary>
	/// Indicates the target emit property style. By default the value is <see cref="EmitPropertyStyle.AssignToProperty"/>.
	/// </summary>
	public EmitPropertyStyle EmitPropertyStyle { get; init; }
}
