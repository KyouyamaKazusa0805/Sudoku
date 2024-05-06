namespace Sudoku.Measuring;

/// <summary>
/// Provides with an attribute type that describes the function is or will be exported outside the API,
/// as isolated function to be called.
/// </summary>
/// <param name="alias">Indicates the alias identifier to be used.</param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed partial class ExportFunctionAttribute([PrimaryConstructorParameter] string alias) : Attribute;
