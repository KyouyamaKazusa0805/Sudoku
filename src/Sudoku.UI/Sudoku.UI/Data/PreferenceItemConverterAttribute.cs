namespace Sudoku.UI.Data;

/// <summary>
/// Defines a <see cref="PreferenceItemConverterAttribute{TFrom, TTo}"/> instance that tells the runtime
/// and the user that the type is a converter, from a specified type to a specified type.
/// </summary>
/// <typeparam name="TFrom">The type from.</typeparam>
/// <typeparam name="TTo">The type to.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PreferenceItemConverterAttribute<TFrom, TTo> : Attribute
{
}