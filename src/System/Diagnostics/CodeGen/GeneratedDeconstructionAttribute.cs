namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used as a mark that tells source generators to generate deconstruction methods of this type.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[Obsolete("This type is being deprecated because the future C# version will support the extension feature 'Roles & Extensions'. For more information, please visit Roslyn repo to learn more information.", false)]
public sealed class GeneratedDeconstructionAttribute : Attribute
{
}
