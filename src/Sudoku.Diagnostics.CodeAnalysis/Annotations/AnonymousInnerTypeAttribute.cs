namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an attribute to tell the source generator the type applied this attribute
/// is an anonymous inner type.
/// </summary>
/// <remarks>
/// An <b>anonymous inner type</b> is a type that is only used for the compiler or .NET API using,
/// or an instant type that will be thrown after having been used.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class AnonymousInnerTypeAttribute : Attribute
{
}
