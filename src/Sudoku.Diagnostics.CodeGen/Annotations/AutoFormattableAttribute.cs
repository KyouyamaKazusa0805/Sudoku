namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute that is used for a <see langword="class"/> or <see langword="struct"/>
/// as a mark that interacts with the source generator, to tell the source generator that
/// it'll generate the source code for <c>ToString</c> overriden,
/// and being with <see cref="IFormattable.ToString(string?, IFormatProvider?)"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class AutoFormattableAttribute : Attribute
{
}