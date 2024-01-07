namespace Sudoku.Resources;

/// <summary>
/// Specifies the location of the resource in the current assembly.
/// </summary>
/// <typeparam name="T">The type of the resource manager provider.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed partial class ResourceLocationAttribute<T> : Attribute where T : class;
