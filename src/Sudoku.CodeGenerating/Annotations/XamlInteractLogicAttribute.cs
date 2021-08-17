namespace Sudoku.CodeGenerating;

/// <summary>
/// Indicates the class is the interact logic bound with the specified XAML file.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class XamlInteractLogicAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="XamlInteractLogicAttribute"/> instance with the specified file name.
	/// </summary>
	/// <param name="xamlFileName"></param>
	public XamlInteractLogicAttribute(string xamlFileName) => XamlFileName = xamlFileName;


	/// <summary>
	/// Indicates the XAML file name.
	/// </summary>
	public string XamlFileName { get; }
}
