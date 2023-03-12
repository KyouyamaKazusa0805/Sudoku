namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Defines a required dependency module attribute.
/// </summary>
/// <typeparam name="T">The type of the dependency module.</typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequiredDependencyModuleAttribute<T> : CommandLineParsingItemAttribute where T : GroupModule, new()
{
}
