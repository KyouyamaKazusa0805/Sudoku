namespace Sudoku.Resources;

/// <summary>
/// Specifies the location of the resource in the current assembly.
/// </summary>
/// <param name="resourceFileSpecifier">The specifier for the assembly.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed partial class ResourceLocationAttribute([Data] Type resourceFileSpecifier) : Attribute;
