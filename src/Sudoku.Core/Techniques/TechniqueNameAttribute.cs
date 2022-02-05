namespace Sudoku.Techniques;

/// <summary>
/// Defines the name of the technique field, which is stored in the type <see cref="Technique"/>.
/// </summary>
/// <seealso cref="Technique"/>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class TechniqueNameAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="TechniqueNameAttribute"/> instance via the specified name.
	/// </summary>
	/// <param name="name">The technique name.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TechniqueNameAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name of the technique.
	/// </summary>
	public string Name { get; }
}
