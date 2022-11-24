namespace System.CommandLine.Annotations;

/// <summary>
/// Represents a single-argument command. The command don't require any argument name such as "<c>-f</c>".
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class SingleArgumentCommandAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SingleArgumentCommandAttribute"/> instance via the specified description for the command.
	/// </summary>
	/// <param name="notation">The notation of the command.</param>
	public SingleArgumentCommandAttribute(string notation) => Notation = notation;


	/// <summary>
	/// <para>Indicates whether the command value can ignore the casing.</para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	public bool IgnoreCase { get; init; } = true;

	/// <summary>
	/// <para>Indicates whether the command is required.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool IsRequired { get; init; } = false;

	/// <summary>
	/// Indicates the fast notation to introduce the command, which is used for the displaying the help text.
	/// </summary>
	public string Notation { get; }

	/// <summary>
	/// Indicates the description of the argument.
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
	/// Indicates the description key of the argument in resource dictionary.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Description" path="/remarks"/>
	/// </remarks>
	/// <seealso cref="Description"/>
	/// <seealso cref="DescriptionResourceKey"/>
	[DisallowNull]
	public string? DescriptionResourceKey { get; init; }
}
