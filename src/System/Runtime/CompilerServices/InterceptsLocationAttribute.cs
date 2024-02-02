namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>
/// Interceptors are an experimental compiler feature planned to ship in .NET 8 (with support for C# only).
/// The feature may be subject to breaking changes or removal in a future release.
/// </para>
/// <para>
/// An interceptor is a method which can declaratively substitute a call to an interceptable method with a call to itself at compile time.
/// This substitution occurs by having the interceptor declare the source locations of the calls that it intercepts.
/// This provides a limited facility to change the semantics of existing code by adding new code to a compilation (e.g. in a source generator).
/// </para>
/// </summary>
/// <param name="filePath">The required file path.</param>
/// <param name="line">The line.</param>
/// <param name="character">The character position.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed partial class InterceptsLocationAttribute(
	[PrimaryConstructorParameter] string filePath,
	[PrimaryConstructorParameter] int line,
	[PrimaryConstructorParameter] int character
) : Attribute;
