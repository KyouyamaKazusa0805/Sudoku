namespace System.CommandLine.Annotations;

/// <summary>
/// Represents a double-argument command. The command requires an argument name, following with a real value,
/// such as "<c>-g grid</c>".
/// </summary>
/// <param name="shortName">The short name of the command.</param>
/// <param name="fullName">The full name of the command. The command should not contain the prefix.</param>
/// <exception cref="ArgumentOutOfRangeException">
/// Throws when the argument <paramref name="shortName"/> is not a letter.
/// </exception>
/// <exception cref="ArgumentException">
/// Throws when the argument <paramref name="fullName"/> doesn't start with a letter, or a hyphen.
/// </exception>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DoubleArgumentsCommandAttribute(char shortName, string fullName) : Attribute
{
	/// <summary>
	/// <para>Indicates whether the command is required.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool IsRequired { get; init; } = false;

	/// <summary>
	/// Indicates the short name.
	/// </summary>
	public char ShortName { get; } = shortName;

	/// <summary>
	/// Indicates the full name.
	/// </summary>
	public string FullName { get; } = fullName;

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
