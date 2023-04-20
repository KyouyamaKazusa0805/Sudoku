namespace System.CommandLine.Annotations;

/// <summary>
/// Represents a root command description.
/// </summary>
/// <param name="name">Indicates the name of the command.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed partial class RootCommandAttribute([PrimaryConstructorParameter] string name) : Attribute
{
	/// <summary>
	/// <para>Indicates whether the command is special.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool IsSpecial { get; init; }

	/// <summary>
	/// Indicates the description of the command.
	/// </summary>
	/// <remarks><b>
	/// This property can be <see langword="null"/>. However, both properties <see cref="Description"/> and <see cref="DescriptionResourceKey"/>
	/// cannot be <see langword="null"/>.
	/// </b></remarks>
	/// <seealso cref="Description"/>
	/// <seealso cref="DescriptionResourceKey"/>
	[DisallowNull]
	public string? Description { get; init; }

	/// <summary>
	/// Indicates the description of the command, represented as a resource key related to a resource dictionary.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Description" path="/remarks"/>
	/// </remarks>
	/// <seealso cref="Description"/>
	/// <seealso cref="DescriptionResourceKey"/>
	[DisallowNull]
	public string? DescriptionResourceKey { get; init; }
}
