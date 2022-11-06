namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used as a mark that tells source generators to generate deconstruction methods of this type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GeneratedDeconstructionAttribute : Attribute
{
}
