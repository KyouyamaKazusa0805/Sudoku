namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for method <see cref="object.ToString"/>, and make source generator generates their own <c>ToString</c>
/// method automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoOverridesToStringAttribute : Attribute
{
}
